// -----------------------------------------------------------------------
// <copyright file="NewReleasesCommand.cs" company="Nokia">
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
    /// Gets a list of new releases
    /// </summary>
    internal sealed class NewReleasesCommand : MusicClientCommand<ListResponse<Product>>
    {
        /// <summary>
        /// Gets or sets the category - only Album and Track lists are available.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            switch (Category)
            {
                case Category.Album:
                case Category.Track:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Category", "Only Album and Track lists are available");
            }

            this.RequestHandler.SendRequestAsync(
                ApiMethod.ProductNewReleases,
                this.MusicClientSettings.AppId,
                this.MusicClientSettings.AppCode,
                this.MusicClientSettings.CountryCode,
                new Dictionary<string, string>() { { "category", Category.ToString().ToLowerInvariant() } },
                null,
                (Response<JObject> rawResult) =>
                    {
                        this.CatalogItemResponseHandler(rawResult, ArrayNameItems, Product.FromJToken, Callback);
                    });
        }
    }
}