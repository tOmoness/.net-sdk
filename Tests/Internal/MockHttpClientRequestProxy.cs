// -----------------------------------------------------------------------
// <copyright file="MockHttpClientRequestProxy.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MixRadio.Internal.Request;

namespace MixRadio.Tests.Internal
{
    public class MockHttpClientRequestProxy : IHttpClientRequestProxy
    {
        private Exception _ex;

        public HttpRequestMessage RequestMessage { get; private set; }
        
        public HttpClient Client { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public void SetupException(Exception ex)
        {
            this._ex = ex;
        }

        public Task<HttpResponseMessage> SendRequestAsync(HttpClient client, HttpRequestMessage request, CancellationToken activeCancellationToken)
        {
            if (this._ex != null)
            {
                throw this._ex;
            }

            this.RequestMessage = request;
            this.Client = client;
            this.CancellationToken = activeCancellationToken;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
