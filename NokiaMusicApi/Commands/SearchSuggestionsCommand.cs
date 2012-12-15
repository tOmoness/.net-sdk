// -----------------------------------------------------------------------
// <copyright file="SearchSuggestionsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
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
        /// <param name="pathParams">The API method parameters</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri, Dictionary<string, string> pathParams)
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

            RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                null,
                new Dictionary<string, string>
                    {
                        { ParamSearchTerm, this.SearchTerm },
                        { "maxitems", this.ItemsPerPage.ToString() }
                    },
                rawResult => this.CatalogItemResponseHandler(rawResult, ArrayNameResults, ExtractStringFromJToken, this.Callback));
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