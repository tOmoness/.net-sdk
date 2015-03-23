// -----------------------------------------------------------------------
// <copyright file="JsonMusicClientCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Internal.Parsing;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
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