// -----------------------------------------------------------------------
// <copyright file="SearchCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Parsing;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Searches the Nokia MixRadio Catalog
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
        /// <returns>An CatalogItem</returns>
        /// <remarks>Internal for testing purposes</remarks>
        internal static MusicItem CreateCatalogItemBasedOnCategory(JToken item)
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
                            return Artist.FromJToken(item);

                        case Types.Category.Album:
                        case Types.Category.Single:
                        case Types.Category.Track:
                            return Product.FromJToken(item);

                        case Types.Category.RadioStation:
                            return Mix.FromJToken(item);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.SearchTerm) && string.IsNullOrEmpty(this.GenreId) && string.IsNullOrEmpty(this.Id) && this.MinBpm == 0 && this.MaxBpm == 0)
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm, Id, genreId, or BPM must be supplied");
            }

            this.InternalSearch<MusicItem>(this.SearchTerm, this.GenreId, this.Id, this.Category, null, null, this.OrderBy, this.SortOrder, this.StartIndex, this.ItemsPerPage, SearchCommand.CreateCatalogItemBasedOnCategory, this.Callback);
        }
    }
}