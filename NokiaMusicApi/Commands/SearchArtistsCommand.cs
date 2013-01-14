// -----------------------------------------------------------------------
// <copyright file="SearchArtistsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Searches for an Artist
    /// </summary>
    internal sealed class SearchArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the latitude to search around.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude to search around.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the max distance to search around the location.
        /// </summary>
        /// <value>
        /// The max distance.
        /// </value>
        public int MaxDistance { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <exception cref="System.ArgumentNullException">SearchTerm;A searchTerm must be supplied</exception>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.SearchTerm) && (this.Latitude == 0 || this.Longitude == 0))
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm or location must be supplied");
            }

            string location = null;
            string maxdistance = null;

            if (this.Latitude != 0 && this.Longitude != 0)
            {
                location = string.Format("{0},{1}", this.Latitude, this.Longitude);
            }

            if (this.MaxDistance > 0)
            {
                maxdistance = this.MaxDistance.ToString();
            }

            this.InternalSearch(this.SearchTerm, null, Category.Artist, location, maxdistance, this.StartIndex, this.ItemsPerPage, Artist.FromJToken, this.Callback);
        }
    }
}