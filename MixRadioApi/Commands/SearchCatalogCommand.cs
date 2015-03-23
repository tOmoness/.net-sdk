// -----------------------------------------------------------------------
// <copyright file="SearchCatalogCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MixRadio.Types;

namespace MixRadio.Commands
{
    /// <summary>
    /// Searches the MixRadio Catalog
    /// </summary>
    /// <typeparam name="TReturnType">The type of the returned object.</typeparam>
    internal abstract class SearchCatalogCommand<TReturnType> : JsonMusicClientCommand<ListResponse<TReturnType>>
    {
        /// <summary>
        /// Gets or sets the order by field.
        /// </summary>
        public OrderBy? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public SortOrder? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Searches for items
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="genreId">The genre to filter the results by.</param>
        /// <param name="id">An artist or product id.</param>
        /// <param name="category">The category to filter the results by.</param>
        /// <param name="location">The location to filter the results by.</param>
        /// <param name="maxdistance">The max distance from the location to to filter the results by.</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items to fetch.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A response</returns>
        protected List<KeyValuePair<string, string>> BuildQueryStringParams(string searchTerm, string genreId, string id, Category? category, string location, string maxdistance, OrderBy? orderBy, SortOrder? sortOrder, int startIndex, int itemsPerPage)
        {
            // Build querystring parameters...
            var parameters = this.GetPagingParams();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamSearchTerm, searchTerm));
            }

            if (!string.IsNullOrEmpty(genreId))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamGenre, genreId));
            }

            if (!string.IsNullOrEmpty(id))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamId, id));
            }

            if (category.HasValue && category != Types.Category.Unknown)
            {
                foreach (var value in Enum.GetValues(typeof(Category)))
                {
                    var availableCategory = (Category)value;

                    if (availableCategory == Types.Category.Unknown)
                    {
                        continue;
                    }

                    if ((category & availableCategory) == availableCategory)
                    {
                        parameters.Add(new KeyValuePair<string, string>(
                            ParamCategory,
                            availableCategory.ToString().ToLowerInvariant()));
                    }
                }
            }

            if (orderBy.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamOrderBy, orderBy.Value.ToString().ToLowerInvariant()));
            }

            if (sortOrder.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamSortOrder, sortOrder.Value.ToString().ToLowerInvariant()));
            }

            if (!string.IsNullOrEmpty(location))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamLocation, location));
            }

            if (!string.IsNullOrEmpty(maxdistance))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamMaxDistance, maxdistance));
            }

            return parameters;
        }
    }
}