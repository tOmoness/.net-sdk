// -----------------------------------------------------------------------
// <copyright file="ProductCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets a specific product.
    /// </summary>
    internal class ProductCommand : JsonMusicClientCommand<Response<Product>>
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

            uri.AppendFormat("products/{0}/", this.ProductId);
        }

        internal override Response<Product> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ItemResponseHandler(rawResponse, Product.FromJToken);
        }
    }
}