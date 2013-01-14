// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    internal abstract class MusicClientCommand : ApiMethod
    {
        internal const string ArrayNameItems = "items";
        internal const string ParamId = "id";
        internal const string ParamCategory = "category";
        internal const string ParamSearchTerm = "q";
        internal const string ParamGenre = "genre";
        internal const string ParamLocation = "location";
        internal const string ParamMaxDistance = "maxdistance";
        internal const string PagingStartIndex = "startindex";
        internal const string PagingItemsPerPage = "itemsperpage";
        internal const string PagingTotal = "total";

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientCommand" /> class.
        /// </summary>
        internal MusicClientCommand()
        {
        }

        /// <summary>
        /// Signifies a method for converting a JToken into a typed object
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>A typed object</returns>
        internal delegate T JTokenConversionDelegate<T>(JToken item);

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        internal IMusicClientSettings MusicClientSettings { get; set; }

        /// <summary>
        /// Gets or sets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected abstract void Execute();
    }
}