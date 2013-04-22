// -----------------------------------------------------------------------
// <copyright file="LibraryRecommendations.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Commands;
using Nokia.Music.Types;
using Xna = Microsoft.Xna.Framework.Media;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Library Recommendations Page
    /// </summary>
    public partial class LibraryRecommendations : PhoneApplicationPage
    {
        private List<TopArtist> _topArtists = new List<TopArtist>();
        private List<Artist> _recommendedArtists = new List<Artist>();
        
        private int _index = 0;
        private int _alreadyHad = 0;
        private bool _abort = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryRecommendations" /> class.
        /// </summary>
        public LibraryRecommendations()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts searching for recommendations.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                this.Intro.Visibility = Visibility.Visible;
                this.Loading.Text = "Finding artists...";
                this.Loading.Visibility = Visibility.Visible;
                this.FindTopArtists();
            }
        }

        /// <summary>
        /// Aborts recommendation process when navigating away from the page.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Debug.WriteLine("navigating away...");
            this._abort = true;
        }

        /// <summary>
        /// Reads local media for artists, which are then ordered based on
        /// the number of tracks and play count of their tracks. Begins
        /// further processing to find recommendations using Nokia Music API.
        /// </summary>
        private void FindTopArtists()
        {
            Xna.MediaLibrary lib = new Xna.MediaLibrary();

            // Get the top artists from the local media library...
            if (lib.Artists.Count > 0)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    this.Note.Text = string.Format("{0} artists on phone...", lib.Artists.Count);
                });

                List<TopArtist> artists = new List<TopArtist>();
                foreach (Xna.Artist artist in lib.Artists)
                {
                    int playCount = 0;
                    try
                    {
                        int count = artist.Songs.Count;
                        foreach (Xna.Song song in artist.Songs)
                        {
                            playCount += song.PlayCount;
                        }

                        int score = count + (playCount * 2);
                        artists.Add(new TopArtist { Name = artist.Name, Score = score });
                    }
                    catch
                    {
                    }
                }

                this._topArtists = (from a in artists
                                    orderby a.Score descending
                                    select a).ToList<TopArtist>();

                Dispatcher.BeginInvoke(() =>
                    {
                        this.Loading.Text = string.Format("Found {0} artists, looking for recommendations...", this._topArtists.Count);
                    });

                this.ProcessNextArtist();
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        this.Loading.Visibility = Visibility.Collapsed;
                        this.Note.Visibility = Visibility.Visible;
                        this.Note.Text = "There is no music on the phone, please add some and try again.";
                    });
            }
        }

        /// <summary>
        /// Works through the top artist list, trying to find the artist in
        /// the Nokia Music API, recommending similar artists from there
        /// and then checking if the user has the artist in their library already!
        /// <remarks>It gets a bit gnarly in here!</remarks>
        /// </summary>
        private void ProcessNextArtist()
        {
            // Check if we've been navigated away from...
            if (this._abort)
            {
                Debug.WriteLine("aborting...");
                return;
            }

            if (this._index < this._topArtists.Count && this._recommendedArtists.Count < 20)
            {
                string name = this._topArtists[this._index].Name;

                Dispatcher.BeginInvoke(() =>
                {
                    this.Loading.Text = string.Format("Searching for {0}...", name);
                });

                App.ApiClient.SearchArtists(
                    (searchResponse) =>
                    {
                        if (searchResponse.Result != null && searchResponse.Result.Count == 1)
                        {
                            Artist artist = searchResponse.Result[0];
                            if (string.Compare(artist.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    this.Loading.Text = string.Format("Getting similar artists for {0}...", name);
                                });

                                // Check if we've been navigated away from...
                                if (this._abort)
                                {
                                    Debug.WriteLine("aborting...");
                                    return;
                                }

                                App.ApiClient.GetSimilarArtists(
                                    (similarResponse) =>
                                    {
                                        if (similarResponse.Result != null)
                                        {
                                            Debug.WriteLine(string.Format("Got {0} recommendations for {1}", similarResponse.Result.Count, artist.Name));
                                            foreach (Artist recommended in similarResponse.Result)
                                            {
                                                // See if we have this already (from the user library or from recommendations)...
                                                int libCount = (from a in this._topArtists
                                                                where string.Compare(a.Name, recommended.Name, StringComparison.InvariantCultureIgnoreCase) == 0
                                                                select a).Count();

                                                int recCount = (from a in this._recommendedArtists
                                                                where string.Compare(a.Name, recommended.Name, StringComparison.InvariantCultureIgnoreCase) == 0
                                                                select a).Count();

                                                if (libCount == 0 && recCount == 0)
                                                {
                                                    this._recommendedArtists.Add(recommended);
                                                }
                                                else if (recCount == 0)
                                                {
                                                    this._alreadyHad++;
                                                }
                                            }

                                            this._index++;
                                            ProcessNextArtist();
                                        }
                                        else
                                        {
                                            this._index++;
                                            ProcessNextArtist();
                                        }
                                    },
                                    artist,
                                    0,
                                    5);
                            }
                            else
                            {
                                this._index++;
                                ProcessNextArtist();
                            }
                        }
                        else
                        {
                            this._index++;
                            ProcessNextArtist();
                        }
                    },
                    name,
                    0,
                    1);
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (this._recommendedArtists.Count > 0 || this._alreadyHad > 0)
                    {
                        this.Loading.Text = string.Format("We found {0} artists that you don't have on the phone; you already had {1} we thought you might like.", this._recommendedArtists.Count, this._alreadyHad);
                    }
                    else
                    {
                        this.Loading.Text = "We couldn't find any recommendations for you, sorry.";
                    }

                    this.Intro.Visibility = Visibility.Collapsed;
                    this.Results.ItemsSource = this._recommendedArtists;
                });
            }
        }

        /// <summary>
        /// Shows details of recommended artist.
        /// </summary>
        /// <param name="sender">recommended artists listbox</param>
        /// <param name="e">Event arguments</param>
        private void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            (App.Current as App).RouteItemClick(this.Results.SelectedItem);
            this.Results.SelectedIndex = -1;
        }

        /// <summary>
        /// Represents local artist with a score.
        /// </summary>
        private struct TopArtist
        {
            /// <summary>
            /// Gets or sets the Artist Name
            /// </summary>
            public string Name { get; internal set; }

            /// <summary>
            /// Gets or sets the Score
            /// </summary>
            public int Score { get; internal set; }
        }
    }
}