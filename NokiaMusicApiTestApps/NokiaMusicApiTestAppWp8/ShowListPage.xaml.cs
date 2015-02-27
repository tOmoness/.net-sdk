// -----------------------------------------------------------------------
// <copyright file="ShowListPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Microsoft Mobile. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Microsoft Mobile. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The List Page
    /// </summary>
    public partial class ShowListPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constant for method parameter.
        /// </summary>
        public const string MethodParam = "method";

        private MethodCall _method = MethodCall.Unknown;

        /// <summary>
        /// Constructor for ShowListPage.
        /// </summary>
        public ShowListPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Plays a sample clip for the track id specified.
        /// </summary>
        /// <param name="id">The id.</param>
        internal void PlayClip(string id)
        {
            this.Player.Stop();
            if (this.Player.CurrentState != System.Windows.Media.MediaElementState.Playing)
            {
                this.Player.Source = App.ApiClient.GetTrackSampleUri(id);
                this.Player.Play();
            }
        }

        /// <summary>
        /// Initialized the page and makes a request to MixRadio API 
        /// based on provided method call parameter.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!NavigationContext.QueryString.ContainsKey(MethodParam))
            {
                MessageBox.Show("The method querystring is missing");
                return;
            }

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                this.Loading.Visibility = Visibility.Visible;
                this.TotalResults.Visibility = Visibility.Collapsed;

                this._method = (MethodCall)Enum.Parse(typeof(MethodCall), NavigationContext.QueryString[MethodParam], true);
                switch (this._method)
                {
                    case MethodCall.GetTopArtists:
                        this.PageTitle.Text = "top artists";
                        this.ResponseHandler(await App.ApiClient.GetTopArtistsAsync(0, 20));
                        break;

                    case MethodCall.GetGenres:
                        this.PageTitle.Text = "genres";
                        this.ResponseHandler(await App.ApiClient.GetGenresAsync());
                        break;

                    case MethodCall.GetTopArtistsForGenre:
                        if (!NavigationContext.QueryString.ContainsKey(App.IdParam))
                        {
                            MessageBox.Show("The id querystring is missing");
                            return;
                        }

                        string genreId = NavigationContext.QueryString[App.IdParam];
                        this.PageTitle.Text = "top artists for " + genreId.ToLowerInvariant();
                        this.ResponseHandler(await App.ApiClient.GetTopArtistsForGenreAsync(genreId, 0, 20));
                        break;

                    case MethodCall.GetMixGroups:
                        this.PageTitle.Text = "mix groups";
                        this.ResponseHandler(await App.ApiClient.GetMixGroupsAsync(0, 100));
                        break;

                    case MethodCall.GetMixes:
                        if (!NavigationContext.QueryString.ContainsKey(App.IdParam) || !NavigationContext.QueryString.ContainsKey(App.NameParam))
                        {
                            MessageBox.Show("The querystring is incomplete");
                            return;
                        }

                        string mixId = NavigationContext.QueryString[App.IdParam];
                        this.PageTitle.Text = HttpUtility.UrlDecode(NavigationContext.QueryString[App.NameParam]);
                        this.ResponseHandler(await App.ApiClient.GetMixesAsync(mixId));
                        break;

                    case MethodCall.GetTopAlbums:
                        this.PageTitle.Text = "top albums";
                        this.ResponseHandler(await App.ApiClient.GetTopProductsAsync(Category.Album, 0, 20));
                        break;

                    case MethodCall.GetNewAlbums:
                        this.PageTitle.Text = "new albums";
                        this.ResponseHandler(await App.ApiClient.GetNewReleasesAsync(Category.Album, 0, 20));
                        break;

                    case MethodCall.GetUserHistory:
                        this.PageTitle.Text = "play history";
                        this.ResponseHandler(await App.ApiClient.GetUserPlayHistoryAsync(UserEventAction.Complete, 0, 100));
                        break;

                    case MethodCall.GetUserTopArtists:
                        this.PageTitle.Text = "top artists of week";
                        this.ResponseHandler(await App.ApiClient.GetUserTopArtistsAsync());
                        break;

                    case MethodCall.GetUserRecentMixes:
                        this.PageTitle.Text = "recent mixes";
                        this.ResponseHandler(await App.ApiClient.GetUserRecentMixesAsync(10));
                        break;

                    default:
                        MessageBox.Show("Unhandled method " + this._method.ToString());
                        return;
                }
            }
        }

        /// <summary>
        /// Populates list box with search results.
        /// </summary>
        /// <param name="response">Search results from MixRadio API</param>
        /// <typeparam name="T">Any MusicItem from MixRadio API</typeparam>
        private void ResponseHandler<T>(ListResponse<T> response)
        {
            Dispatcher.BeginInvoke(async () =>
            {
                this.Loading.Visibility = Visibility.Collapsed;
                this.Results.ItemsSource = response.Result;

                if (response.Result != null)
                {
                    if (response.Result.Count == 0)
                    {
                        this.TotalResults.Visibility = Visibility.Collapsed;
                        MessageBox.Show(@"No results found");
                        this.LeavePage();
                    }
                    else
                    {
                        if (response.TotalResults != null && response.TotalResults.Value != response.Count)
                        {
                            this.TotalResults.Text = string.Format("{0:#,###0} item(s) available", response.TotalResults.Value);
                            this.TotalResults.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.TotalResults.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else if (response.Error != null)
                {
#pragma warning disable 0618  // Disable this for now
                    await App.ApiClient.DeleteAuthenticationTokenAsync();
#pragma warning restore 0618
                    MessageBox.Show(response.Error.Message);
                    this.LeavePage();
                }
            });
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

        /// <summary>
        /// Shows details of a product/artist/genre/mix group.
        /// Tracks and mixes will be shown in MixRadio app.
        /// </summary>
        /// <param name="sender">Results listbox</param>
        /// <param name="e">Event arguments</param>
        private async void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            await(App.Current as App).RouteItemClick(this.Results.SelectedItem);
            this.Results.SelectedIndex = -1;
        }

        private void Player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CurrentStateChanged: " + this.Player.CurrentState.ToString());
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MediaFailed: " + e.ErrorException.ToString());
        }

        private void Player_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("BufferingProgressChanged: " + this.Player.BufferingProgress.ToString());
        }
    }
}