// -----------------------------------------------------------------------
// <copyright file="SearchSuggestionsCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MixRadio.Internal;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
{
    /// <summary>
    /// Gets suggestions for a search term
    /// </summary>
    internal sealed class SearchSuggestionsCommand : JsonMusicClientCommand<ListResponse<string>>
    {
        private const string ArrayNameResults = "results";

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict to artist suggestions
        /// </summary>
        public bool SuggestArtists { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (this.SuggestArtists)
            {
                uri.Append("suggestions/creators/");
            }
            else
            {
                uri.Append("suggestions/");
            }
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                throw new ArgumentNullException("SearchTerm", "A search term must be supplied");
            }

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(ParamSearchTerm, this.SearchTerm),
                new KeyValuePair<string, string>(ParamMaxItems, this.ItemsPerPage.ToString(CultureInfo.InvariantCulture))
            };
        }

        internal override ListResponse<string> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, ArrayNameResults, ExtractStringFromJToken);
        }

        /// <summary>
        /// Extracts a
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// Returns the string value
        /// </returns>
        private static string ExtractStringFromJToken(JToken item, IMusicClientSettings settings)
        {
            return item.Value<string>();
        }
    }
}