// -----------------------------------------------------------------------
// <copyright file="OAuthUserFlow.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
#if NETFX_CORE
using System.Net;
#endif
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif
using MixRadio.AuthHelpers;
using Nokia.Music.Commands;
using Nokia.Music.Types;
#if NETFX_CORE
using Windows.Security.Authentication.Web;
#endif

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Class that handles user OAuth flow.
    /// </summary>
    internal class OAuthUserFlow
    {
#if WINDOWS_PHONE
        private OAuthBrowserController _browserController;

#endif
        private string _clientId;
        private string _clientSecret;
        private string _secureBaseApiUri;
        private GetAuthTokenCommand _tokenCommand;

        /// <summary>
        /// Initialises the OAuthUserFlow class
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="secureBaseApiUri">The secure base API URI.</param>
        /// <param name="command">The command.</param>
        public OAuthUserFlow(string clientId, string clientSecret, string secureBaseApiUri, GetAuthTokenCommand command)
        {
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._secureBaseApiUri = secureBaseApiUri;
            this._tokenCommand = command;
            this.TokenResponse = null;
        }

        internal bool TokenCallInProgress { get; set; }

        internal bool IsBusy
        {
            get
            {
#if WINDOWS_PHONE
                return (this._browserController != null && this._browserController.IsBusy) || this.TokenCallInProgress;
#elif NETFX_CORE
                return this.TokenCallInProgress;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets the token response.
        /// </summary>
        /// <value>
        /// The token response.
        /// </value>
        internal TokenResponse TokenResponse { get; private set; }

#if WINDOWS_PHONE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="startUri">The auth uri.</param>
        /// <param name="browser">The browser control to use to drive authentication.</param>
        /// <param name="oauthRedirectUri">The OAuth completed URI.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>
        /// An async task
        /// </returns>
        public async Task<Response<AuthResultCode>> AuthenticateUserAsync(Uri startUri, WebBrowser browser, string oauthRedirectUri, CancellationToken? cancellationToken = null)
        {
            if (this._browserController == null)
            {
                this._browserController = new OAuthBrowserController();
            }

            CancellationToken token = cancellationToken ?? CancellationToken.None;

            await Task.Run(() => this._browserController.DriveAuthProcess(browser, startUri, oauthRedirectUri, token), token);
            return await this.ConvertAuthPermissionParamsAndFinalise(this._browserController.ResultCode != AuthResultCode.Cancelled, token);
        }
#endif

#if WINDOWS_APP
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="startUri">The auth uri.</param>
        /// <param name="oauthCompletedUri">The OAuth completed URI.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        public async Task<Response<AuthResultCode>> AuthenticateUserAsync(Uri startUri, Uri oauthCompletedUri, CancellationToken? cancellationToken = null)
        {
            var brokerResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, oauthCompletedUri);
            return await this.ConvertAuthPermissionParams(brokerResult);
        }

#endif

#if WINDOWS_PHONE
        /// <summary>
        /// Continues the authenticate user flow by extracting results from the WP8-specific
        /// auth method and moving on to the final common step.
        /// </summary>
        /// <param name="taskCompleted">if set to <c>true</c> oauth task completed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Whether the token was retrieved
        /// </returns>
        internal async Task<Response<AuthResultCode>> ConvertAuthPermissionParamsAndFinalise(bool taskCompleted, CancellationToken? cancellationToken)
        {
            if (taskCompleted)
            {
                // Grab the results and kill the browser controller
                string authorizationCode = this._browserController.AuthorizationCode;
                AuthResultCode resultCode = this._browserController.ResultCode;
                this._browserController = null;

                // Move on to obtain a token
                return await this.ObtainToken(authorizationCode, null, resultCode);
            }
            else
            {
                return new Response<AuthResultCode>(null, new OperationCanceledException(), null, Guid.Empty);
            }
        }

#elif NETFX_CORE
        /// <summary>
        /// Continues the authenticate user flow by extracting results from the Windows-specific
        /// auth method and moving on to the final common step.
        /// </summary>
        /// <param name="authResult">The result from the WebAuthenticationBroker call.</param>
        /// <returns>
        /// Whether the token was retrieved
        /// </returns>
        internal async Task<Response<AuthResultCode>> ConvertAuthPermissionParams(WebAuthenticationResult authResult)
        {
            try
            {
                switch (authResult.ResponseStatus)
                {
                    case WebAuthenticationStatus.Success:

                        // ResponseData will give us the final URI with a Querystring containing an auth code or error details
                        // e.g. code=90e754fd-0a4a-4eb5-b5f6-25dad9face6a or error=access_denied
                        AuthResultCode resultCode = AuthResultCode.Unknown;
                        string authorizationCode = null;

                        OAuthResultParser.ParseQuerystringForCompletedFlags(authResult.ResponseData, out resultCode, out authorizationCode);
                        if (resultCode != AuthResultCode.Unknown)
                        {
                            // Move on to obtain a token
                            return await this.ObtainToken(authorizationCode, null, resultCode);
                        }
                        else
                        {
                            return new Response<AuthResultCode>(null, AuthResultCode.Unknown, Guid.Empty);
                        }

                    case WebAuthenticationStatus.ErrorHttp:
                        switch ((HttpStatusCode)authResult.ResponseErrorDetail)
                        {
                            case HttpStatusCode.BadRequest:
                                return new Response<AuthResultCode>(null, AuthResultCode.InvalidScope, Guid.Empty);

                            case HttpStatusCode.Unauthorized:
                                return new Response<AuthResultCode>(null, AuthResultCode.UnauthorizedClient, Guid.Empty);

                            case HttpStatusCode.InternalServerError:
                                return new Response<AuthResultCode>(null, AuthResultCode.ServerError, Guid.Empty);
                        }

                        // Any other items will return as cancelled below...
                        break;
                }
            }
            catch
            {
                // Usually means we got cancelled
            }

            return new Response<AuthResultCode>(null, AuthResultCode.Cancelled, Guid.Empty);
        }
#endif
#if WINDOWS_APP || WINDOWS_PHONE || WINDOWS_PHONE_APP || PORTABLE

        /// <summary>
        /// Finalises authorisation to obtain a token
        /// </summary>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="resultCode">The result code for the process so far.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// Whether the token was retrieved
        /// </returns>
        internal async Task<Response<AuthResultCode>> ObtainToken(string authorizationCode, string refreshToken, AuthResultCode resultCode, CancellationToken? cancellationToken = null)
        {
            // Next get a token, for now just return whether we got the authorization code...
            if (!string.IsNullOrEmpty(authorizationCode) || !string.IsNullOrEmpty(refreshToken))
            {
                Response<AuthResultCode> result = null;

                this.TokenCallInProgress = true;

                // Set up auth code and secret...
                this._tokenCommand.AuthorizationCode = authorizationCode;
                this._tokenCommand.ClientId = this._clientId;
                this._tokenCommand.ClientSecret = this._clientSecret;
                this._tokenCommand.RefreshToken = refreshToken;

                try
                {
                    var tokenResponse = await this._tokenCommand.ExecuteAsync(cancellationToken);
                    if (tokenResponse.Result != null)
                    {
                        result = new Response<AuthResultCode>(null, AuthResultCode.Success, Guid.Empty);
                        this.TokenResponse = tokenResponse.Result;
                    }
                }
                catch
                {
                    result = new Response<AuthResultCode>(null, AuthResultCode.Unknown, Guid.Empty);
                }

                this.TokenCallInProgress = false;

                return result;
            }
            else
            {
                return new Response<AuthResultCode>(null, resultCode, Guid.Empty);
            }
        }
#endif
    }
}
