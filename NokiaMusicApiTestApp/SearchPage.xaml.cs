// -----------------------------------------------------------------------
// <copyright file="SearchPage.xaml.cs" company="Nokia">
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
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Search Page
    /// </summary>
    public partial class SearchPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constant for search scope parameter.
        /// </summary>
        public const string SearchScopeParam = "scope";

        /// <summary>
        /// Constant for search scope for artists.
        /// </summary>
        public const string SearchScopeArtists = "artists";

        private bool _artistSearch = false;

        /// <summary>
        /// Constructor for SearchPage.
        /// </summary>
        public SearchPage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SearchPage_Loaded);
        }

        /// <summary>
        /// Initializes the page based on search scope.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey(SearchScopeParam))
            {
                this._artistSearch = string.CompareOrdinal(NavigationContext.QueryString[SearchScopeParam], SearchScopeArtists) == 0;
            }

            if (this._artistSearch)
            {
                this.PageTitle.Text = "artist search";
            }
            else
            {
                this.PageTitle.Text = "search";
            }
        }

        /// <summary>
        /// Puts focus on search box after initialization.
        /// </summary>
        /// <param name="sender">SearchPage object</param>
        /// <param name="e">Event arguments</param>
        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SearchTerm.Focus();
            this.Loaded -= this.SearchPage_Loaded;
        }

        /// <summary>
        /// Request auto completion data from Nokia Music API.
        /// </summary>
        /// <param name="sender">AutoCompleteBox object</param>
        /// <param name="e">Event arguments</param>
        private void SearchPopulating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;

            if (this._artistSearch)
            {
                App.ApiClient.GetArtistSearchSuggestions(this.HandleSearchSuggestionsResponse, e.Parameter);
            }
            else
            {
                App.ApiClient.GetSearchSuggestions(this.HandleSearchSuggestionsResponse, e.Parameter);
            }
        }

        /// <summary>
        /// Populates AutoCompleteBox with suggestions.
        /// </summary>
        /// <param name="response">List of auto complete suggestions</param>
        private void HandleSearchSuggestionsResponse(ListResponse<string> response)
        {
            if (response.Result != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.SearchTerm.ItemsSource = response.Result;
                    this.SearchTerm.PopulateComplete();
                });
            }
        }

        /// <summary>
        /// Starts searching with current search term.
        /// </summary>
        /// <param name="sender">AutoCompleteBox object</param>
        /// <param name="e">Event arguments</param>
        private void SearchTermKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Dismiss keyboard
                this.Focus();

                this.PerformSearch(this, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Makes a search request with current search term.
        /// </summary>
        /// <param name="sender">SearchButton object</param>
        /// <param name="e">Event arguments</param>
        private void PerformSearch(object sender, RoutedEventArgs e)
        {
            if (this.SearchTerm.Text.Trim().Length > 0)
            {
                // Scroll to top...
                if (this.Results.Items != null && this.Results.Items.Count > 0)
                {
                    this.Results.ScrollIntoView(this.Results.Items[0]);
                }

                this.Results.ItemsSource = null;
                this.Loading.Visibility = Visibility.Visible;

                this.SearchTerm.IsEnabled = false;
                this.SearchButton.IsEnabled = false;

                if (this._artistSearch)
                {
                    App.ApiClient.SearchArtists(this.ResponseHandler, this.SearchTerm.Text, 0, 20);
                }
                else
                {
                    App.ApiClient.Search(this.ResponseHandler, this.SearchTerm.Text, itemsPerPage: 20);
                }
            }
        }

        /// <summary>
        /// Populates list box with search results.
        /// </summary>
        /// <param name="response">Search results from Nokia Music API</param>
        /// <typeparam name="T">Any MusicItem from Nokia Music API</typeparam>
        private void ResponseHandler<T>(ListResponse<T> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.Loading.Visibility = Visibility.Collapsed;
                this.Results.ItemsSource = response.Result;

                if (response.Result != null && response.Result.Count == 0)
                {
                    MessageBox.Show(@"No results found");
                }
                else if (response.Error != null)
                {
                    MessageBox.Show(response.Error.Message);
                }

                this.SearchTerm.IsEnabled = true;
                this.SearchButton.IsEnabled = true;
            });
        }

        /// <summary>
        /// Shows details of a product or artist.
        /// Tracks will be shown in Nokia Music app.
        /// </summary>
        /// <param name="sender">Results listbox</param>
        /// <param name="e">Event arguments</param>
        private void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).RouteItemClick(this.Results.SelectedItem);
            this.Results.SelectedIndex = -1;
        }
    }
}