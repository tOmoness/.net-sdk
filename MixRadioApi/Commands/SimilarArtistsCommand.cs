// -----------------------------------------------------------------------
// <copyright file="SimilarArtistsCommand.cs" company="Nokia">
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
    /// Gets similar artists to the supplied artist
    /// </summary>
    internal sealed class SimilarArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the artist id.
        /// </summary>
        /// <value>
        /// The artist id.
        /// </value>
        public string ArtistId { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (string.IsNullOrEmpty(this.ArtistId))
            {
                throw new ArgumentNullException("ArtistId", "An artist ID must be supplied");
            }

            uri.AppendFormat("creators/{0}/similar/", this.ArtistId);
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return this.GetPagingParams();
        }

        internal override ListResponse<Artist> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Artist.FromJToken);
        }
    }
}