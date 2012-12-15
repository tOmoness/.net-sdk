// -----------------------------------------------------------------------
// <copyright file="Artist.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Tasks;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Nokia Music Artist
    /// </summary>
    public class Artist : MusicItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Artist" /> class.
        /// </summary>
        internal Artist()
        {
        }

        /// <summary>
        /// Gets the artist's country of origin.
        /// </summary>
        /// <value>
        /// The artist's country of origin.
        /// </value>
        public string Country { get; internal set; }

        /// <summary>
        /// Gets the artist's genres.
        /// </summary>
        /// <value>
        /// The artist's genres.
        /// </value>
        public Genre[] Genres { get; internal set; }

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
            return this.Id.GetHashCode();
        }

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

        /// <summary>
        /// Creates an Artist from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>An Artist object</returns>
        internal static Artist FromJToken(JToken item)
        {
            // Extract genres...
            Genre[] genres = null;
            JArray jsonGenres = item.Value<JArray>("genres");
            if (jsonGenres != null)
            {
                List<Genre> list = new List<Genre>();
                foreach (JToken jsonGenre in jsonGenres)
                {
                    list.Add((Genre)Genre.FromJToken(jsonGenre));
                }

                genres = list.ToArray();
            }

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

            // Extract thumbnails...
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square100, out square200, out square320);

            // Create the resulting Artist object...
            return new Artist()
                {
                    Id = item.Value<string>("id"),
                    Name = item.Value<string>("name"),
                    Country = country,
                    Genres = genres,
                    Thumb100Uri = square100,
                    Thumb200Uri = square200,
                    Thumb320Uri = square320
                };
        }
    }
}
