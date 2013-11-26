// -----------------------------------------------------------------------
// <copyright file="SearchCatalogCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Searches the Nokia MixRadio Catalog
    /// </summary>
    /// <typeparam name="TReturnType">The type of the returned object.</typeparam>
    internal abstract class SearchCatalogCommand<TReturnType> : MusicClientCommand<ListResponse<TReturnType>>
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
        /// <typeparam name="T">The type to return</typeparam>
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
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The callback to use when the API call has completed</param>
        protected void InternalSearch<T>(string searchTerm, string genreId, string id, Category? category, string location, string maxdistance, OrderBy? orderBy, SortOrder? sortOrder, int startIndex, int itemsPerPage, JTokenConversionDelegate<T> converter, Action<ListResponse<T>> callback)
        {
            // Build querystring parameters...
            var parameters = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(PagingStartIndex, startIndex.ToString(CultureInfo.InvariantCulture)),
                            new KeyValuePair<string, string>(PagingItemsPerPage, itemsPerPage.ToString(CultureInfo.InvariantCulture))
                        };

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

            if (category.HasValue && category.Value != Types.Category.Unknown)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamCategory, category.Value.ToString().ToLowerInvariant()));
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

            this.RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                parameters,
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler<T>(rawResult, ArrayNameItems, converter, callback)));
        }
    }
}