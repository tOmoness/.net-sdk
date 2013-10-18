// -----------------------------------------------------------------------
// <copyright file="NokiaMusicAuthPage.xaml.cs" company="Nokia">
// Copyright © 2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The NokiaMusicAuthPage allows user authentication for access to their data
    /// </summary>
    public partial class NokiaMusicAuthPage : PhoneApplicationPage
    {
        private const Scope ScopeRequired = Scope.ReadUserPlayHistory;

        private static DependencyProperty noticeHeight = DependencyProperty.Register("NoticeHeight", typeof(double), typeof(NokiaMusicAuthPage), new PropertyMetadata(116.0));
        private CancellationTokenSource _authenticateUserCancellationSource = null;

        public NokiaMusicAuthPage()
        {
            this.InitializeComponent();
        }

        public DependencyProperty NoticeHeight
        {
            get
            {
                return noticeHeight;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.PerformUserAuth();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Signal that we want to end the auth flow...
            if (this._authenticateUserCancellationSource != null)
            {
                this._authenticateUserCancellationSource.Cancel(true);
            }
        }

        private async void PerformUserAuth()
        {
            if (App.ApiClient.IsUserAuthenticated)
            {
                this.LeavePage();
                return;
            }

            this._authenticateUserCancellationSource = new CancellationTokenSource();

            string message = null;
            try
            {
                var result = await App.ApiClient.AuthenticateUserAsync(ApiKeys.ClientSecret, Scope.ReadUserPlayHistory, this.AuthBrowser, this._authenticateUserCancellationSource.Token);
                if (result != AuthResultCode.Success)
                {
                    message = "User auth failed: " + result.ToString();
                }
                else
                {
                    message = "User auth successful!";
                }
            }
            catch (OperationCanceledException)
            {
                message = "Cancelled";
            }
            catch (Exception ex)
            {
                message = "User auth failed: " + ex.Message;
            }
            
            MessageBox.Show(message);

            this.AuthBrowser.Visibility = Visibility.Collapsed;
            this.FinalStep.Visibility = Visibility.Visible;

            this._authenticateUserCancellationSource = null;

            this.LeavePage();
        }

        private void LeavePage()
        {
            if (this.NavigationService.CanGoBack)
            {
                try
                {
                    this.NavigationService.GoBack();
                }
                catch
                {
                    // Sometimes going back causes exceptions
                }
            }
        }

        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Uri.ToString()))
            {
                System.Diagnostics.Debug.WriteLine("BrowserNavigated " + e.Uri.ToString());

                if (e.Uri.Scheme.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.TitlePanel.Visibility = Visibility.Visible;
                    this.AuthBrowser.Visibility = Visibility.Visible;
                    if ((double)this.GetValue(noticeHeight) > 100)
                    {
                        this.NoticeHide.Begin();
                    }
                }
                else
                {
                    // Final step in the MusicClient is now happening...
                    this.AuthBrowser.Visibility = Visibility.Collapsed;
                    this.FinalStep.Visibility = Visibility.Visible;
                }
            }
        }
    }
}