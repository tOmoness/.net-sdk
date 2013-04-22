// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Nokia.Music.Tasks;
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
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.  The Parameter
        ///     property is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
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
        }

        /// <summary>
        /// Launches search task.
        /// </summary>
        /// <param name="sender">Search Task button</param>
        /// <param name="e">Event arguments</param>
        private void ShowSearchTask(object sender, RoutedEventArgs e)
        {
            MusicSearchTask task = new MusicSearchTask();
            task.SearchTerms = "Muse";
            task.Show();
        }

        /// <summary>
        /// Launches show artist task.
        /// </summary>
        /// <param name="sender">Show Artist Task button</param>
        /// <param name="e">Event arguments</param>
        private void ShowArtistTask(object sender, RoutedEventArgs e)
        {
            ShowArtistTask task = new ShowArtistTask();
            task.ArtistName = "Lady Gaga";
            task.Show();
        }

        /// <summary>
        /// Launches play mix task.
        /// </summary>
        /// <param name="sender">Play Mix Task button</param>
        /// <param name="e">Event arguments</param>
        private void PlayMixTask(object sender, RoutedEventArgs e)
        {
            PlayMixTask task = new PlayMixTask();
            task.ArtistName = "Coldplay";
            task.Show();
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
            CountryResolver resolver = new CountryResolver(ApiKeys.AppId);
            Response<bool> response = await resolver.CheckAvailabilityAsync(countryCode);

            if (response.Result)
            {
                await MessageBox.Show("Hooray! Nokia Music is available in " + RegionInfo.CurrentRegion.DisplayName + "!");
            }
            else
            {
                if (response.Error != null)
                {
                    await MessageBox.Show(response.Error.Message);
                }
                else
                {
                    await MessageBox.Show("Sorry, Nokia Music is not available in your region - you won't be able to use the API features.");
                }

                countryCode = null;
            }

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

            // enable / disable API test buttons...
            this.TopArtistsButton.IsEnabled = countryCode != null;
            this.GenresButton.IsEnabled = countryCode != null;
            this.TopAlbumsButton.IsEnabled = countryCode != null;
            this.NewAlbumsButton.IsEnabled = countryCode != null;
            this.MixGroupsButton.IsEnabled = countryCode != null;
        }

        /// <summary>
        /// Navigates to SearchPage in artist search scope.
        /// </summary>
        /// <param name="sender">Search Artists button</param>
        /// <param name="e">Event arguments</param>
        private void SearchArtists(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/SearchPage.xaml?" + SearchPage.SearchScopeParam + "=" + SearchPage.SearchScopeArtists, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to SearchPage.
        /// </summary>
        /// <param name="sender">Search button</param>
        /// <param name="e">Event arguments</param>
        private void Search(object sender, RoutedEventArgs e)
        {
            ////NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
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

        private void LaunchApp(object sender, RoutedEventArgs e)
        {
            new LaunchTask().Show();
        }
    }
}