// -----------------------------------------------------------------------
// <copyright file="ArtistPage.xaml.cs" company="MixRadio">
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
using Microsoft.Phone.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Tasks;
using MixRadio.Types;

namespace MixRadio.TestApp
{
    /// <summary>
    /// The Artist Page
    /// </summary>
    public partial class ArtistPage : PhoneApplicationPage
    {
        private string _artistId;
        private string _musicBrainzId;

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

            if (NavigationContext.QueryString.ContainsKey(App.MbIdParam))
            {
                this._musicBrainzId = NavigationContext.QueryString[App.MbIdParam];
            }

            MusicBrainzButton.Visibility = string.IsNullOrEmpty(this._musicBrainzId) ? Visibility.Collapsed : Visibility.Visible;

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
        /// Launches MixRadio app to an artist view.
        /// </summary>
        /// <param name="sender">"Show in MixRadio" button</param>
        /// <param name="e">Event arguments</param>
        private async void ShowArtist(object sender, RoutedEventArgs e)
        {
            await new ShowArtistTask
            {
                ArtistId = this._artistId
            }.Show();
        }

        /// <summary>
        /// Shows the artist on MusicBrainz.
        /// </summary>
        /// <param name="sender">"Show in MsicBrainz" button</param>
        /// <param name="e">Event arguments</param>
        private void ShowMusicBrainz(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this._musicBrainzId))
            {
                new WebBrowserTask
                {
                    Uri = new Uri(string.Format("http://musicbrainz.org/artist/{0}", this._musicBrainzId))
                }.Show();
            }
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
        /// Shows details of a top track (in MixRadio) or similar artist.
        /// </summary>
        /// <param name="sender">top tracks or similar artists listbox</param>
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