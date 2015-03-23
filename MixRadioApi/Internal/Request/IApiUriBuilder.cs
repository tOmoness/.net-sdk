// -----------------------------------------------------------------------
// <copyright file="IApiUriBuilder.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MixRadio.Commands;

namespace MixRadio.Internal.Request
{
    /// <summary>
    /// Defines the API URI Builder interface
    /// </summary>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
    interface IApiUriBuilder
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
