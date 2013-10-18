// -----------------------------------------------------------------------
// <copyright file="ArtistPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
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
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Commands;
using Nokia.Music.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Artist Page
    /// </summary>
    public partial class ArtistPage : PhoneApplicationPage
    {
        private string _artistId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistPage" /> class.
        /// </summary>
        public ArtistPage()
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
        /// artists and similar artists upon successful navigation.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NavigationContext.QueryString.ContainsKey(App.IdParam)
                || !NavigationContext.QueryString.ContainsKey(App.NameParam)
                || !NavigationContext.QueryString.ContainsKey(App.ThumbParam))
            {
                MessageBox.Show("The querystring is incomplete");
                return;
            }

            this._artistId = NavigationContext.QueryString[App.IdParam];

            this.ArtistName.Text = HttpUtility.UrlDecode(NavigationContext.QueryString[App.NameParam]);
            this.ApplicationTitle.Text = this.ArtistName.Text.ToUpperInvariant();
            string thumb = NavigationContext.QueryString[App.ThumbParam];
            if (!string.IsNullOrEmpty(thumb))
            {
                this.ArtistThumb.Source = new BitmapImage(new Uri(HttpUtility.UrlDecode(thumb)));
            }
            else
            {
                this.ArtistThumb.Source = null;
            }

            this.LoadingArtists.Visibility = Visibility.Visible;
            this.ArtistsResponseHandler(await App.ApiClient.GetSimilarArtistsAsync(this._artistId));

            this.LoadingSongs.Visibility = Visibility.Visible;
            this.SongsResponseHandler(await App.ApiClient.GetArtistProductsAsync(this._artistId, Category.Track));
        }

        /// <summary>
        /// Launches Nokia Music app to an artist view.
        /// </summary>
        /// <param name="sender">"Show in Nokia Music" button</param>
        /// <param name="e">Event arguments</param>
        private void ShowArtist(object sender, RoutedEventArgs e)
        {
            ShowArtistTask task = new ShowArtistTask();
            task.ArtistId = this._artistId;
            task.Show();
        }

        /// <summary>
        /// Populates the similar artists list with results from API.
        /// </summary>
        /// <param name="response">List of similar products</param>
        private void ArtistsResponseHandler(ListResponse<Artist> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingArtists.Visibility = Visibility.Collapsed;
                this.SimilarArtists.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Populates the top tracks list with results from API.
        /// </summary>
        /// <param name="response">List of similar products</param>
        private void SongsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingSongs.Visibility = Visibility.Collapsed;
                this.TopSongs.ItemsSource = response.Result;
            });
        }

        /// <summary>
        /// Shows details of a top track (in Nokia Music) or similar artist.
        /// </summary>
        /// <param name="sender">top tracks or similar artists listbox</param>
        /// <param name="e">Event arguments</param>
        private void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;
            if (list != null)
            {
                (App.Current as App).RouteItemClick(list.SelectedItem);
            }
        }
    }
}