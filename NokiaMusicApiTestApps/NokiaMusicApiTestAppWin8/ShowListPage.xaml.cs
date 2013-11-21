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
    public sealed partial class ShowListPage : Nokia.Music.TestApp.Common.LayoutAwarePage
    {
        private ShowListParams _params = null;

        public ShowListPage()
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
        protected override async void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            this._params = (ShowListParams)navigationParameter;

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

                default:
                    await MessageBox.Show("Unhandled method " + this._params.ToString());
                    return;
            }
        }

        /// <summary>
        /// Populates list box with search results.
        /// </summary>
        /// <param name="response">Search results from Nokia MixRadio API</param>
        /// <typeparam name="T">Any MusicItem from Nokia MixRadio API</typeparam>
        private async void ResponseHandler<T>(ListResponse<T> response)
        {
            this.DefaultViewModel["Items"] = response.Result;

            if (response.Result != null)
            {
                if (response.Result.Count == 0)
                {
                    await MessageBox.Show(@"No results found");
                }
            }
            else if (response.Error != null)
            {
                await App.ApiClient.DeleteAuthenticationTokenAsync();
                await MessageBox.Show(response.Error.Message);
                this.LeavePage();
            }
        }

        private void LeavePage()
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void ItemClick(object sender, ItemClickEventArgs e)
        {
            (App.Current as App).RouteItemClick(e.ClickedItem, this.Player);
        }
    }
}
