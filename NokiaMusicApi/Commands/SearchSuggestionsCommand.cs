// -----------------------------------------------------------------------
// <copyright file="SearchSuggestionsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets suggestions for a search term
    /// </summary>
    internal sealed class SearchSuggestionsCommand : MusicClientCommand<ListResponse<string>>
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

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                throw new ArgumentNullException("SearchTerm", "A search term must be supplied");
            }

            var queryStringParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(ParamSearchTerm, this.SearchTerm),
                    new KeyValuePair<string, string>(ParamMaxItems, this.ItemsPerPage.ToString(CultureInfo.InvariantCulture))
                };

            RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                queryStringParams,
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, ArrayNameResults, ExtractStringFromJToken, this.Callback)));
        }

        /// <summary>
        /// Extracts a 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the string value</returns>
        private static string ExtractStringFromJToken(JToken item)
        {
            return item.Value<string>();
        }
    }
}