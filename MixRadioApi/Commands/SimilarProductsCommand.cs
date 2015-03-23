// -----------------------------------------------------------------------
// <copyright file="SimilarProductsCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
{
    /// <summary>
    ///  Gets similar products to the supplied product
    /// </summary>
    internal sealed class SimilarProductsCommand : SearchCatalogCommand<Product>
    {
        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (string.IsNullOrEmpty(this.ProductId))
            {
                throw new ArgumentNullException("ProductId", "A product ID must be supplied");
            }

            uri.AppendFormat("products/{0}/similar", this.ProductId);
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return this.GetPagingParams();
        }

        internal override ListResponse<Product> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Product.FromJToken);
        }
    }
}