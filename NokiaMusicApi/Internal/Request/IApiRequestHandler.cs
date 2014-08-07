// -----------------------------------------------------------------------
// <copyright file="IApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nokia.Music.Commands;

namespace Nokia.Music.Internal.Request
{
    /// <summary>
    /// Defines the raw API interface for making requests
    /// </summary>
#if OPEN_INTERNALS
    public
#else
        internal
#endif
    interface IApiRequestHandler
    {
        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        IApiUriBuilder UriBuilder { get; }

        /// <summary>
        /// Gets the server UTC time.
        /// </summary>
        /// <value>
        /// The server UTC time.
        /// </value>
        DateTime ServerTimeUtc { get; }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The queryString parameters.</param>
        /// <param name="rawDataHandler">The convertion handler for the data received.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A response for the API request.</returns>
        Task<Response<T>> SendRequestAsync<T>(
                              MusicClientCommand command,
                              IMusicClientSettings settings,
                              List<KeyValuePair<string, string>> queryParams,
                              Func<string, T> rawDataHandler,
                              Dictionary<string, string> requestHeaders,
                              CancellationToken? cancellationToken);
    }
}
