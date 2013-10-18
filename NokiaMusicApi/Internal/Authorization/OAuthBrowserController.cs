// -----------------------------------------------------------------------
// <copyright file="OAuthBrowserController.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Nokia.Music.Internal.Authorization
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
#if WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
#endif
    using Nokia.Music.Types;

    /// <summary>
    /// Wraps browser interaction for OAuth flows
    /// </summary>
    internal sealed class OAuthBrowserController
    {
        private WebBrowser _browser = null;
        private ManualResetEventSlim _authWaiter;

        // Properties derived by this flow...
        internal string AuthorizationCode { get; private set; }
        internal AuthResultCode ResultCode { get; private set; }
        internal bool IsBusy { get; private set; }

        /// <summary>
        /// Drives the Authentication process.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="startUri">The start URI.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A boolean indicating the result
        /// </returns>
        internal void DriveAuthProcess(WebBrowser browser, Uri startUri, CancellationToken? cancellationToken)
        {
            if (this.IsBusy)
            {
                throw new InvalidOperationException("An authentication call is in progress already");
            }

            if (cancellationToken != null && cancellationToken.HasValue)
            {
                cancellationToken.Value.Register(() =>
                {
                    if (this.ResultCode == AuthResultCode.Unknown)
                    {
                        this.ResultCode = AuthResultCode.Cancelled;
                        this._authWaiter.Set();
                    }
                });
            }

            this._browser = browser;
            this._authWaiter = null;

            this.AuthorizationCode = null;
            this.ResultCode = AuthResultCode.Unknown;
            this.IsBusy = true;

            // Ensure JavaScript is enabled...
            this._browser.Dispatcher.BeginInvoke(() => { this._browser.IsScriptEnabled = true; });

            // hook events
            this._browser.Navigating += this.Browser_Navigating;
            this._browser.NavigationFailed += this.Browser_NavigationFailed;

            this._authWaiter = new ManualResetEventSlim(false);
            this._browser.Dispatcher.BeginInvoke(() => { this._browser.Navigate(startUri); });
            this._authWaiter.Wait();

            // Once we get here, the user will have signed in, allowed or cancelled authorization...
            this._authWaiter = null;
            this.IsBusy = false;

            // unhook events
            this._browser.Navigating -= this.Browser_Navigating;
            this._browser.NavigationFailed -= this.Browser_NavigationFailed;

            this._browser.Dispatcher.BeginInvoke(() => { this._browser.Navigate(new Uri("about:blank")); });
        }

        /// <summary>
        /// Handles the Navigating event of the Browser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event args instance containing the event data.</param>
#if WINDOWS_PHONE
        private void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            string query = e.Uri.Query;
#endif
#if NET40
        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string query = e.Url.Query;
#endif
            if (!string.IsNullOrEmpty(query))
            {
                AuthResultCode result = AuthResultCode.Unknown;
                string authorizationCode = null;

                if (OAuthResultParser.ParseQuerystringForCompletedFlags(query, out result, out authorizationCode))
                {
                    if (result == AuthResultCode.Success)
                    {
                        this.AuthorizationCode = authorizationCode;
                    }
                    
                    this.ResultCode = result;
                    e.Cancel = true;
                    this._authWaiter.Set();
                }
            }
        }

        /// <summary>
        /// Handles the NavigationFailed event of the Browser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationFailedEventArgs"/> instance containing the event data.</param>
        private void Browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (this.ResultCode == AuthResultCode.Unknown)
            {
                WebBrowserNavigationException navException = e.Exception as WebBrowserNavigationException;
                if (navException != null && navException.StatusCode == HttpStatusCode.Unauthorized)
                {
                    this.ResultCode = AuthResultCode.UnauthorizedClient;
                }
                else
                {
                    this.ResultCode = AuthResultCode.ServerError;
                }
                this._authWaiter.Set();
            }
        }
    }
}
