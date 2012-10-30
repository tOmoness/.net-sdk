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
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.ArtistId))
            {
                throw new ArgumentNullException("ArtistId", "An artist ID must be supplied");
            }

            this.RequestHandler.SendRequestAsync(
                ApiMethod.SimilarArtists,
                this.MusicClientSettings.AppId,
                this.MusicClientSettings.AppCode,
                this.MusicClientSettings.CountryCode,
                new Dictionary<string, string>() { { MusicClientCommand.ParamId, this.ArtistId } },
                new Dictionary<string, string>()
                    {
                        { PagingStartIndex, StartIndex.ToString(CultureInfo.InvariantCulture) },
                        { PagingItemsPerPage, ItemsPerPage.ToString(CultureInfo.InvariantCulture) }
                    },
                (Response<JObject> rawResult) =>
                    {
                        this.CatalogItemResponseHandler<Artist>(rawResult, ArrayNameItems, new JTokenConversionDelegate<Artist>(Artist.FromJToken), Callback);
                    });
        }
    }
}