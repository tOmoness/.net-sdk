// -----------------------------------------------------------------------
// <copyright file="DetailPage.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Types;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            this.InitializeComponent();

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
#endif
        }
#if WINDOWS_PHONE_APP

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
#endif

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
#if WINDOWS_PHONE_APP
                this.Pivot.Items.Clear();
                this.Pivot.Title = "LOADING...";
#else
                this.pageTitle.Text = "loading...";
#endif

                List<GroupedItems> datasource = null;
                string title = null;

                // See if we're displaying an artist...
                Artist artist = e.Parameter as Artist;

                if (artist != null)
                {
                    title = artist.Name.ToLowerInvariant();

                    var topSongs = new GroupedItems()
                    {
                        Title = "top songs"
                    };

                    var similarArtists = new GroupedItems()
                    {
                        Title = "similar artists"
                    };

                    datasource = new List<GroupedItems>() { topSongs, similarArtists };

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
                Product product = e.Parameter as Product;

                if (product != null)
                {
                    title = product.Name.ToLowerInvariant();

                    var tracks = new GroupedItems()
                    {
                        Title = "tracks"
                    };

                    var similarAlbums = new GroupedItems()
                    {
                        Title = "similar albums"
                    };

                    datasource = new List<GroupedItems>() { tracks, similarAlbums };

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
                Genre genre = e.Parameter as Genre;

                if (genre != null)
                {
                    title = genre.Name.ToLowerInvariant();

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

                    datasource = new List<GroupedItems>() { topArtists, topAlbums, topSongs, newAlbums, newSongs };

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

#if WINDOWS_APP
            this.groupedItemsViewSource.Source = datasource;
            this.pageTitle.Text = title;
#elif WINDOWS_PHONE_APP
                this.Pivot.Title = title.ToUpperInvariant();

                foreach (var group in datasource)
                {
                    var pivotItem = new PivotItem();
                    pivotItem.Header = group.Title;

                    var listbox = new ListBox();
                    listbox.SelectionMode = SelectionMode.Single;
                    listbox.SelectionChanged += this.ShowItem;
                    listbox.ItemsSource = group.Items;
                    listbox.ItemTemplate = Application.Current.Resources["ApiListItem"] as DataTemplate;
                    listbox.Background = Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;

                    pivotItem.Content = listbox;

                    this.Pivot.Items.Add(pivotItem);
                }
#endif
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.LeavePage();
        }

        private void LeavePage()
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            var listbox = sender as ListBox;
            await(App.Current as App).RouteItemClick(listbox.SelectedItem, this.Player);
        }

        private async void ItemClick(object sender, ItemClickEventArgs e)
        {
            await(App.Current as App).RouteItemClick(e.ClickedItem, this.Player);
        }
    }
}
