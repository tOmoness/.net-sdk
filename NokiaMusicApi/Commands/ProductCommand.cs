// -----------------------------------------------------------------------
// <copyright file="ProductCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Internal.Parsing;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets a specific product.
    /// </summary>
    internal class ProductCommand : MusicClientCommand<Response<Product>>
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
            uri.AppendFormat("products/{0}/", this.ProductId);
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

            RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                null,
                new JsonResponseCallback(rawResult => this.ItemResponseHandler<Product>(rawResult, Product.FromJToken, this.Callback)));
        }
    }
}