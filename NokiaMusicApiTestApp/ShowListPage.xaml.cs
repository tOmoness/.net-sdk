﻿// -----------------------------------------------------------------------
// <copyright file="ShowListPage.xaml.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Nokia.Music.Phone;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Tasks;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The List Page
    /// </summary>
    public partial class ShowListPage : PhoneApplicationPage
    {
        public const string MethodParam = "method";

        private MethodCall _method = MethodCall.Unknown;

        public ShowListPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Defines method calls this page can get data with
        /// </summary>
        public enum MethodCall
        {
            /// <summary>
            /// An unknown method
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Get Top Artists
            /// </summary>
            GetTopArtists = 1,

            /// <summary>
            /// Get Top Artists for a genre
            /// </summary>
            GetTopArtistsForGenre = 2,

            /// <summary>
            /// Get the available genres
            /// </summary>
            GetGenres = 3,

            /// <summary>
            /// Get the available mix groups
            /// </summary>
            GetMixGroups = 4,

            /// <summary>
            /// Get the available mixes in a group
            /// </summary>
            GetMixes = 5,

            /// <summary>
            /// Get Top Albums
            /// </summary>
            GetTopAlbums = 6,

            /// <summary>
            /// Get New Albums
            /// </summary>
            GetNewAlbums = 7,
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!NavigationContext.QueryString.ContainsKey(MethodParam))
            {
                MessageBox.Show("The method querystring is missing");
                return;
            }

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                this.Loading.Visibility = Visibility.Visible;
                this.TotalResults.Visibility = Visibility.Collapsed;

                this._method = (MethodCall)Enum.Parse(typeof(MethodCall), NavigationContext.QueryString[MethodParam], true);
                switch (this._method)
                {
                    case MethodCall.GetTopArtists:
                        this.PageTitle.Text = "top artists";
                        App.ApiClient.GetTopArtists(this.ResponseHandler, 0, 20);
                        break;

                    case MethodCall.GetGenres:
                        this.PageTitle.Text = "genres";
                        App.ApiClient.GetGenres(this.ResponseHandler);
                        break;

                    case MethodCall.GetTopArtistsForGenre:
                        if (!NavigationContext.QueryString.ContainsKey(App.IdParam))
                        {
                            MessageBox.Show("The id querystring is missing");
                            return;
                        }

                        string genreId = NavigationContext.QueryString[App.IdParam];
                        this.PageTitle.Text = "top artists for " + genreId.ToLowerInvariant();
                        App.ApiClient.GetTopArtistsForGenre(this.ResponseHandler, genreId, 0, 20);
                        break;

                    case MethodCall.GetMixGroups:
                        this.PageTitle.Text = "mix groups";
                        App.ApiClient.GetMixGroups(this.ResponseHandler, 0, 100);
                        break;

                    case MethodCall.GetMixes:
                        if (!NavigationContext.QueryString.ContainsKey(App.IdParam) || !NavigationContext.QueryString.ContainsKey(App.NameParam))
                        {
                            MessageBox.Show("The querystring is incomplete");
                            return;
                        }

                        string mixId = NavigationContext.QueryString[App.IdParam];
                        this.PageTitle.Text = HttpUtility.UrlDecode(NavigationContext.QueryString[App.NameParam]);
                        App.ApiClient.GetMixes(this.ResponseHandler, mixId);
                        break;

                    case MethodCall.GetTopAlbums:
                        this.PageTitle.Text = "top albums";
                        App.ApiClient.GetTopProducts(this.ResponseHandler, Category.Album, 0, 20);
                        break;

                    case MethodCall.GetNewAlbums:
                        this.PageTitle.Text = "new albums";
                        App.ApiClient.GetNewReleases(this.ResponseHandler, Category.Album, 0, 20);
                        break;

                    default:
                        MessageBox.Show("Unhandled method " + this._method.ToString());
                        return;
                }
            }
        }

        private void ResponseHandler<T>(ListResponse<T> response)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.Loading.Visibility = Visibility.Collapsed;
                this.Results.ItemsSource = response.Result;

                if (response.Result != null)
                {
                    if (response.Result.Count == 0)
                    {
                        this.TotalResults.Visibility = Visibility.Collapsed;
                        MessageBox.Show(@"No results found");
                    }
                    else
                    {
                        if (response.TotalResults != null)
                        {
                            this.TotalResults.Text = string.Format("{0:#,###0} item(s) available", response.TotalResults.Value);
                            this.TotalResults.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.TotalResults.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else if (response.Error != null)
                {
                    MessageBox.Show(response.Error.Message);
                }
            });
        }

        private void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).RouteItemClick(this.Results.SelectedItem);
            this.Results.SelectedIndex = -1;
        }
    }
}