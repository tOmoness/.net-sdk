// -----------------------------------------------------------------------
// <copyright file="SingleRawItemJsonProcessor.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Commands;

namespace Nokia.Music.Phone.Internal.Parsing
{
    /// <summary>
    /// Processor for a raw item where we treat the whole json as a single item
    /// </summary>
    internal sealed class SingleRawItemJsonProcessor : IJsonProcessor
    {
        public List<T> ParseList<T>(JToken rawJson, string listName, MusicClientCommand.JTokenConversionDelegate<T> converter)
        {
            List<T> results = new List<T>();
            T result = converter(rawJson);
            if (result != null)
            {
                results.Add(result);
            }

            return results;
        }
    }
}
