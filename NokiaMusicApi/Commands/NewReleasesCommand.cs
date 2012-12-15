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
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        /// <param name="pathParams">The API method parameters</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri, Dictionary<string, string> pathParams)
        {
            if (pathParams != null && pathParams.ContainsKey("category"))
            {
                uri.AppendFormat("products/new/{0}/", pathParams["category"]);
            }
            else
            {
                throw new ArgumentNullException("category");
            }
        }

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
                this,
                this.MusicClientSettings,
                new Dictionary<string, string>() { { "category", Category.ToString().ToLowerInvariant() } },
                null,
                (Response<JObject> rawResult) =>
                    {
                        this.CatalogItemResponseHandler(rawResult, ArrayNameItems, Product.FromJToken, Callback);
                    });
        }
    }
}