// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;
using Nokia.Music.Tasks;
using Nokia.Music.Types;
using Windows.Security.Authentication.Web;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Nokia.Music.TestApp
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
#endif
        }
#if WINDOWS_PHONE_APP

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            e.Handled = true;

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
            else
            {
                App.Current.Exit();
            }
        }
#endif

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.  The Parameter
        ///     property is typically used to configure the page.
        /// </param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DeviceRegionCode.Text = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLowerInvariant();
            if (App.ApiClient != null)
            {
                this.EnableCountrySpecificApiButtons(App.GetSettingsCountryCode());
            }
            else
            {
                this.EnableCountrySpecificApiButtons(null);
            }

            // Attempt silent token refresh...
            bool userLoggedIn = false;
            if (App.ApiClient != null)
            {
                userLoggedIn = App.ApiClient.IsUserAuthenticated;
                if (!userLoggedIn && await App.ApiClient.IsUserTokenCached())
                {
                    try
                    {
                        await App.ApiClient.AuthenticateUserAsync(ApiKeys.ClientSecret);
                        userLoggedIn = App.ApiClient != null && App.ApiClient.IsUserAuthenticated;
                    }
                    catch
                    {
                        // Ignore for now
                    }
                }
            }

            this.SetAuthPanelVisibility();
        }

#if WINDOWS_PHONE_APP
        internal async Task CompleteUserAuthAsync(WebAuthenticationResult webAuthenticationResult)
        {
            this.AuthFinalising.Visibility = Visibility.Visible;

            string message = null;
            try
            {
                var result = await App.ApiClient.CompleteAuthenticateUserAsync(ApiKeys.ClientSecret, webAuthenticationResult);
                if (result != AuthResultCode.Success && result != AuthResultCode.InProgress)
                {
                    message = "User auth failed: " + result.ToString();
                }
                else
                {
                    // no message required
                    message = null;
                }
            }
            catch (Exception ex)
            {
                message = "User auth failed: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(message))
            {
                await MessageBox.Show(message);
            }

            this.SetAuthPanelVisibility();

            this.AuthFinalising.Visibility = Visibility.Collapsed;
        }

#endif
        /// <summary>
        /// Launches search task.
        /// </summary>
        /// <param name="sender">Search Task button</param>
        /// <param name="e">Event arguments</param>
        private async void ShowSearchTask(object sender, RoutedEventArgs e)
        {
            MusicSearchTask task = new MusicSearchTask();
            task.SearchTerms = "Muse";
            await task.Show();
        }

        /// <summary>
        /// Launches show artist task.
        /// </summary>
        /// <param name="sender">Show Artist Task button</param>
        /// <param name="e">Event arguments</param>
        private async void ShowArtistTask(object sender, RoutedEventArgs e)
        {
            ShowArtistTask task = new ShowArtistTask();
            task.ArtistName = "Lady Gaga";
            await task.Show();
        }

        /// <summary>
        /// Launches play mix task.
        /// </summary>
        /// <param name="sender">Play Mix Task button</param>
        /// <param name="e">Event arguments</param>
        private async void PlayMixTask(object sender, RoutedEventArgs e)
        {
            PlayMixTask task = new PlayMixTask();
            task.ArtistName = "Coldplay";
            await task.Show();
        }

        /// <summary>
        /// Validates the country code got from phone region settings.
        /// </summary>
        /// <param name="sender">Validate Device Country button</param>
        /// <param name="e">Event arguments</param>
        private async void ValidateDeviceCountry(object sender, RoutedEventArgs e)
        {
            this.ValidateDeviceCountryButton.IsEnabled = false;

            string countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower();
            CountryResolver resolver = new CountryResolver(ApiKeys.ClientId);
            string message = null;

            try
            {
                bool available = await resolver.CheckAvailabilityAsync(countryCode);
                if (available)
                {
                    message = "Hooray! MixRadio is available in " + RegionInfo.CurrentRegion.DisplayName + "!";
                }
                else
                {
                    message = "Sorry, MixRadio is not available in your region - you won't be able to use the API features.";
                    countryCode = null;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                countryCode = null;
            }

            await MessageBox.Show(message);

            this.EnableCountrySpecificApiButtons(countryCode);
            App.SaveCountryCode(countryCode);
            this.ValidateDeviceCountryButton.IsEnabled = true;
        }

        /// <summary>
        /// Clears the country code validated earlier.
        /// </summary>
        /// <param name="sender">Reset Country button</param>
        /// <param name="e">Event arguments</param>
        private void ClearDeviceCountry(object sender, RoutedEventArgs e)
        {
            this.EnableCountrySpecificApiButtons(null);
            App.SaveCountryCode(null);
        }

        /// <summary>
        /// Enables Country-specific APIs
        /// </summary>
        /// <param name="countryCode">The Country Code</param>
        private void EnableCountrySpecificApiButtons(string countryCode)
        {
            // Show / Hide the descriptions...
            this.ValidatedPanel.Visibility = countryCode != null ? Visibility.Visible : Visibility.Collapsed;
            this.NotValidatedPanel.Visibility = countryCode == null ? Visibility.Visible : Visibility.Collapsed;

            // Set the country in use...
            if (string.IsNullOrEmpty(countryCode))
            {
                this.CountryCodeInUse.Text = string.Empty;
            }
            else
            {
                this.CountryCodeInUse.Text = countryCode;
            }

            // enable / disable API buttons...
#if WINDOWS_PHONE_APP
            this.SearchButton.IsEnabled = countryCode != null;
            this.SearchArtistsButton.IsEnabled = countryCode != null;
#endif
            this.TopArtistsButton.IsEnabled = countryCode != null;
            this.GenresButton.IsEnabled = countryCode != null;
            this.TopAlbumsButton.IsEnabled = countryCode != null;
            this.NewAlbumsButton.IsEnabled = countryCode != null;
            this.MixGroupsButton.IsEnabled = countryCode != null;
            this.AuthUserButton.IsEnabled = countryCode != null;
        }

        /// <summary>
        /// Navigates to ShowListPage in top artist mode.
        /// </summary>
        /// <param name="sender">Top Artists button</param>
        /// <param name="e">Event arguments</param>
        private void GetTopArtists(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetTopArtists));
        }

        /// <summary>
        /// Navigates to ShowListPage in genres mode.
        /// </summary>
        /// <param name="sender">Genres button</param>
        /// <param name="e">Event arguments</param>
        private void GetGenres(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetGenres));
        }

        /// <summary>
        /// Navigates to ShowListPage in top albums mode.
        /// </summary>
        /// <param name="sender">Top Albums button</param>
        /// <param name="e">Event arguments</param>
        private void GetTopAlbums(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetTopAlbums));
        }

        /// <summary>
        /// Navigates to ShowListPage in new albums mode.
        /// </summary>
        /// <param name="sender">New Albums button</param>
        /// <param name="e">Event arguments</param>
        private void GetNewAlbums(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetNewAlbums));
        }

        /// <summary>
        /// Navigates to ShowListPage in mix group mode.
        /// </summary>
        /// <param name="sender">Get Mix Groups button</param>
        /// <param name="e">Event arguments</param>
        private void GetMixGroups(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetMixGroups));
        }

        /// <summary>
        /// Attempts to authenticate the user.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void UserAuth(object sender, RoutedEventArgs e)
        {
            string message = null;
            try
            {
                var result = await App.ApiClient.AuthenticateUserAsync(ApiKeys.ClientSecret, Scope.ReadUserPlayHistory, ApiKeys.OAuthRedirectUri);
                if (result != AuthResultCode.Success && result != AuthResultCode.InProgress)
                {
                    message = "User auth failed: " + result.ToString();
                }
                else
                {
                    // no message required
                    message = null;
                }
            }
            catch (Exception ex)
            {
                message = "User auth failed: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(message))
            {
                await MessageBox.Show(message);
            }

            this.SetAuthPanelVisibility();
        }

        private void SetAuthPanelVisibility()
        {
            bool userLoggedIn = App.ApiClient != null && App.ApiClient.IsUserAuthenticated;

            this.AuthedPanel.Visibility = userLoggedIn ? Visibility.Visible : Visibility.Collapsed;
            this.NotAuthedPanel.Visibility = userLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Shows the OAuth docs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void ShowAuthUserDocs(object sender, RoutedEventArgs e)
        {
            Uri link = new Uri((sender as Button).Tag.ToString());
            await Launcher.LaunchUriAsync(link);
        }

        /// <summary>
        /// Navigates to ShowListPage in user history mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments</param>
        private void UserPlayHistory(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetUserHistory));
        }

        /// <summary>
        /// Navigates to ShowListPage in user chart mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserChart(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetUserTopArtists));
        }

        private void UserRecentMixes(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetUserRecentMixes));
        }

        private async void LaunchApp(object sender, RoutedEventArgs e)
        {
            await new LaunchTask().Show();
        }
#if WINDOWS_PHONE_APP

        private void SearchArtists(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage), SearchPage.SearchScopeArtists);
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private async void ShowGigsTask(object sender, RoutedEventArgs e)
        {
            await new ShowGigsTask().Show();
        }

        private async void SearchGigsTask(object sender, RoutedEventArgs e)
        {
            await new ShowGigsTask { SearchTerms = "New York" }.Show();
        }

        private async void PlayMeTask(object sender, RoutedEventArgs e)
        {
            await new PlayMeTask().Show();
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
#endif
    }
}