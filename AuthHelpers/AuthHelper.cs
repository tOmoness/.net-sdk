// -----------------------------------------------------------------------
// <copyright file="AuthHelper.cs" company="MixRadio">
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
using Nokia.Music;
using Nokia.Music.Types;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif

#if WINDOWS_PHONE || NETFX_CORE || WINDOWS_PHONE_APP
namespace MixRadio.AuthHelpers
{
    /// <summary>
    /// Authentication helper class to migrate to the PCL version of the MixRadio SDK
    /// </summary>
    internal class AuthHelper
    {
        private const string TokenCacheFile = "NokiaMusicOAuthToken.json";
        private MusicClient _mixRadioClient = null;
#if WINDOWS_PHONE
        private OAuthBrowserController _browserController;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="client">The MixRadio client.</param>
        public AuthHelper(MusicClient client)
        {
            this._mixRadioClient = client;
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="browser">The browser control to use to drive authentication.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        /// <remarks>Sorry, this method is messy due to the platform differences</remarks>
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, WebBrowser browser, CancellationToken? cancellationToken = null)
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
            AuthResultCode cachedResult = await this.AuthenticateUserAsync(clientSecret, cancellationToken).ConfigureAwait(false);
            if (cachedResult == AuthResultCode.Success)
            {
                return cachedResult;
            }

            if (this._browserController == null)
            {
                this._browserController = new OAuthBrowserController();
            }

            CancellationToken token = cancellationToken ?? CancellationToken.None;

            await Task.Run(() => this._browserController.DriveAuthProcess(browser, this._mixRadioClient.GetAuthenticationUri(scopes), token), token);

            AuthResultCode authResult = this._browserController.ResultCode;
            if (authResult != AuthResultCode.Cancelled)
            {
                // Grab the results and kill the browser controller
                string code = this._browserController.AuthorizationCode;
                this._browserController = null;

                // Move on to obtain a token
                if (authResult == AuthResultCode.Success)
                {
                    var authToken = await this._mixRadioClient.GetAuthenticationTokenAsync(clientSecret, code, cancellationToken);
                    if (authToken != null)
                    {
                        await this.StoreOAuthToken(authToken, clientSecret, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            return authResult;
        }
#endif

#if NETFX_CORE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
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
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, string oauthRedirectUri = MusicClient.DefaultOAuthRedirectUri, CancellationToken? cancellationToken = null)
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
            AuthResultCode cachedResult = await this.AuthenticateUserAsync(clientSecret, cancellationToken).ConfigureAwait(false);
            if (cachedResult == AuthResultCode.Success)
            {
                return cachedResult;
            }

#if WINDOWS_PHONE_APP
            WebAuthenticationBroker.AuthenticateAndContinue(this._mixRadioClient.GetAuthenticationUri(scopes), new Uri(oauthRedirectUri));
            return AuthResultCode.InProgress;
#elif WINDOWS_APP
            var authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, this._mixRadioClient.GetAuthenticationUri(scopes), new Uri(oauthRedirectUri));
            return await this.CompleteAuthenticateUserAsync(clientSecret, authResult, cancellationToken);
#endif
        }

        /// <summary>
        /// Completes the authenticate user call.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="result">The result received through LaunchActivatedEventArgs.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>This method is for Windows Phone 8.1 use</remarks>
        public async Task<AuthResultCode> CompleteAuthenticateUserAsync(string clientSecret, WebAuthenticationResult result, CancellationToken? cancellationToken = null)
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
                        var authToken = await this._mixRadioClient.GetAuthenticationTokenAsync(clientSecret, code, cancellationToken);
                        if (authToken != null)
                        {
                            await this.StoreOAuthToken(authToken, clientSecret, cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// <c>true</c> if a user token is cached; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsUserTokenCached(CancellationToken? cancellationToken = null)
        {
            return await StorageHelper.FileExistsAsync(TokenCacheFile).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts a silent authentication a user to enable the user data APIs using a cached access token.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>
        /// This overload of AuthenticateUserAsync can only be used once the user has gone through the OAuth flow and given permission to access their data.
        /// </remarks>
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, CancellationToken? cancellationToken = null)
        {
            if (this._mixRadioClient.IsUserAuthenticated && this._mixRadioClient.IsUserTokenActive)
            {
                return AuthResultCode.Success;
            }

            // Attempt to load a cached token...
            string cachedToken = await StorageHelper.ReadTextAsync(TokenCacheFile).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(cachedToken))
            {
#if NETFX_CORE
                // Token is encrypted to stop prying eyes on Win8
                string decodedJson = EncryptionHelper.Decrypt(cachedToken, clientSecret, this._mixRadioClient.ClientId);
#else
                string decodedJson = cachedToken;
#endif

                this._mixRadioClient.SetAuthenticationToken(AuthToken.FromJson(decodedJson));

                // Check expiry...
                if (!this._mixRadioClient.IsUserTokenActive)
                {
                    // expired -> need to Refresh and cache
                    var token = await this._mixRadioClient.RefreshAuthenticationTokenAsync(clientSecret, cancellationToken);
                    if (token != null)
                    {
                        await this.StoreOAuthToken(token, clientSecret, cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An async task
        /// </returns>
        public async Task DeleteAuthenticationTokenAsync(CancellationToken? cancellationToken = null)
        {
            this._mixRadioClient.SetAuthenticationToken(null);

            if (await StorageHelper.FileExistsAsync(TokenCacheFile).ConfigureAwait(false))
            {
                await StorageHelper.DeleteFileAsync(TokenCacheFile).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Stores the OAuth token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A Task for async execution
        /// </returns>
        private async Task StoreOAuthToken(AuthToken token, string clientSecret, CancellationToken? cancellationToken = null)
        {
            var tokenString = token.ToString();
#if NETFX_CORE
            // Encrypt to stop prying eyes on Win8
            await StorageHelper.WriteTextAsync(TokenCacheFile, EncryptionHelper.Encrypt(tokenString, clientSecret, this._mixRadioClient.ClientId)).ConfigureAwait(false);
#else
            await StorageHelper.WriteTextAsync(TokenCacheFile, tokenString).ConfigureAwait(false);
#endif
        }
    }
}
#endif
