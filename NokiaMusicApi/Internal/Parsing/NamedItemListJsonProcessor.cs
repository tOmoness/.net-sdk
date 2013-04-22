// -----------------------------------------------------------------------
// <copyright file="NamedItemListJsonProcessor.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;

namespace Nokia.Music.Internal.Parsing
{
    /// <summary>
    /// The typical parsing mechanism for json list responses
    /// </summary>
    internal sealed class NamedItemListJsonProcessor : IJsonProcessor
    {
        /// <summary>
        /// Parses a named json list.
        /// </summary>
        /// <typeparam name="T">The type being parsed</typeparam>
        /// <param name="rawJson">The raw json</param>
        /// <param name="listName">The name of the list if appropriate eg. "items"</param>
        /// <param name="converter">A delegate that can parse each object of type T</param>
        /// <returns>A list of type T</returns>
        public List<T> ParseList<T>(JToken rawJson, string listName, MusicClientCommand.JTokenConversionDelegate<T> converter)
        {
            List<T> results = new List<T>();
            JArray items = rawJson.Value<JArray>(listName);
            if (items != null)
            {
                foreach (JToken item in rawJson.Value<JArray>(listName))
                {
                    T result = converter(item);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}
