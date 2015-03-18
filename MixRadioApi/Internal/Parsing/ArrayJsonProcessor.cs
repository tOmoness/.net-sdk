﻿// -----------------------------------------------------------------------
// <copyright file="ArrayJsonProcessor.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;

namespace Nokia.Music.Internal.Parsing
{
    internal class ArrayJsonProcessor : IJsonProcessor
    {
        /// <summary>
        /// Parses a json array
        /// </summary>
        /// <typeparam name="T">The type being parsed</typeparam>
        /// <param name="rawJson">The raw json</param>
        /// <param name="listName">The name of the list</param>
        /// <param name="converter">A delegate that can parse each object of type T</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A list of type T of each of the items in the array
        /// </returns>
        public List<T> ParseList<T>(JToken rawJson, string listName, MusicClientCommand.JTokenConversionDelegate<T> converter, IMusicClientSettings settings)
        {
            var results = new List<T>();

            foreach (JToken item in rawJson.Children())
            {
                T result = converter(item, settings);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results;
        }
    }
}
