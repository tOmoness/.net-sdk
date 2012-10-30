// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using Microsoft.Phone.Controls;
using Nokia.Music.Phone;
using Nokia.Music.Phone.Tasks;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Main Page
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
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

        private void ShowSearchTask(object sender, RoutedEventArgs e)
        {
            MusicSearchTask task = new MusicSearchTask();
            task.SearchTerms = "Muse";
            task.Show();
        }

        private void ShowArtistTask(object sender, RoutedEventArgs e)
        {
            ShowArtistTask task = new ShowArtistTask();
            task.ArtistName = "Lady Gaga";
            task.Show();
        }

        private void PlayArtistTask(object sender, RoutedEventArgs e)
        {
            PlayMixTask task = new PlayMixTask();
            task.ArtistName = "Coldplay";
            task.Show();
        }

        private void ShowGigsTask(object sender, RoutedEventArgs e)
        {
            ShowGigsTask task = new ShowGigsTask();
            task.Show();
        }

        private void SearchGigsTask(object sender, RoutedEventArgs e)
        {
            ShowGigsTask task = new ShowGigsTask();
            task.SearchTerms = "New York";
            task.Show();
        }

        private void ValidateDeviceCountry(object sender, RoutedEventArgs e)
        {
            this.ValidateDeviceCountryButton.IsEnabled = false;

            string countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower();
            CountryResolver resolver = new CountryResolver(ApiKeys.AppId, ApiKeys.AppCode);
            resolver.CheckAvailability(
                (Response<bool> response) =>
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            if (response.Result)
                            {
                                MessageBox.Show("Hooray! Nokia Music is available in " + RegionInfo.CurrentRegion.DisplayName + "!");
                            }
                            else
                            {
                                if (response.Error != null)
                                {
                                    MessageBox.Show(response.Error.Message);
                                }
                                else
                                {
                                    MessageBox.Show("Sorry, Nokia Music is not available in your region - you won't be able to use the API features.");
                                }

                                countryCode = null;
                            }

                            EnableCountrySpecificApiButtons(countryCode);
                            App.SaveCountryCode(countryCode);
                            this.ValidateDeviceCountryButton.IsEnabled = true;
                        });
                },
                countryCode);
        }

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
        }

        private void SearchArtists(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml?" + SearchPage.SearchScopeParam + "=" + SearchPage.SearchScopeArtists, UriKind.Relative));
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void GetTopArtists(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + ShowListPage.MethodCall.GetTopArtists, UriKind.Relative));
        }

        private void GetGenres(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + ShowListPage.MethodCall.GetGenres, UriKind.Relative));
        }

        private void GetTopAlbums(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + ShowListPage.MethodCall.GetTopAlbums, UriKind.Relative));
        }

        private void GetNewAlbums(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + ShowListPage.MethodCall.GetNewAlbums, UriKind.Relative));
        }

        private void GetMixGroups(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + ShowListPage.MethodCall.GetMixGroups, UriKind.Relative));
        }

        private void ShowLibraryRecommendations(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LibraryRecommendations.xaml", UriKind.Relative));
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}