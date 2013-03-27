// -----------------------------------------------------------------------
// <copyright file="AlbumPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
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
using Nokia.Music.Phone;
using Nokia.Music.Phone.Tasks;
using Nokia.Music.Phone.Types;

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

            App.ApiClient.GetSimilarProducts(this.SimilarProductsResponseHandler, this._albumId, 0, 10);
            this.LoadingTracks.Visibility = Visibility.Visible;
            App.ApiClient.GetProduct(this.TracksResponseHandler, this._albumId);
        }

        /// <summary>
        /// Launches Nokia Music app to an album view.
        /// </summary>
        /// <param name="sender">"Show in Nokia Music" button</param>
        /// <param name="e">Event arguments</param>
        private void ShowProduct(object sender, RoutedEventArgs e)
        {
            ShowProductTask task = new ShowProductTask();
            task.ProductId = this._albumId;
            task.Show();
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
        /// Shows details of an album track (in Nokia Music) or similar album.
        /// </summary>
        /// <param name="sender">tracks or similar albums listbox</param>
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