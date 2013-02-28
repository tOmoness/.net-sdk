// -----------------------------------------------------------------------
// <copyright file="SearchCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal.Parsing;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Searches the Nokia Music Catalog
    /// </summary>
    internal sealed class SearchCommand : SearchCatalogCommand<MusicItem>
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public Category? Category { get; set; }

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
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm must be supplied");
            }

            this.InternalSearch<MusicItem>(this.SearchTerm, null, this.Category, null, null, this.StartIndex, this.ItemsPerPage, SearchCommand.CreateCatalogItemBasedOnCategory, this.Callback);
        }
    }
}