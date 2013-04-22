// -----------------------------------------------------------------------
// <copyright file="DetailPage.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Types;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class DetailPage : Nokia.Music.TestApp.Common.LayoutAwarePage
    {
        public DetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            // See if we're displaying an artist...
            Artist artist = navigationParameter as Artist;

            if (artist != null)
            {
                this.pageTitle.Text = artist.Name.ToLowerInvariant();

                var topSongs = new GroupedItems()
                {
                    Title = "top songs"
                };

                var similarArtists = new GroupedItems()
                {
                    Title = "similar artists"
                };

                this.DefaultViewModel["Groups"] = new List<GroupedItems>() { topSongs, similarArtists };

                ListResponse<Product> topSongsList = await App.ApiClient.GetArtistProductsAsync(artist, category: Category.Track, itemsPerPage: 6);
                if (topSongsList.Result != null)
                {
                    foreach (Product p in topSongsList)
                    {
                        topSongs.Items.Add(p);
                    }
                }

                ListResponse<Artist> similarArtistList = await App.ApiClient.GetSimilarArtistsAsync(artist, itemsPerPage: 8);
                if (similarArtistList.Result != null)
                {
                    foreach (Artist a in similarArtistList)
                    {
                        similarArtists.Items.Add(a);
                    }
                }
            }

            // See if we're displaying a product...
            Product product = navigationParameter as Product;

            if (product != null)
            {
                this.pageTitle.Text = product.Name.ToLowerInvariant();

                var tracks = new GroupedItems()
                {
                    Title = "tracks"
                };

                var similarAlbums = new GroupedItems()
                {
                    Title = "similar albums"
                };

                this.DefaultViewModel["Groups"] = new List<GroupedItems>() { tracks, similarAlbums };

                Response<Product> productDetails = await App.ApiClient.GetProductAsync(product.Id);
                if (productDetails.Result != null)
                {
                    foreach (Product p in productDetails.Result.Tracks)
                    {
                        tracks.Items.Add(p);
                    }
                }

                ListResponse<Product> similarAlbumsList = await App.ApiClient.GetSimilarProductsAsync(product, itemsPerPage: 8);
                if (similarAlbumsList.Result != null)
                {
                    foreach (Product p in similarAlbumsList)
                    {
                        similarAlbums.Items.Add(p);
                    }
                }
            }

            // See if we're displaying a genre...
            Genre genre = navigationParameter as Genre;

            if (genre != null)
            {
                this.pageTitle.Text = genre.Name.ToLowerInvariant();

                var topArtists = new GroupedItems()
                {
                    Title = "top artists"
                };

                var topAlbums = new GroupedItems()
                {
                    Title = "top albums"
                };

                var topSongs = new GroupedItems()
                {
                    Title = "top songs"
                };

                var newAlbums = new GroupedItems()
                {
                    Title = "new albums"
                };

                var newSongs = new GroupedItems()
                {
                    Title = "new songs"
                };

                this.DefaultViewModel["Groups"] = new List<GroupedItems>() { topArtists, topAlbums, topSongs, newAlbums, newSongs };

                ListResponse<Artist> topArtistsList = await App.ApiClient.GetTopArtistsForGenreAsync(genre, itemsPerPage: 6);
                if (topArtistsList.Result != null)
                {
                    foreach (Artist a in topArtistsList.Result)
                    {
                        topArtists.Items.Add(a);
                    }
                }

                ListResponse<Product> topAlbumsList = await App.ApiClient.GetTopProductsForGenreAsync(genre, Category.Album, itemsPerPage: 6);
                if (topAlbumsList.Result != null)
                {
                    foreach (Product p in topAlbumsList)
                    {
                        topAlbums.Items.Add(p);
                    }
                }

                ListResponse<Product> topSongsList = await App.ApiClient.GetTopProductsForGenreAsync(genre, Category.Track, itemsPerPage: 6);
                if (topSongsList.Result != null)
                {
                    foreach (Product p in topSongsList)
                    {
                        topSongs.Items.Add(p);
                    }
                }

                ListResponse<Product> newAlbumsList = await App.ApiClient.GetNewReleasesForGenreAsync(genre, Category.Album, itemsPerPage: 6);
                if (newAlbumsList.Result != null)
                {
                    foreach (Product p in newAlbumsList)
                    {
                        newAlbums.Items.Add(p);
                    }
                }

                ListResponse<Product> newSongsList = await App.ApiClient.GetNewReleasesForGenreAsync(genre, Category.Track, itemsPerPage: 6);
                if (topSongsList.Result != null)
                {
                    foreach (Product p in newSongsList)
                    {
                        newSongs.Items.Add(p);
                    }
                }
            }
        }

        private void ItemClick(object sender, ItemClickEventArgs e)
        {
            (App.Current as App).RouteItemClick(e.ClickedItem, this.Player);
        }
    }
}
