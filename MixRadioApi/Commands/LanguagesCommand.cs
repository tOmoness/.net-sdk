// -----------------------------------------------------------------------
// <copyright file="LanguagesCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
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