// -----------------------------------------------------------------------
// <copyright file="SearchArtistsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
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
        /// Gets or sets the latitude and longitude to search around.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the max distance to search around the location.
        /// </summary>
        /// <value>
        /// The max distance.
        /// </value>
        public int MaxDistance { get; set; }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            if (string.IsNullOrEmpty(this.SearchTerm) && (this.Location == null || (this.Location.Latitude == 0 && this.Location.Longitude == 0)))
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm or location must be supplied");
            }

            string location = null;
            string maxdistance = null;

            if (this.Location != null)
            {
                location = this.Location.ToString();
            }

            if (this.MaxDistance > 0)
            {
                maxdistance = this.MaxDistance.ToString();
            }

            return this.BuildQueryStringParams(this.SearchTerm, null, null, Types.Category.Artist, location, maxdistance, null, null, this.StartIndex, this.ItemsPerPage);
        }

        internal override ListResponse<Artist> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Artist.FromJToken);
        }
    }
}