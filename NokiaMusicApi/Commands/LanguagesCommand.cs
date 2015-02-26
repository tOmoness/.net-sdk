// -----------------------------------------------------------------------
// <copyright file="LanguagesCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    internal sealed class LanguagesCommand : JsonMusicClientCommand<ListResponse<Language>>
    {
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.Append("languages/");
        }

        internal override ListResponse<Language> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Language.FromJToken);
        }
    }
}