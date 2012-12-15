// -----------------------------------------------------------------------
// <copyright file="SearchPage.xaml.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Nokia.Music.Phone;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Tasks;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Search Page
    /// </summary>
    public partial class SearchPage : PhoneApplicationPage
    {
        public const string SearchScopeParam = "scope";
        public const string SearchScopeArtists = "artists";

        private bool _artistSearch = false;

        public SearchPage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SearchPage_Loaded);
        }

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

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SearchTerm.Focus();
        }

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

        private void SearchTermKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Dismiss keyboard
                this.Focus();

                this.PerformSearch(this, new RoutedEventArgs());
            }
        }

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

        private void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).RootItemClick(this.Results.SelectedItem);
            this.Results.SelectedIndex = -1;
        }
    }
}