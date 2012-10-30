// -----------------------------------------------------------------------
// <copyright file="ArtistProductsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
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
        /// Gets or sets the category.
        /// </summary>
        public Category? Category { get; set; }

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
            var querystring = new Dictionary<string, string>
                                  {
                                      { PagingStartIndex, StartIndex.ToString(CultureInfo.InvariantCulture) },
                                      { PagingItemsPerPage, ItemsPerPage.ToString(CultureInfo.InvariantCulture) }
                                  };

            if (this.Category != null)
            {
                querystring.Add(MusicClientCommand.ParamCategory, this.Category.ToString().ToLowerInvariant());
            }

            RequestHandler.SendRequestAsync(
                ApiMethod.ArtistProducts,
                MusicClientSettings.AppId,
                MusicClientSettings.AppCode,
                MusicClientSettings.CountryCode,
                new Dictionary<string, string>
                    {
                        { MusicClientCommand.ParamId, this.ArtistId }
                    },
                querystring,
                rawResult => this.CatalogItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, Product.FromJToken, this.Callback));
        }
    }
}