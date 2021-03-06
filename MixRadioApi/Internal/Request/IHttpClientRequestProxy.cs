﻿// -----------------------------------------------------------------------
// <copyright file="IHttpClientRequestProxy.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MixRadio.Internal.Request
{
    /// <summary>
    /// Abstracts the sending of the HTTPClient request for testability
    /// </summary>
    #if OPEN_INTERNALS
        public
#else
    internal
#endif
    interface IHttpClientRequestProxy
    {
        /// <summary>
        /// Sends the request
        /// </summary>
        /// <param name="client">The HTTPClient instance</param>
        /// <param name="request">The request information</param>
        /// <param name="activeCancellationToken">The cancellation request</param>
        /// <returns>The http response </returns>
        Task<HttpResponseMessage> SendRequestAsync(HttpClient client, HttpRequestMessage request, CancellationToken activeCancellationToken);
    }
}
