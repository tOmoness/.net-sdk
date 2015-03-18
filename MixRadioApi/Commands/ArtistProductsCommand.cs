// -----------------------------------------------------------------------
// <copyright file="ArtistProductsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets products by an artist.
    /// </summary>
    internal sealed class ArtistProductsCommand : SearchCatalogCommand<Product>
    {
        /// <summary>
        /// Gets or sets the artist id.
        /// </summary>
        /// <value>
        /// The artist id.
        /// </value>
        public string ArtistId { get; set; }
        
        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (string.IsNullOrEmpty(this.ArtistId))
            {
                throw new ArgumentNullException("ArtistId", "An artist ID must be supplied");
            }

            uri.AppendFormat("creators/{0}/products/", this.ArtistId);
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            var parameters = this.GetPagingParams();

            if (this.Category.HasValue)
            {
                foreach (var value in Enum.GetValues(typeof(Category)))
                {
                    var availableCategory = (Category)value;

                    if (availableCategory == Types.Category.Unknown)
                    {
                        continue;
                    }

                    if ((this.Category & availableCategory) == availableCategory)
                    {
                        parameters.Add(new KeyValuePair<string, string>(
                            ParamCategory,
                            availableCategory.ToString().ToLowerInvariant()));
                    }
                }
            }

            if (this.OrderBy.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamOrderBy, this.OrderBy.ToString().ToLowerInvariant()));
            }

            if (this.SortOrder.HasValue)
            {
                parameters.Add(new KeyValuePair<string, string>(ParamSortOrder, this.SortOrder.ToString().ToLowerInvariant()));
            }

            return parameters;
        }

        internal override ListResponse<Product> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Product.FromJToken);
        }
    }
}