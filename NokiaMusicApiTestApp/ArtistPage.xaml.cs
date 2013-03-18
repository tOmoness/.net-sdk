// -----------------------------------------------------------------------
// <copyright file="ArtistPage.xaml.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Nokia.Music.Phone;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Tasks;
using Nokia.Music.Phone.Types;

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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!NavigationContext.QueryString.ContainsKey(App.IdParam) || !NavigationContext.QueryString.ContainsKey(App.NameParam) || !NavigationContext.QueryString.ContainsKey(App.ThumbParam))
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

            App.ApiClient.GetSimilarArtists(this.ArtistsResponseHandler, this._artistId, 0, 10);
            this.LoadingSongs.Visibility = Visibility.Visible;
            App.ApiClient.GetArtistProducts(this.SongsResponseHandler, this._artistId, Category.Track, 0, 10);
        }

        private void ShowArtist(object sender, RoutedEventArgs e)
        {
            ShowArtistTask task = new ShowArtistTask();
            task.ArtistId = this._artistId;
            task.Show();
        }

        private void ArtistsResponseHandler(ListResponse<Artist> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingArtists.Visibility = Visibility.Collapsed;
                this.SimilarArtists.ItemsSource = response.Result;
            });
        }

        private void SongsResponseHandler(ListResponse<Product> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.LoadingSongs.Visibility = Visibility.Collapsed;
                this.TopSongs.ItemsSource = response.Result;
            });
        }

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