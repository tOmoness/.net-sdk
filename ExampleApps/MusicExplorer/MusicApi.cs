using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;

using MixRadio;
using MixRadio.Tasks;
using MixRadio.Types;

using MusicExplorer.Models;


namespace MusicExplorer
{
    /// <summary>
    /// This class represents MixRadio API to the rest of the application.
    /// All requests to MixRadio API are sent by an instance of this class,
    /// and all the responses from MixRadio API are handled by this class.
    /// </summary>
    public class MusicApi
    {
        // Members
        private MusicClient client = null;
        private bool initialized = false;

        private Collection<Artist> localArtists = new Collection<Artist>();
        private Collection<string> progressIndicatorTexts = new Collection<string>();

        private int localAudioResponses = 0;
        private int recommendResponses = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MusicApi()
        {
        }

        /// <summary>
        /// Initializes MixRadio API for further requests. Responses to  
        /// requests depend on the region - TopArtists are country specific
        /// for example, and genres are localized by the region.
        /// </summary>
        /// <param name="countryCode">An ISO 3166-2 country code validated by the MixRadio API CountryResolver</param>
        public void Initialize(string countryCode)
        {
            // Create a music client with correct AppId and Token/AppCode
            if (countryCode == null || countryCode.Length != 2)
            {
                client = new MusicClient(MixRadio.TestApp.ApiKeys.ClientId);
            }
            else
            {
                client = new MusicClient(MixRadio.TestApp.ApiKeys.ClientId, countryCode.ToLower());
            }
            initialized = true;
        }

        /// <summary>
        /// Retrieves information (id, genre, thumbnail, etc.) for local artists.
        /// This method initiates a chain of requests, which
        /// 1. requests artist information for each of the local artists
        ///    one after another.
        /// 2. Initiates recommendations searching.
        /// </summary>
        public async void GetArtistInfoForLocalAudio()
        {
            if (!initialized || localAudioResponses >= App.ViewModel.LocalAudio.Count)
            {
                return;
            }

            ArtistModel m = App.ViewModel.LocalAudio[localAudioResponses];

            ShowProgressIndicator("GetArtistInfoForLocalAudio()");
            ListResponse<Artist> response = await client.SearchArtistsAsync(m.Name);

            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                m.Id = response.Result[0].Id;
                m.Country = CountryCodes.CountryNameFromTwoLetter(response.Result[0].Country);

                if (response.Result[0].Genres != null)
                {
                    m.Genres = response.Result[0].Genres[0].Name;
                }
                int itemHeight = Int32.Parse(m.ItemHeight);

                m.Thumb100Uri = response.Result[0].Thumb100Uri;
                m.Thumb200Uri = response.Result[0].Thumb200Uri;
                m.Thumb320Uri = response.Result[0].Thumb320Uri;

                localArtists.Add(response.Result[0]);
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetArtistInfoForLocalAudio()");

            localAudioResponses++;
            GetArtistInfoForLocalAudio();

            if (localAudioResponses == App.ViewModel.LocalAudio.Count)
            {
                // Request recommendations after receiving info for all local artists
                await FetchRecommendations();
            }
        }

        /// <summary>
        /// Builds the recommended artists lists.
        /// This method initiates a chain of requests, which requests similar
        /// artists information for each of the local artists one after another.
        /// </summary>
        private async Task FetchRecommendations()
        {
            if (!initialized || localArtists.Count <= recommendResponses)
            {
                if (initialized && App.ViewModel.Recommendations.Count <= 0)
                {
                    App.ViewModel.NoRecommendedVisibility = Visibility.Visible;
                }
                else
                {
                    App.ViewModel.NoRecommendedVisibility = Visibility.Collapsed;
                }

                // limit the number of recommended artists to 10
                if (localArtists.Count == recommendResponses)
                {
                    int i = App.ViewModel.Recommendations.Count - 1;
                    while (i > 20)
                    {
                        App.ViewModel.Recommendations.RemoveAt(i);
                        i--;
                    }
                }

                return;
            }

            ShowProgressIndicator("FetchRecommendations()");
            ListResponse<Artist> response = await client.GetSimilarArtistsAsync(localArtists[recommendResponses].Id);

            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                foreach (Artist a in response.Result)
                {
                    bool handled = false;

                    // Don't recommend artists already stored in device.
                    if (App.ViewModel.IsLocalArtist(a.Name))
                    {
                        handled = true;
                    }

                    // Check if the artist has already been recommended -> add some weight
                    // to recommendation.
                    if (!handled)
                    {
                        for (int i = 0; i < App.ViewModel.Recommendations.Count; i++)
                        {
                            if (App.ViewModel.Recommendations[i].Name == a.Name)
                            {
                                handled = true;
                                App.ViewModel.Recommendations[i].SimilarArtistCount++;

                                // position according to weight
                                if (i > 1)
                                {
                                    int j = 1;

                                    for (j = i - 1; j > 1; j--)
                                    {
                                        if (App.ViewModel.Recommendations[j].SimilarArtistCount >=
                                            App.ViewModel.Recommendations[i].SimilarArtistCount)
                                        {
                                            j++; // This item (j) has been ranked higher or equal - correct index is one more
                                            break;
                                        }
                                    }

                                    if (i > j)
                                    {
                                        ArtistModel artist = App.ViewModel.Recommendations[i];
                                        App.ViewModel.Recommendations.RemoveAt(i);
                                        App.ViewModel.Recommendations.Insert(j, artist);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // If the artist is not present in the device and has not yet been
                    // recommended, do it now.
                    if (!handled)
                    {
                        App.ViewModel.Recommendations.Add(new ArtistModel()
                        {
                            Name = a.Name,
                            Country = CountryCodes.CountryNameFromTwoLetter(a.Country),
                            Genres =  a.Genres == null ? null : a.Genres[0].Name,
                            Thumb100Uri = a.Thumb100Uri,
                            Thumb200Uri = a.Thumb200Uri,
                            Thumb320Uri = a.Thumb320Uri,
                            Id = a.Id,
                            SimilarArtistCount = 1,
                            ItemWidth = "205",
                            ItemHeight = "205"
                        });
                    }
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("FetchRecommendations()");

            recommendResponses++;
            await FetchRecommendations();
        }

        /// <summary>
        /// Retrieves top artists (10 most popular) from MixRadio API.
        /// </summary>
        public async void GetTopArtists()
        {
            if (!initialized)
            {
                return;
            }

            ShowProgressIndicator("GetTopArtists()");

            ListResponse<Artist> response = await client.GetTopArtistsAsync();
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                App.ViewModel.TopArtists.Clear();

                // Insert a place holder for title text
                App.ViewModel.TopArtists.Add(new ArtistModel()
                {
                    Name = "TitlePlaceholderwho's hot",
                    ItemHeight = "110",
                    ItemWidth = "450"
                });

                foreach (Artist a in response.Result)
                {
                    App.ViewModel.TopArtists.Add(new ArtistModel()
                    {
                        Name = a.Name,
                        Country = CountryCodes.CountryNameFromTwoLetter(a.Country),
                        Genres = a.Genres[0].Name,
                        Thumb100Uri = a.Thumb100Uri,
                        Thumb200Uri = a.Thumb200Uri,
                        Thumb320Uri = a.Thumb320Uri,
                        Id = a.Id,
                        ItemWidth = "205",
                        ItemHeight = "205"
                    });
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetTopArtists()");
        }

        /// <summary>
        /// Retrieves new releases (10 latest albums) from MixRadio API.
        /// </summary>
        public async void GetNewReleases()
        {
            if (!initialized)
            {
                return;
            }

            ShowProgressIndicator("GetNewReleases()");
            ListResponse<Product> response = await client.GetNewReleasesAsync(Category.Album);

            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                App.ViewModel.NewReleases.Clear();

                // Insert a place holder for title text
                App.ViewModel.NewReleases.Add(new ProductModel()
                {
                    Performers = "TitlePlaceholderwhat's new",
                    ItemHeight = "110",
                    ItemWidth = "450"
                });

                foreach (Product p in response.Result)
                {
                    string categoryString = "Album";

                    if (p.Category == Category.Single)
                    {
                        categoryString = "Single";
                    }
                    else if (p.Category == Category.Track)
                    {
                        categoryString = "Track";
                    }

                    string performersString = "";

                    if (p.Performers != null && p.Performers.Length > 0)
                    {
                        performersString = p.Performers[0].Name;
                    }

                    App.ViewModel.NewReleases.Add(new ProductModel()
                    {
                        Performers = performersString,
                        Name = p.Name,
                        Category = categoryString,
                        Thumb100Uri = p.Thumb100Uri,
                        Thumb200Uri = p.Thumb200Uri,
                        Thumb320Uri = p.Thumb320Uri,
                        Id = p.Id,
                        ItemWidth = "205",
                        ItemHeight = "205"
                    });
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }

            HideProgressIndicator("GetNewReleases()");
        }

        /// <summary>
        /// Retrieves available genres from MixRadio API.
        /// </summary>
        public async void GetGenres()
        {
            if (!initialized)
            {
                return;
            }

            ShowProgressIndicator("GetGenres()");
            ListResponse<Genre> response = await client.GetGenresAsync();
            // Use results
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                int genreCount = response.Count();
                App.ViewModel.Genres.Clear();

                foreach (Genre g in response.Result)
                {
                    App.ViewModel.Genres.Add(new GenreModel() { Name = g.Name, Id = g.Id });
                }
            }
            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetGenres()");
        }

        /// <summary>
        /// Retrieves top artists of a selected genre from MixRadio API.
        /// </summary>
        /// <param name="id">Id of the genre.</param>
        public async void GetTopArtistsForGenre(string id)
        {
            if (!initialized)
            {
                return;
            }

            ShowProgressIndicator("GetTopArtistsForGenre()");
            ListResponse<Artist> response = await client.GetTopArtistsForGenreAsync(id);

            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                App.ViewModel.TopArtistsForGenre.Clear();

                foreach (Artist a in response.Result)
                {
                    App.ViewModel.TopArtistsForGenre.Add(new ArtistModel()
                    {
                        Name = a.Name,
                        Country = CountryCodes.CountryNameFromTwoLetter(a.Country),
                        Genres = a.Genres[0].Name,
                        Thumb100Uri = a.Thumb100Uri,
                        Thumb200Uri = a.Thumb200Uri,
                        Thumb320Uri = a.Thumb320Uri,
                        Id = a.Id,
                        ItemWidth = "205",
                        ItemHeight = "205"
                    });
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetTopArtistsForGenre()");
        }

        /// <summary>
        /// Retrieves available mix groups from MixRadio API.
        /// </summary>
        public async void GetMixGroups()
        {
            if (!initialized)
            {
                return;
            }

            ShowProgressIndicator("GetMixGroups()");
            ListResponse<MixGroup> response = await client.GetMixGroupsAsync();
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                App.ViewModel.MixGroups.Clear();

                foreach (MixGroup mg in response.Result)
                {
                    App.ViewModel.MixGroups.Add(new MixGroupModel()
                        {
                            Name = mg.Name,
                            Id = mg.Id
                        });
                }
            }
					
            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetMixGroups()");
        }

        /// <summary>
        /// Retrieves available mixes in a selected mix group from MixRadio API.
        /// </summary>
        /// <param name="id">Id of the mix group.</param>
        public async void GetMixes(string id)
        {
            if (!initialized)
            {
                return;
            }
            
            ShowProgressIndicator("GetMixes()");

            ListResponse<Mix> response = await client.GetMixesAsync(id);

            // Use results
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                App.ViewModel.Mixes.Clear();

                foreach (Mix m in response.Result)
                {
                    string parentalAdvisoryString = "";

                    if (m.ParentalAdvisory)
                    {
                        parentalAdvisoryString = "Parental advisory";
                    }

                    App.ViewModel.Mixes.Add(new MixModel()
                    {
                        Name = m.Name,
                        ParentalAdvisory = parentalAdvisoryString,
                        Id = m.Id,
                        Thumb100Uri = m.Thumb100Uri,
                        Thumb200Uri = m.Thumb200Uri,
                        Thumb320Uri = m.Thumb320Uri,
                        ItemWidth = "205",
                        ItemHeight = "205"
                    });
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetMixes()");
        }

        /// <summary>
        /// Retrieves 30 products for a selected artist from MixRadio API.
        /// </summary>
        /// <param name="id">Id of the artist.</param>
        public async void GetProductsForArtist(string id)
        {
            if (!initialized)
            {
                return;
            }

            App.ViewModel.TracksForArtist.Clear();
            App.ViewModel.AlbumsForArtist.Clear();
            App.ViewModel.SinglesForArtist.Clear();

            App.ViewModel.NoTracksVisibility = Visibility.Visible;
            App.ViewModel.NoAlbumsVisibility = Visibility.Visible;
            App.ViewModel.NoSinglesVisibility = Visibility.Visible;

            ShowProgressIndicator("GetProductsForArtist()");

            ListResponse<Product> response = await client.GetArtistProductsAsync(id, itemsPerPage:30);
            
            // Use results
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                foreach (Product p in response.Result)
                {
                    string priceString = "";

                    if (p.Price != null)
                    {
                        priceString = p.Price.Value + p.Price.Currency;
                    }

                    string takenFromString = "";

                    if (p.TakenFrom != null)
                    {
                        takenFromString = p.TakenFrom.Name;
                    }

                    string performersString = "";

                    if (p.Performers != null && p.Performers.Length > 0)
                    {
                        performersString = p.Performers[0].Name;
                    }

                    string genresString = "";

                    if (p.Genres != null && p.Genres.Length > 0)
                    {
                        genresString = p.Genres[0].Name;
                    }

                    string categoryString = "Album";

                    string trackCountString = null;
                    if (p.TrackCount > 0)
                    {
                        trackCountString = p.TrackCount + " track";
                    }
                    if (p.TrackCount > 1)
                    {
                        trackCountString += "s";
                    }

                    if (p.Category == Category.Track)
                    {
                        categoryString = "Track";
                        App.ViewModel.TracksForArtist.Add(new ProductModel()
                            {
                                Performers = performersString,
                                Name = p.Name,
                                TakenFrom = takenFromString,
                                Category = categoryString,
                                Thumb100Uri = p.Thumb100Uri,
                                Thumb200Uri = p.Thumb200Uri,
                                Thumb320Uri = p.Thumb320Uri,
                                Id = p.Id
                            });
                    }
                    else if (p.Category == Category.Single)
                    {
                        categoryString = "Single";
                        App.ViewModel.SinglesForArtist.Add(new ProductModel()
                            {
                                Performers = performersString,
                                Name = p.Name,
                                TrackCount = trackCountString,
                                Category = categoryString,
                                Thumb100Uri = p.Thumb100Uri,
                                Thumb200Uri = p.Thumb200Uri,
                                Thumb320Uri = p.Thumb320Uri,
                                Id = p.Id
                            });
                    }
                    else
                    {
                        App.ViewModel.AlbumsForArtist.Add(new ProductModel()
                            {
                                Performers = performersString,
                                Name = p.Name,
                                TrackCount = trackCountString,
                                Category = categoryString,
                                Thumb100Uri = p.Thumb100Uri,
                                Thumb200Uri = p.Thumb200Uri,
                                Thumb320Uri = p.Thumb320Uri,
                                Id = p.Id
                            });
                    }
                }

                if (App.ViewModel.TracksForArtist.Count > 0)
                {
                    App.ViewModel.NoTracksVisibility = Visibility.Collapsed;
                }

                if (App.ViewModel.AlbumsForArtist.Count > 0)
                {
                    App.ViewModel.NoAlbumsVisibility = Visibility.Collapsed;
                }

                if (App.ViewModel.SinglesForArtist.Count > 0)
                {
                    App.ViewModel.NoSinglesVisibility = Visibility.Collapsed;
                }
            }

            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetProductsForArtist()");

        }

        /// <summary>
        /// Retrieves similar artists for a selected artist from MixRadio API.
        /// </summary>
        /// <param name="id">Id of the artist.</param>
        public async void GetSimilarArtists(string id)
        {
            if (!initialized)
            {
                return;
            }

            App.ViewModel.SimilarForArtist.Clear();
            App.ViewModel.NoSimilarVisibility = Visibility.Visible;

            ListResponse<Artist> response = await client.GetSimilarArtistsAsync(id);

            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                foreach (Artist a in response.Result)
                {
                    App.ViewModel.SimilarForArtist.Add(new ArtistModel()
                    {
                        Name = a.Name,
                        Country = CountryCodes.CountryNameFromTwoLetter(a.Country),
                        Genres = a.Genres[0].Name,
                        Thumb100Uri = a.Thumb100Uri,
                        Thumb200Uri = a.Thumb200Uri,
                        Thumb320Uri = a.Thumb320Uri,
                        Id = a.Id,
                        ItemWidth = "205",
                        ItemHeight = "205"
                    });
                }
            }

            if (App.ViewModel.SimilarForArtist.Count() > 0)
            {
                App.ViewModel.NoSimilarVisibility = Visibility.Collapsed;
            }


            if (response != null && response.Error != null)
            {
                ShowMixRadioApiError();
            }
            HideProgressIndicator("GetSimilarArtists()");
        }

        /// <summary>
        /// Launches MixRadio App to play a selected mix.
        /// </summary>
        /// <param name="id">Id of the mix.</param>
        public async void LaunchMix(string id)
        {
            if (!initialized)
            {
                return;
            }

            PlayMixTask task = new PlayMixTask();
            task.MixId = id;
            await task.Show();
        }

        /// <summary>
        /// Launches MixRadio App to play a mix for a selected artist.
        /// </summary>
        /// <param name="artistName">Name of the artist.</param>
        public async void LaunchArtistMix(string artistName)
        {
            if (!initialized)
            {
                return;
            }

            PlayMixTask task = new PlayMixTask();
            task.ArtistName = artistName;
            await task.Show();
        }

        /// <summary>
        /// Launches MixRadio App to show information on a selected product.
        /// </summary>
        /// <param name="id">Id of the product.</param>
        public async void LaunchProduct(string id)
        {
            if (!initialized)
            {
                return;
            }

            ShowProductTask task = new ShowProductTask();
            task.ProductId = id;
            await task.Show();
        }

        /// <summary>
        /// Launches MixRadio App to show information on a selected artist.
        /// </summary>
        /// <param name="id">id of the artist.</param>
        public async void LaunchArtist(string id)
        {
            if (!initialized)
            {
                return;
            }

            ShowArtistTask task = new ShowArtistTask();
            task.ArtistId = id;
            await task.Show();
        }

        /// <summary>
        /// This method makes progress indicator visible.
        /// Provided text is added into an array of strings of which the
        /// latest text is shown in progress indicator.
        /// </summary>
        /// <param name="text">Text to show in progress indicator.</param>
        private void ShowProgressIndicator(string text)
        {
            progressIndicatorTexts.Add(text);
            App.ViewModel.ProgressIndicatorText = progressIndicatorTexts[progressIndicatorTexts.Count - 1];
            App.ViewModel.ProgressIndicatorVisible = true;
        }

        /// <summary>
        /// This method removes provided text from the array of strings of
        /// which the latest text is shown in progress indicator.
        /// Indicator is hidden if the array becomes empty. 
        /// </summary>
        /// <param name="text">Text to be removed from progress indicator.</param>
        private void HideProgressIndicator(string text)
        {
            progressIndicatorTexts.Remove(text);
            if (progressIndicatorTexts.Count > 0)
            {
                App.ViewModel.ProgressIndicatorText = progressIndicatorTexts[progressIndicatorTexts.Count - 1];
            }
            else
            {
                App.ViewModel.ProgressIndicatorText = "";
                App.ViewModel.ProgressIndicatorVisible = false;
            }
        }

        /// <summary>
        /// Shows MessageBox with MixRadio API error text
        /// </summary>
        void ShowMixRadioApiError()
        {
            MessageBox.Show("MixRadio API error. Please ensure that the "
                          + "device is connected to Internet and restart "
                          + "the application.");
        }
    }
}
