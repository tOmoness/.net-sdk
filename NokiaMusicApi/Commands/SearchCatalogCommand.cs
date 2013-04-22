// -----------------------------------------------------------------------
// <copyright file="SearchCatalogCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Searches the Nokia Music Catalog
    /// </summary>
    /// <typeparam name="TReturnType">The type of the returned object.</typeparam>
    internal abstract class SearchCatalogCommand<TReturnType> : MusicClientCommand<ListResponse<TReturnType>>
    {
        /// <summary>
        /// Searches for items
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="genreId">The genre to filter the results by.</param>
        /// <param name="category">The category to filter the results by.</param>
        /// <param name="location">The location to filter the results by.</param>
        /// <param name="maxdistance">The max distance from the location to to filter the results by.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The callback to use when the API call has completed</param>
        protected void InternalSearch<T>(string searchTerm, string genreId, Category? category, string location, string maxdistance, int startIndex, int itemsPerPage, JTokenConversionDelegate<T> converter, Action<ListResponse<T>> callback)
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

            if (category != null && category.Value != Category.Unknown)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamCategory, category.ToString().ToLowerInvariant()));
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
                this.MusicClientSettings,
                parameters,
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler<T>(rawResult, ArrayNameItems, converter, callback)));
        }
    }
}