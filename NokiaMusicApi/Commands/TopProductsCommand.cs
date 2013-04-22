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
        private string _category;

        /// <summary>
        /// Gets or sets the category - only Album and Track charts are available.
        /// </summary>
        public Category Category { get; set; }

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
            if (string.IsNullOrEmpty(this.GenreId))
            {
                uri.AppendFormat("products/charts/{0}/", this._category);
            }
            else
            {
                uri.AppendFormat("genres/{0}/charts/{1}/", this.GenreId, this._category);
            }
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.ValidateCategory();

            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                this.GetPagingParams(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, ArrayNameItems, Product.FromJToken, Callback)));
        }

        /// <summary>
        /// Ensures that the supplied category is one of the supported types
        /// </summary>
        private void ValidateCategory()
        {
            switch (this.Category)
            {
                case Category.Album:
                case Category.Track:
                    this._category = this.Category.ToString().ToLowerInvariant();
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Category", "Only Album and Track charts are available");
            }
        }
    }
}