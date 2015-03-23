// -----------------------------------------------------------------------
// <copyright file="GenrePage.xaml.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Tasks;
using MixRadio.Types;

namespace MixRadio.TestApp
{
    /// <summary>
    /// The Genre Page
    /// </summary>
    public partial class GenrePage : PhoneApplicationPage
    {
        private string _genreId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenrePage" /> class.
        /// </summary>
        public GenrePage()
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
        /// Initializes details view and makes requests for top tracks of the
        /// Genres and similar Genres upon successful navigation.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NavigationContext.QueryString.ContainsKey(App.IdParam)
                || !NavigationContext.QueryString.ContainsKey(App.NameParam))
            {
                MessageBox.Show("The querystring is incomplete");
                return;
            }

            this._genreId = NavigationContext.QueryString[App.IdParam];

            this.ApplicationTitle.Text = HttpUtility.UrlDecode(NavigationContext.QueryString[App.NameParam]).ToUpperInvariant();

            this.LoadingArtists.Visibility = Visibility.Visible;
            App.ApiClient.GetTopArtistsForGenreAsync(this._genreId, 0, 10).ContinueWith(result => this.TopArtistsResponseHandler(result.Result));

            this.LoadingTopAlbums.Visibility = Visibility.Visible;
            App.ApiClient.GetTopProductsForGenreAsync(this._genreId, Category.Album, 0, 10).ContinueWith(result => this.TopAlbumsResponseHandler(result.Result));

            this.LoadingTopSongs.Visibility = Visibility.Visible;
            App.ApiClient.GetTopProductsForGenreAsync(this._genreId, Category.Track, 0, 10).ContinueWith(result => this.TopSongsResponseHandler(result.Result));

            this.LoadingNewAlbums.Visibility = Visibility.Visible;
            App.ApiClient.GetNewReleasesForGenreAsync(this._genreId, Category.Album, 0, 10).ContinueWith(result => this.NewAlbumsResponseHandler(result.Result));

            this.LoadingNewSongs.Visibility = Visibility.Visible;
            App.ApiClient.GetNewReleasesForGenreAsync(this._genreId, Category.Track, 0, 10).ContinueWith(result => this.NewSongsResponseHandler(result.Result));
        }

        /// <summary>
        /// Populates the top artists list with results from API.
        /// </summary>
        /// <param name="response">List of items from the API</param>
        private void TopArtistsResponseHandler(ListResponse<Artist> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingArtists.Visibility = Visibility.Collapsed;
                this.TopArtists.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the top albums list with results from API.
        /// </summary>
        /// <param name="response">List of items from the API</param>
        private void TopAlbumsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingTopAlbums.Visibility = Visibility.Collapsed;
                this.TopAlbums.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the top tracks list with results from API.
        /// </summary>
        /// <param name="response">List of items from the API</param>
        private void TopSongsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingTopSongs.Visibility = Visibility.Collapsed;
                this.TopSongs.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the new albums list with results from API.
        /// </summary>
        /// <param name="response">List of items from the API</param>
        private void NewAlbumsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingNewAlbums.Visibility = Visibility.Collapsed;
                this.NewAlbums.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the new tracks list with results from API.
        /// </summary>
        /// <param name="response">List of items from the API</param>
        private void NewSongsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingNewSongs.Visibility = Visibility.Collapsed;
                this.NewSongs.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Shows details of a top track (in MixRadio) or similar Genre.
        /// </summary>
        /// <param name="sender">top tracks or similar Genres listbox</param>
        /// <param name="e">Event arguments</param>
        private async void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list != null)
            {
                await(App.Current as App).RouteItemClick(list.SelectedItem);
            }
        }
    }
}