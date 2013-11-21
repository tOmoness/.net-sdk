// -----------------------------------------------------------------------
// <copyright file="ArtistProductsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
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
            uri.AppendFormat("creators/{0}/products/", this.ArtistId);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.ArtistId))
            {
                throw new ArgumentNullException("ArtistId", "An artist ID must be supplied");
            }

            // Build querystring parameters...
            var querystring = this.GetPagingParams();
            if (this.Category.HasValue)
            {
                querystring.Add(new KeyValuePair<string, string>(ParamCategory, this.Category.ToString().ToLowerInvariant()));
            }

            if (this.OrderBy.HasValue)
            {
                querystring.Add(new KeyValuePair<string, string>(ParamOrderBy, this.OrderBy.ToString().ToLowerInvariant()));
            }

            if (this.SortOrder.HasValue)
            {
                querystring.Add(new KeyValuePair<string, string>(ParamSortOrder, this.SortOrder.ToString().ToLowerInvariant()));
            }

            RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                querystring,
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, Product.FromJToken, this.Callback)));
        }
    }
}