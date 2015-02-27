// -----------------------------------------------------------------------
// <copyright file="Artist.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
#if !PORTABLE
using System.Threading.Tasks;
#endif
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
#if !PORTABLE
using Nokia.Music.Tasks;
#endif

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a MixRadio Artist
    /// </summary>
    public partial class Artist : MusicItem
    {
        // For now, the Win8 MixRadio app only supports the old nokia-music protocol.
#if WINDOWS_APP
        internal const string AppToAppShowUri = "nokia-music://show/artist/?id={0}";
        internal const string AppToAppPlayUriById = "nokia-music://play/artist/?id={0}";
        internal const string AppToAppPlayUriByName = "nokia-music://play/artist/?artist={0}";
        internal const string AppToAppShowUriById = "nokia-music://show/artist/?id={0}";
        internal const string AppToAppShowUriByName = "nokia-music://show/artist/?name={0}";
#else
        internal const string AppToAppPlayUriById = "mixradio://play/artist/{0}";
        internal const string AppToAppPlayUriByName = "mixradio://play/artist/name/{0}";
        internal const string AppToAppShowUriById = "mixradio://show/artist/{0}";
        internal const string AppToAppShowUriByName = "mixradio://show/artist/name/{0}";
#endif
        internal const string WebShowUriById = "http://www.mixrad.io/artists/-/{0}";
        internal const string WebPlayUriByName = "http://www.mixrad.io/gb/en/mixes/seeded?artists={0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Artist" /> class.
        /// </summary>
        public Artist()
        {
        }

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in MixRadio
        /// </summary>
        public override Uri AppToAppUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(AppToAppShowUriById, this.Id));
                }
                else if (!string.IsNullOrEmpty(this.Name))
                {
                    return new Uri(string.Format(AppToAppShowUriByName, this.Name.Replace("&", string.Empty)));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the app-to-app uri to use to play this item in MixRadio
        /// </summary>
        public Uri AppToAppPlayUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(AppToAppPlayUriById, this.Id));
                }
                else if (!string.IsNullOrEmpty(this.Name))
                {
                    return new Uri(string.Format(AppToAppPlayUriByName, this.Name.Replace("&", string.Empty)));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the web uri to use to show this artist in MixRadio on the web
        /// </summary>
        public override Uri WebUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(WebShowUriById, this.Id));
                }
                else if (!string.IsNullOrEmpty(this.Name))
                {
                    return new Uri(string.Format(WebPlayUriByName, this.Name));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the artist's country of origin.
        /// </summary>
        /// <value>
        /// The artist's country of origin.
        /// </value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the artist's genres.
        /// </summary>
        /// <value>
        /// The artist's genres.
        /// </value>
        public Genre[] Genres { get; set; }

        /// <summary>
        /// Gets or sets the MusicBrainz ID.
        /// </summary>
        /// <value>
        /// The artist MusicBrainz ID if known.
        /// </value>
        public string MusicBrainzId { get; set; }

        /// <summary>
        /// Gets or sets the artist's origin location where available.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        public Location Origin { get; set; }

        /// <summary>
        /// Gets or sets the PlayCount if available
        /// </summary>
        /// <value>
        /// The PlayCount if available
        /// </value>
        public int PlayCount { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Artist target = obj as Artist;
            if (target != null)
            {
                return string.Compare(target.Id, this.Id, StringComparison.OrdinalIgnoreCase) == 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (this.Id == null)
            {
                return base.GetHashCode();
            }

            return this.Id.GetHashCode();
        }

#if !PORTABLE
        /// <summary>
        /// Launches MixRadio to start a mix for the artist using the PlayMixTask
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task PlayMix()
        {
            PlayMixTask task = new PlayMixTask() { ArtistName = this.Name };
            await task.Show().ConfigureAwait(false);
        }

        /// <summary>
        /// Launches MixRadio to show details for the artist using the ShowArtistTask
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            ShowArtistTask task = new ShowArtistTask() { ArtistId = this.Id };
            await task.Show().ConfigureAwait(false);
        }

#endif
        /// <summary>
        /// Creates an Artist from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// An Artist object
        /// </returns>
        internal static Artist FromJToken(JToken item, IMusicClientSettings settings)
        {
            // Extract thumbnails...
            Uri square50 = null;
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square50, out square100, out square200, out square320);

            // Derive 640 thumb
            Uri square640 = null;
            if (square320 != null)
            {
                square640 = new Uri(square320.ToString().Replace("320x320", "640x640"));
            }

            var count = item["count"];
            var playCount = (count != null && count.Type != JTokenType.Null) ? count.Value<int>() : 0;

            return new Artist()
                {
                    Id = item.Value<string>("id"),
                    Name = item.Value<string>("name"),
                    Country = GetCountry(item),
                    Genres = GetGenres(item, settings),
                    MusicBrainzId = item.Value<string>("mbid"),
                    Origin = GetOrigin(item),
                    Thumb50Uri = square50,
                    Thumb100Uri = square100,
                    Thumb200Uri = square200,
                    Thumb320Uri = square320,
                    Thumb640Uri = square640,
                    PlayCount = playCount
                };
        }

        private static Location GetOrigin(JToken item)
        {
            Location origin = null;
            JToken originToken = item["origin"];

            if (originToken != null)
            {
                string name = originToken.Value<string>("name");

                JToken location = originToken["location"];
                if (location != null)
                {
                    origin = new Location()
                    {
                        Latitude = location.Value<double>("lat"),
                        Longitude = location.Value<double>("lng"),
                        Name = name
                    };
                }
            }

            return origin;
        }

        private static Genre[] GetGenres(JToken item, IMusicClientSettings settings)
        {
            // Extract genres...
            Genre[] genres = null;
            JArray jsonGenres = item.Value<JArray>("genres");

            if (jsonGenres != null)
            {
                var list = new List<Genre>();
                foreach (JToken jsonGenre in jsonGenres)
                {
                    list.Add(Genre.FromJToken(jsonGenre, settings));
                }

                genres = list.ToArray();
            }

            return genres;
        }

        private static string GetCountry(JToken item)
        {
            string country = null;
            JToken countryToken = item["country"];

            if (countryToken != null)
            {
                string countryId = countryToken.Value<string>("id");

                // Check country isn't unknown...
                if (string.Compare(countryId, "XX", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    country = countryId.ToLowerInvariant();
                }
            }

            return country;
        }
    }
}
