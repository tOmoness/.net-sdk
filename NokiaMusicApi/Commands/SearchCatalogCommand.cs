// -----------------------------------------------------------------------
// <copyright file="SearchCatalogCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
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
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The callback to use when the API call has completed</param>
        protected void InternalSearch<T>(string searchTerm, string genreId, Category? category, int startIndex, int itemsPerPage, JTokenConversionDelegate<T> converter, Action<ListResponse<T>> callback)
        {
            // Build querystring parameters...
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { PagingStartIndex, startIndex.ToString() }, { PagingItemsPerPage, itemsPerPage.ToString() } };

            if (category != null && category.HasValue && category.Value != Category.Unknown)
            {
                parameters.Add(MusicClientCommand.ParamCategory, category.ToString().ToLowerInvariant());
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                parameters.Add(MusicClientCommand.ParamSearchTerm, searchTerm);
            }

            if (!string.IsNullOrEmpty(genreId))
            {
                parameters.Add(MusicClientCommand.ParamGenre, genreId);
            }

            this.RequestHandler.SendRequestAsync(
                ApiMethod.Search,
                this.MusicClientSettings.AppId,
                this.MusicClientSettings.AppCode,
                this.MusicClientSettings.CountryCode,
                null,
                parameters,
                (Response<JObject> rawResult) =>
                {
                    this.CatalogItemResponseHandler<T>(rawResult, ArrayNameItems, converter, callback);
                });
        }
    }
}