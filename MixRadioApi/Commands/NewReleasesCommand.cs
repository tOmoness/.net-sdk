// -----------------------------------------------------------------------
// <copyright file="NewReleasesCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Gets a list of new releases
    /// </summary>
    internal sealed class NewReleasesCommand : JsonMusicClientCommand<ListResponse<Product>>
    {
        /// <summary>
        /// Gets or sets the category - only Album and Track lists are available.
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
        internal override void AppendUriPath(StringBuilder uri)
        {
            string category = null;

            switch (this.Category)
            {
                case Category.Album:
                case Category.Track:
                    category = this.Category.ToString().ToLowerInvariant();
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Category", "Only Album and Track lists are available");
            }

            if (string.IsNullOrEmpty(this.GenreId))
            {
                uri.AppendFormat("products/new/{0}/", category);
            }
            else
            {
                uri.AppendFormat("genres/{0}/new/{1}/", this.GenreId.ToLowerInvariant(), category);
            }
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