﻿// -----------------------------------------------------------------------
// <copyright file="AlbumPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Microsoft Mobile. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Microsoft Mobile. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Album Page
    /// </summary>
    public partial class AlbumPage : PhoneApplicationPage
    {
        private string _albumId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumPage" /> class.
        /// </summary>
        public AlbumPage()
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
        /// Initializes details view and makes requests for tracks on the 
        /// album and similar albums upon successful navigation.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NavigationContext.QueryString.ContainsKey(App.IdParam) 
                || !NavigationContext.QueryString.ContainsKey(App.NameParam) 
                || !NavigationContext.QueryString.ContainsKey(App.ThumbParam))
            {
                MessageBox.Show("The querystring is incomplete");
                return;
            }

            this._albumId = NavigationContext.QueryString[App.IdParam];

            this.AlbumName.Text = HttpUtility.UrlDecode(NavigationContext.QueryString[App.NameParam]);
            this.ApplicationTitle.Text = this.AlbumName.Text.ToUpperInvariant();
            string thumb = NavigationContext.QueryString[App.ThumbParam];
            if (!string.IsNullOrEmpty(thumb))
            {
                this.AlbumThumb.Source = new BitmapImage(new Uri(HttpUtility.UrlDecode(thumb)));
            }
            else
            {
                this.AlbumThumb.Source = null;
            }

            this.LoadingAlbums.Visibility = Visibility.Visible;

            App.ApiClient.GetSimilarProductsAsync(this._albumId, 0, 10).ContinueWith(result => this.SimilarProductsResponseHandler(result.Result));
            this.LoadingTracks.Visibility = Visibility.Visible;
            App.ApiClient.GetProductAsync(this._albumId).ContinueWith(result => this.TracksResponseHandler(result.Result));
        }

        /// <summary>
        /// Launches MixRadio app to an album view.
        /// </summary>
        /// <param name="sender">"Show in MixRadio" button</param>
        /// <param name="e">Event arguments</param>
        private async void ShowProduct(object sender, RoutedEventArgs e)
        {
            ShowProductTask task = new ShowProductTask() { ClientId = ApiKeys.ClientId, ProductId = this._albumId };
            await task.Show();
        }

        /// <summary>
        /// Populates the similar albums list with results from API.
        /// </summary>
        /// <param name="response">List of similar products</param>
        private void SimilarProductsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingAlbums.Visibility = Visibility.Collapsed;
                this.SimilarAlbums.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the album tracks list with results from API.
        /// </summary>
        /// <param name="response">List of similar products</param>
        private void TracksResponseHandler(Response<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingTracks.Visibility = Visibility.Collapsed;
                this.Tracks.ItemsSource = response.Result.Tracks;
            });
        }

        /// <summary>
        /// Shows details of an album track (in MixRadio) or similar album.
        /// </summary>
        /// <param name="sender">tracks or similar albums listbox</param>
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