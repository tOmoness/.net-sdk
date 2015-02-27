// -----------------------------------------------------------------------
// <copyright file="ShowListPage.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Types;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ShowListPage : Page
    {
        private ShowListParams _params = null;

        public ShowListPage()
        {
            this.InitializeComponent();

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
#endif
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;

            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
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

            this._params = (ShowListParams)e.Parameter;

            ////this.Loading.Visibility = Visibility.Visible;
            ////this.TotalResults.Visibility = Visibility.Collapsed;

            switch (this._params.Method)
            {
                case MethodCall.GetTopArtists:
                    this.PageTitle.Text = "top artists";
                    this.ResponseHandler<Artist>(await App.ApiClient.GetTopArtistsAsync(0, 40));
                    break;

                case MethodCall.GetGenres:
                    this.PageTitle.Text = "genres";
                    this.ResponseHandler<Genre>(await App.ApiClient.GetGenresAsync());
                    break;

                case MethodCall.GetTopArtistsForGenre:
                    if (string.IsNullOrEmpty(this._params.Id))
                    {
                        await MessageBox.Show("The id parameter was missing");
                        return;
                    }

                    string genreId = this._params.Id;
                    this.PageTitle.Text = "top artists for " + genreId.ToLowerInvariant();
                    this.ResponseHandler<Artist>(await App.ApiClient.GetTopArtistsForGenreAsync(genreId, 0, 40));
                    break;

                case MethodCall.GetMixGroups:
                    this.PageTitle.Text = "mix groups";
                    this.ResponseHandler<MixGroup>(await App.ApiClient.GetMixGroupsAsync(0, 100));
                    break;

                case MethodCall.GetMixes:
                    if (string.IsNullOrEmpty(this._params.Id))
                    {
                        await MessageBox.Show("The id parameter was missing");
                        return;
                    }

                    string mixId = this._params.Id;
                    this.PageTitle.Text = (this._params.Parameter as string).ToLowerInvariant();
                    this.ResponseHandler<Mix>(await App.ApiClient.GetMixesAsync(mixId));
                    break;

                case MethodCall.GetTopAlbums:
                    this.PageTitle.Text = "top albums";
                    this.ResponseHandler<Product>(await App.ApiClient.GetTopProductsAsync(Category.Album, 0, 40));
                    break;

                case MethodCall.GetNewAlbums:
                    this.PageTitle.Text = "new albums";
                    this.ResponseHandler<Product>(await App.ApiClient.GetNewReleasesAsync(Category.Album, 0, 40));
                    break;

                case MethodCall.Search:
                    string term = this._params.Parameter as string;
                    this.PageTitle.Text = "search for " + term.ToLowerInvariant();
                    this.ResponseHandler<MusicItem>(await App.ApiClient.SearchAsync(term, itemsPerPage: 40));
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
                    await MessageBox.Show("Unhandled method " + this._params.ToString());
                    return;
            }
        }

        /// <summary>
        /// Populates list box with search results.
        /// </summary>
        /// <param name="response">Search results from MixRadio API</param>
        /// <typeparam name="T">Any MusicItem from MixRadio API</typeparam>
        private async void ResponseHandler<T>(ListResponse<T> response)
        {
#if WINDOWS_PHONE_APP
            this.Results.ItemsSource = response.Result;
#else
            this.itemsViewSource.Source = response.Result;
#endif
            if (response.Result != null)
            {
                if (response.Result.Count == 0)
                {
                    await MessageBox.Show(@"No results found");
                }
            }
            else if (response.Error != null)
            {
#pragma warning disable 0618  // Disable this for now
                await App.ApiClient.DeleteAuthenticationTokenAsync();
#pragma warning restore 0618
                await MessageBox.Show(response.Error.Message);
                this.LeavePage();
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

#if WINDOWS_PHONE_APP
        private async void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            await (App.Current as App).RouteItemClick(this.Results.SelectedItem, this.Player);
        }
#else
        private async void ItemClick(object sender, ItemClickEventArgs e)
        {
            await(App.Current as App).RouteItemClick(e.ClickedItem, this.Player);
        }
#endif
    }
}
