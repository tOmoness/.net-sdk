// -----------------------------------------------------------------------
// <copyright file="NewReleasesCommand.cs" company="Nokia">
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
    /// Gets a list of new releases
    /// </summary>
    internal sealed class NewReleasesCommand : MusicClientCommand<ListResponse<Product>>
    {
        private string _category;

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
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (string.IsNullOrEmpty(this.GenreId))
            {
                uri.AppendFormat("products/new/{0}/", this._category);
            }
            else
            {
                uri.AppendFormat("genres/{0}/new/{1}/", this.GenreId.ToLowerInvariant(), this._category);
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
        /// Ensures that only a supported category type is used
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
                    throw new ArgumentOutOfRangeException("Category", "Only Album and Track lists are available");
            }
        }
    }
}