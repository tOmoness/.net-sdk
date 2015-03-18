// -----------------------------------------------------------------------
// <copyright file="HttpClientRequestProxy.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nokia.Music.Internal.Request
{
    /// <summary>
    /// Responsible for sending the HTTPClient request
    /// </summary>
    internal class HttpClientRequestProxy : IHttpClientRequestProxy
    {
        /// <summary>
        /// Sends the request
        /// </summary>
        /// <param name="client">The HTTPClient instance</param>
        /// <param name="request">The request information</param>
        /// <param name="activeCancellationToken">The cancellation request</param>
        /// <returns>The http response </returns>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpClient client, HttpRequestMessage request, CancellationToken activeCancellationToken)
        {
            return await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, activeCancellationToken).ConfigureAwait(false);
        }
    }
}
