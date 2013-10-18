// -----------------------------------------------------------------------
// <copyright file="Artist.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if !PORTABLE
using Nokia.Music.Tasks;
#endif

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Nokia Music Artist
    /// </summary>
    public partial class Artist : MusicItem
    {
        internal const string AppToAppShowUri = "nokia-music://show/artist/?id={0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Artist" /> class.
        /// </summary>
        public Artist()
        {
        }

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in Nokia Music
        /// </summary>
        public override Uri AppToAppUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(AppToAppShowUri, this.Id));
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
        /// Gets or sets the artist's origin location where available.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        public Location Origin { get; set; }

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
        /// Launches Nokia Music to start a mix for the artist using the PlayMixTask
        /// </summary>
        public void PlayMix()
        {
            PlayMixTask task = new PlayMixTask() { ArtistName = this.Name };
            task.Show();
        }

        /// <summary>
        /// Launches Nokia Music to show details for the artist using the ShowArtistTask
        /// </summary>
        public void Show()
        {
            ShowArtistTask task = new ShowArtistTask() { ArtistId = this.Id };
            task.Show();
        }

#endif
        /// <summary>
        /// Creates an Artist from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>An Artist object</returns>
        internal static Artist FromJToken(JToken item)
        {
            // Extract thumbnails...
            Uri square50 = null;
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square50, out square100, out square200, out square320);

            return new Artist()
                {
                    Id = item.Value<string>("id"),
                    Name = item.Value<string>("name"),
                    Country = GetCountry(item),
                    Genres = GetGenres(item),
                    Origin = GetOrigin(item),
                    Thumb50Uri = square50,
                    Thumb100Uri = square100,
                    Thumb200Uri = square200,
                    Thumb320Uri = square320
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

        private static Genre[] GetGenres(JToken item)
        {
            // Extract genres...
            Genre[] genres = null;
            JArray jsonGenres = item.Value<JArray>("genres");

            if (jsonGenres != null)
            {
                var list = new List<Genre>();
                foreach (JToken jsonGenre in jsonGenres)
                {
                    list.Add(Genre.FromJToken(jsonGenre));
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
