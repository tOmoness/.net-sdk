// -----------------------------------------------------------------------
// <copyright file="IApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal.Response;

namespace Nokia.Music.Phone.Internal.Request
{
    /// <summary>
    /// Defines the raw API interface for making requests
    /// </summary>
    internal interface IApiRequestHandler
    {
        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        IApiUriBuilder UriBuilder { get; }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The queryString parameters.</param>
        /// <param name="callback">The callback to hit when done.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        /// <exception cref="System.ArgumentNullException">Thrown when no callback is specified</exception>
        void SendRequestAsync<T>(
                              MusicClientCommand command,
                              IMusicClientSettings settings,
                              List<KeyValuePair<string, string>> queryParams,
                              IResponseCallback<T> callback,
                              Dictionary<string, string> requestHeaders = null);
    }
}
