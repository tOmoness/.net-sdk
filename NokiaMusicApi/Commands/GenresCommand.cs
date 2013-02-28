// -----------------------------------------------------------------------
// <copyright file="GenresCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Response;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets the available genres
    /// </summary>
    internal sealed class GenresCommand : MusicClientCommand<ListResponse<Genre>>
    {
        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("genres/");
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                null,
                new JsonResponseCallback(rawResult => this.CatalogItemResponseHandler(rawResult, ArrayNameItems, Genre.FromJToken, Callback)));
        }
    }
}