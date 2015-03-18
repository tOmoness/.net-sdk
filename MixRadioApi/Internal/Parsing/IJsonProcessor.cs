// -----------------------------------------------------------------------
// <copyright file="IJsonProcessor.cs" company="Nokia">
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
    /// Provides a common interface for parsing different kinds of lists
    /// </summary>
    internal interface IJsonProcessor
    {
        /// <summary>
        /// Parses a particular type of Json list
        /// </summary>
        /// <typeparam name="T">The type being parsed</typeparam>
        /// <param name="rawJson">The raw json</param>
        /// <param name="listName">The name of the list if appropriate eg. items</param>
        /// <param name="converter">A delegate that can parse each object of type T</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A list of type T
        /// </returns>
        List<T> ParseList<T>(JToken rawJson, string listName, MusicClientCommand.JTokenConversionDelegate<T> converter, IMusicClientSettings settings);
    }
}
