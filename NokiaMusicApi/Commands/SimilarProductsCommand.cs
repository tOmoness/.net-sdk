// -----------------------------------------------------------------------
// <copyright file="SimilarProductsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Parsing;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
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
            uri.AppendFormat("products/{0}/similar", this.ProductId);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.ProductId))
            {
                throw new ArgumentNullException("ProductId", "A product ID must be supplied");
            }

            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                this.GetPagingParams(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler<Product>(rawResult, ArrayNameItems, Product.FromJToken, Callback)));
        }
    }
}