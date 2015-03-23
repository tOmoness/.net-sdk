// -----------------------------------------------------------------------
// <copyright file="SearchCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MixRadio.Internal;
using MixRadio.Internal.Parsing;
using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
{
    /// <summary>
    /// Searches the MixRadio Catalog
    /// </summary>
    internal sealed class SearchCommand : SearchCatalogCommand<MusicItem>
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the genre
        /// </summary>
        public string GenreId { get; set; }

        /// <summary>
        /// Gets or sets the artist or product id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the minimum BPM.
        /// </summary>
        /// <value>
        /// The minimum BPM.
        /// </value>
        public int MinBpm { get; set; }

        /// <summary>
        /// Gets or sets the maximum BPM.
        /// </summary>
        /// <value>
        /// The maximum BPM.
        /// </value>
        public int MaxBpm { get; set; }

        /// <summary>
        /// Creates an CatalogItem based on it's category field
        /// </summary>
        /// <param name="item">The JSON item</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// An CatalogItem
        /// </returns>
        /// <remarks>
        /// Internal for testing purposes
        /// </remarks>
        internal static MusicItem CreateCatalogItemBasedOnCategory(JToken item, IMusicClientSettings settings = null)
        {
            if (item != null)
            {
                JToken resultCategory = item[ParamCategory];
                if (resultCategory != null)
                {
                    Category itemCategory = ParseHelper.ParseEnumOrDefault<Category>(resultCategory.Value<string>(ParamId));
                    switch (itemCategory)
                    {
                        case Types.Category.Artist:
                            return Artist.FromJToken(item, settings);

                        case Types.Category.Album:
                        case Types.Category.Single:
                        case Types.Category.Track:
                            return Product.FromJToken(item, settings);

                        case Types.Category.RadioStation:
                            return Mix.FromJToken(item, settings);
                    }
                }
            }

            return null;
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            if (string.IsNullOrEmpty(this.SearchTerm) && string.IsNullOrEmpty(this.GenreId) && string.IsNullOrEmpty(this.Id) && this.MinBpm == 0 && this.MaxBpm == 0)
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm, Id, genreId, or BPM must be supplied");
            }

            return this.BuildQueryStringParams(this.SearchTerm, this.GenreId, this.Id, this.Category, null, null, this.OrderBy, this.SortOrder, this.StartIndex, this.ItemsPerPage);
        }

        internal override ListResponse<MusicItem> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, SearchCommand.CreateCatalogItemBasedOnCategory);
        }
    }
}