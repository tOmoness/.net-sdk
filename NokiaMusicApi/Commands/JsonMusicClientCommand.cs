// -----------------------------------------------------------------------
// <copyright file="JsonMusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the Music Client Json Command base class
    /// </summary>
    /// <typeparam name="TResult">The type of the returned object.</typeparam>
    internal abstract class JsonMusicClientCommand<TResult> : MusicClientCommand<JObject, TResult>
        where TResult : Response
    {
        internal override JObject HandleRawData(string rawData)
        {
            if (!string.IsNullOrEmpty(rawData))
            {
                return ParseHelper.ParseWithDate(rawData);
            }

            return null;
        }
    }
}