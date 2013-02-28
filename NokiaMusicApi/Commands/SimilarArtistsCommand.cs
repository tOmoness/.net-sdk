// -----------------------------------------------------------------------
// <copyright file="SimilarArtistsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Request;
using Nokia.Music.Phone.Internal.Response;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets similar artists to the supplied artist
    /// </summary>
    internal sealed class SimilarArtistsCommand : SearchCatalogCommand<Artist>
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
            uri.AppendFormat("creators/{0}/similar/", this.ArtistId);
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

            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                this.GetPagingParams(),
                new JsonResponseCallback(rawResult => this.CatalogItemResponseHandler<Artist>(rawResult, ArrayNameItems, Artist.FromJToken, Callback)));
        }
    }
}