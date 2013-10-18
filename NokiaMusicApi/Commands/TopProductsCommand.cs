// -----------------------------------------------------------------------
// <copyright file="TopProductsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Gets a chart
    /// </summary>
    internal sealed class TopProductsCommand : SearchCatalogCommand<Product>
    {
        /// <summary>
        /// Gets or sets the genre ID to get results for.
        /// </summary>
        public string GenreId { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            string category = null;

            if (this.Category != null && this.Category.HasValue)
            {
                switch (this.Category.Value)
                {
                    case Types.Category.Album:
                    case Types.Category.Track:
                        category = this.Category.ToString().ToLowerInvariant();
                        break;
                }
            }

            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentOutOfRangeException("Category", "Only Album and Track charts are available");
            }

            if (string.IsNullOrEmpty(this.GenreId))
            {
                uri.AppendFormat("products/charts/{0}/", category);
            }
            else
            {
                uri.AppendFormat("genres/{0}/charts/{1}/", this.GenreId, category);
            }
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                this.GetPagingParams(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, ArrayNameItems, Product.FromJToken, Callback)));
        }
    }
}