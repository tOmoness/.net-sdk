// -----------------------------------------------------------------------
// <copyright file="IApiUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nokia.Music.Phone.Commands;

namespace Nokia.Music.Phone.Internal.Request
{
    /// <summary>
    /// Defines the API URI Builder interface
    /// </summary>
    internal interface IApiUriBuilder
    {
        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The client settings.</param>
        /// <param name="queryParams">The querystring.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        Uri BuildUri(MusicClientCommand command, IMusicClientSettings settings, List<KeyValuePair<string, string>> queryParams);
    }
}
