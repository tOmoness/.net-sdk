// -----------------------------------------------------------------------
// <copyright file="GenresCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Gets the available genres
    /// </summary>
    internal sealed class GenresCommand : JsonMusicClientCommand<ListResponse<Genre>>
    {
        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("genres/");
        }

        internal override ListResponse<Genre> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Genre.FromJToken);
        }
    }
}