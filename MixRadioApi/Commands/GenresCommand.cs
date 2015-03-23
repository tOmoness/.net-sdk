// -----------------------------------------------------------------------
// <copyright file="GenresCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
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