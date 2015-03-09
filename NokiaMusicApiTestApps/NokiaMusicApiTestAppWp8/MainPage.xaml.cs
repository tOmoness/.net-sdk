﻿// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Microsoft Mobile. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Microsoft Mobile. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Nokia.Music;
using Nokia.Music.Tasks;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Main Page
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor for MainPage.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.PhoneRegionCode.Text = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLowerInvariant();
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
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">An object that contains the event data.</param>
        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Attempt silent token refresh...
            bool userLoggedIn = false;
            if (App.ApiClient != null)
            {
                userLoggedIn = App.ApiClient.IsUserAuthenticated;
                if (!userLoggedIn && await App.AuthHelper.IsUserTokenCached())
                {
                    try
                    {
                        await App.AuthHelper.AuthenticateUserAsync(ApiKeys.ClientSecret);
                        userLoggedIn = App.ApiClient != null && App.ApiClient.IsUserAuthenticated;
                    }
                    catch
                    {
                        // Ignore as this is a silent token refresh
                    }
                }
            }

            this.AuthedPanel.Visibility = userLoggedIn ? Visibility.Visible : Visibility.Collapsed;
            this.NotAuthedPanel.Visibility = userLoggedIn ? Visibility.Collapsed : Visibility.Visible;
        }

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
        /// Launches show gigs task.
        /// </summary>
        /// <param name="sender">Show Gigs Task button</param>
        /// <param name="e">Event arguments</param>
        private async void ShowGigsTask(object sender, RoutedEventArgs e)
        {
            await new ShowGigsTask().Show();
        }

        /// <summary>
        /// Launches show gigs task with a search term.
        /// </summary>
        /// <param name="sender">Search Gigs Task button</param>
        /// <param name="e">Event arguments</param>
        private async void SearchGigsTask(object sender, RoutedEventArgs e)
        {
            await new ShowGigsTask { SearchTerms = "New York" }.Show();
        }

        private async void PlayMeTask(object sender, RoutedEventArgs e)
        {
            await new PlayMeTask().Show();
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

            MessageBox.Show(message);

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
            this.SearchArtistsButton.IsEnabled = countryCode != null;
            this.SearchButton.IsEnabled = countryCode != null;
            this.TopArtistsButton.IsEnabled = countryCode != null;
            this.GenresButton.IsEnabled = countryCode != null;
            this.TopAlbumsButton.IsEnabled = countryCode != null;
            this.NewAlbumsButton.IsEnabled = countryCode != null;
            this.MixGroupsButton.IsEnabled = countryCode != null;
            this.LibraryRecommendationsButton.IsEnabled = countryCode != null;
            this.AuthUserButton.IsEnabled = countryCode != null;
        }

        /// <summary>
        /// Navigates to SearchPage in artist search scope.
        /// </summary>
        /// <param name="sender">Search Artists button</param>
        /// <param name="e">Event arguments</param>
        private void SearchArtists(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml?" + SearchPage.SearchScopeParam + "=" + SearchPage.SearchScopeArtists, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to SearchPage.
        /// </summary>
        /// <param name="sender">Search button</param>
        /// <param name="e">Event arguments</param>
        private void Search(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in top artist mode.
        /// </summary>
        /// <param name="sender">Top Artists button</param>
        /// <param name="e">Event arguments</param>
        private void GetTopArtists(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetTopArtists, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in genres mode.
        /// </summary>
        /// <param name="sender">Genres button</param>
        /// <param name="e">Event arguments</param>
        private void GetGenres(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetGenres, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in top albums mode.
        /// </summary>
        /// <param name="sender">Top Albums button</param>
        /// <param name="e">Event arguments</param>
        private void GetTopAlbums(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetTopAlbums, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in new albums mode.
        /// </summary>
        /// <param name="sender">New Albums button</param>
        /// <param name="e">Event arguments</param>
        private void GetNewAlbums(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetNewAlbums, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in mix group mode.
        /// </summary>
        /// <param name="sender">Get Mix Groups button</param>
        /// <param name="e">Event arguments</param>
        private void GetMixGroups(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetMixGroups, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to LibraryRecommendations page.
        /// </summary>
        /// <param name="sender">Get Recommendations button</param>
        /// <param name="e">Event arguments</param>
        private void ShowLibraryRecommendations(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LibraryRecommendations.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the NokiaMusicAuthPage page
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserAuth(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NokiaMusicAuthPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to AboutPage.
        /// </summary>
        /// <param name="sender">Get Recommendations button</param>
        /// <param name="e">Event arguments</param>
        private void ShowAbout(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Shows the OAuth docs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowAuthUserDocs(object sender, RoutedEventArgs e)
        {
            Uri link = new Uri((sender as Button).Tag.ToString());
            WebBrowserTask browser = new WebBrowserTask() { Uri = link };
            browser.Show();
        }

        /// <summary>
        /// Navigates to ShowListPage in user history mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments</param>
        private void UserPlayHistory(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetUserHistory, UriKind.Relative));
        }

        /// <summary>
        /// Navigates to ShowListPage in user chart mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserChart(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetUserTopArtists, UriKind.Relative));
        }

        private void UserRecentMixes(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetUserRecentMixes, UriKind.Relative));
        }
    }
}