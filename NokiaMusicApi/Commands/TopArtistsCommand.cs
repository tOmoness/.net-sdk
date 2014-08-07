// -----------------------------------------------------------------------
// <copyright file="TopArtistsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets top artists
    /// </summary>
    internal sealed class TopArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the genre ID to get results for.
        /// </summary>
        public string GenreId { get; set; }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return this.BuildQueryStringParams(null, this.GenreId, null, Types.Category.Artist, null, null, null, null, this.StartIndex, this.ItemsPerPage);
        }

        internal override ListResponse<Artist> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Artist.FromJToken);
        }
    }
}