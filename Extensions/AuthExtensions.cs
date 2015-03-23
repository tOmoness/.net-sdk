// -----------------------------------------------------------------------
// <copyright file="AuthExtensions.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif
using MixRadio;
using MixRadio.Types;
#if NETFX_CORE || WINDOWS_PHONE_APP
using Windows.Security.Authentication.Web;
#endif

#if WINDOWS_PHONE || NETFX_CORE || WINDOWS_PHONE_APP
namespace MixRadio
{
    /// <summary>
    /// Authentication helper class to migrate to the PCL version of the MixRadio SDK
    /// </summary>
    public static class AuthExtensions
    {
        private const string TokenCacheFile = "NokiaMusicOAuthToken.json";

#if WINDOWS_PHONE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="browser">The browser control to use to drive authentication.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <param name="oauthRedirectUri">The OAuth completed URI.</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// browser;You must supply a web browser to allow user interaction
        /// or
        /// clientSecret;You must supply your app client secret obtained during app registration
        /// </exception>
        /// <remarks>
        /// Sorry, this method is messy due to the platform differences
        /// </remarks>
        public static async Task<AuthResultCode> AuthenticateUserAsync(this MusicClient client, string clientSecret, Scope scopes, WebBrowser browser, CancellationToken? cancellationToken = null, string oauthRedirectUri = MusicClient.DefaultOAuthRedirectUri)
        {
            if (browser == null)
            {
                throw new ArgumentNullException("browser", "You must supply a web browser to allow user interaction");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "You must supply your app client secret obtained during app registration");
            }

            // See if we have a cached token...
            AuthResultCode cachedResult = await AuthenticateUserAsync(client, clientSecret, cancellationToken).ConfigureAwait(false);
            if (cachedResult == AuthResultCode.Success)
            {
                return cachedResult;
            }

            var browserController = new OAuthBrowserController();

            CancellationToken token = cancellationToken ?? CancellationToken.None;

            await Task.Run(() => browserController.DriveAuthProcess(browser, client.GetAuthenticationUri(scopes), oauthRedirectUri, token), token);

            AuthResultCode authResult = browserController.ResultCode;
            if (authResult != AuthResultCode.Cancelled)
            {
                // Grab the results and kill the browser controller
                string code = browserController.AuthorizationCode;
                browserController = null;

                // Move on to obtain a token
                if (authResult == AuthResultCode.Success)
                {
                    var authToken = await client.GetAuthenticationTokenAsync(clientSecret, code, cancellationToken);
                    if (authToken != null)
                    {
                        await StoreOAuthToken(authToken, client.ClientId, clientSecret, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            return authResult;
        }
#endif

#if NETFX_CORE || WINDOWS_PHONE_APP
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="oauthRedirectUri">The OAuth completed URI.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        /// <remarks>
        /// Sorry, this method is messy due to the platform differences!
        /// </remarks>
        public static async Task<AuthResultCode> AuthenticateUserAsync(this MusicClient client, string clientSecret, Scope scopes, string oauthRedirectUri = MusicClient.DefaultOAuthRedirectUri, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrEmpty(oauthRedirectUri))
            {
                throw new ArgumentNullException("oauthRedirectUri", "You must supply your OAuth Redirect URI to allow user interaction");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "You must supply your app client secret obtained during app registration");
            }

            // See if we have a cached token...
            AuthResultCode cachedResult = await AuthenticateUserAsync(client, clientSecret, cancellationToken).ConfigureAwait(false);
            if (cachedResult == AuthResultCode.Success)
            {
                return cachedResult;
            }

#if WINDOWS_PHONE_APP
            WebAuthenticationBroker.AuthenticateAndContinue(client.GetAuthenticationUri(scopes), new Uri(oauthRedirectUri));
            return AuthResultCode.InProgress;
#elif WINDOWS_APP
            var authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, client.GetAuthenticationUri(scopes), new Uri(oauthRedirectUri));
            return await CompleteAuthenticateUserAsync(client, clientSecret, authResult, cancellationToken);
#endif
        }

        /// <summary>
        /// Completes the authenticate user call.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="result">The result received through LaunchActivatedEventArgs.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>This method is for Windows Phone 8.1 use</remarks>
        public static async Task<AuthResultCode> CompleteAuthenticateUserAsync(this MusicClient client, string clientSecret, WebAuthenticationResult result, CancellationToken? cancellationToken = null)
        {
            if (result != null)
            {
                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var authResult = AuthResultCode.Unknown;
                    string code = null;

                    OAuthResultParser.ParseQuerystringForCompletedFlags(result.ResponseData, out authResult, out code);
                    if (authResult == AuthResultCode.Success)
                    {
                        var authToken = await client.GetAuthenticationTokenAsync(clientSecret, code, cancellationToken);
                        if (authToken != null)
                        {
                            await StoreOAuthToken(authToken, client.ClientId, clientSecret, cancellationToken).ConfigureAwait(false);
                        }
                    }

                    return authResult;
                }
            }

            return AuthResultCode.Unknown;
        }
#endif

        /// <summary>
        /// Gets a value indicating whether a user token is cached and the silent version of AuthenticateUserAsync can be used.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// <c>true</c> if a user token is cached; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> IsUserTokenCached(this MusicClient client, CancellationToken? cancellationToken = null)
        {
            return await StorageHelper.FileExistsAsync(TokenCacheFile).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts a silent authentication a user to enable the user data APIs using a cached access token.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>
        /// This overload of AuthenticateUserAsync can only be used once the user has gone through the OAuth flow and given permission to access their data.
        /// </remarks>
        public static async Task<AuthResultCode> AuthenticateUserAsync(this MusicClient client, string clientSecret, CancellationToken? cancellationToken = null)
        {
            if (client.IsUserAuthenticated && client.IsUserTokenActive)
            {
                return AuthResultCode.Success;
            }

            // Attempt to load a cached token...
            string cachedToken = await StorageHelper.ReadTextAsync(TokenCacheFile).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(cachedToken))
            {
#if NETFX_CORE || WINDOWS_PHONE_APP
                // Token is encrypted to stop prying eyes on Win8
                string decodedJson = EncryptionHelper.Decrypt(cachedToken, clientSecret, client.ClientId);
#else
                string decodedJson = cachedToken;
#endif

                client.SetAuthenticationToken(AuthToken.FromJson(decodedJson));

                // Check expiry...
                if (!client.IsUserTokenActive)
                {
                    // expired -> need to Refresh and cache
                    var token = await client.RefreshAuthenticationTokenAsync(clientSecret, cancellationToken);
                    if (token != null)
                    {
                        await StoreOAuthToken(token, client.ClientId, clientSecret, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        // Failed to refresh the token - remove the cached token in case it's causing problems...
                        await StorageHelper.DeleteFileAsync(TokenCacheFile).ConfigureAwait(false);
                        return AuthResultCode.FailedToRefresh;
                    }
                }
                else
                {
                    return AuthResultCode.Success;
                }
            }

            return AuthResultCode.NoCachedToken;
        }

        /// <summary>
        /// Deletes any cached authentication token.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An async task
        /// </returns>
        public static async Task DeleteAuthenticationTokenAsync(this MusicClient client, CancellationToken? cancellationToken = null)
        {
            client.SetAuthenticationToken(null);

            if (await StorageHelper.FileExistsAsync(TokenCacheFile).ConfigureAwait(false))
            {
                await StorageHelper.DeleteFileAsync(TokenCacheFile).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Stores the OAuth token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="clientId">The client ID.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A Task for async execution
        /// </returns>
        private static async Task StoreOAuthToken(AuthToken token, string clientId, string clientSecret, CancellationToken? cancellationToken = null)
        {
            var tokenString = token.ToString();
#if NETFX_CORE || WINDOWS_PHONE_APP
            // Encrypt to stop prying eyes on Win8
            await StorageHelper.WriteTextAsync(TokenCacheFile, EncryptionHelper.Encrypt(tokenString, clientSecret, clientId)).ConfigureAwait(false);
#else
            await StorageHelper.WriteTextAsync(TokenCacheFile, tokenString).ConfigureAwait(false);
#endif
        }
    }
}
#endif
