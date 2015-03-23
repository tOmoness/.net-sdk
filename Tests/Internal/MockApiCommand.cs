// -----------------------------------------------------------------------
// <copyright file="MockApiCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net.Http;
using MixRadio;
using MixRadio.Commands;
using Newtonsoft.Json.Linq;

namespace MixRadio.Tests.Internal
{
    internal class MockApiCommand : JsonMusicClientCommand<Response<JObject>>
    {
        private readonly string _body;
        private readonly HttpMethod _httpMethod;
        private bool _gzipRequest;

        public MockApiCommand(string body = "", HttpMethod httpMethod = null, bool gzipRequest = false)
        {
            this._body = body;
            this._httpMethod = httpMethod ?? HttpMethod.Post;
            this._gzipRequest = gzipRequest;
        }

        internal override HttpMethod HttpMethod
        {
            get
            {
                return this._httpMethod;
            }
        }

        internal override bool GzipRequestBody
        {
            get 
            { 
                return this._gzipRequest;
            }
        }

        internal override string ContentType
        {
            get 
            { 
                return "text/xml";
            }
        }

        internal override Response<JObject> HandleRawResponse(Response<JObject> rawResponse)
        {
            return rawResponse;
        }

        internal override string BuildRequestBody()
        {
            return this._body;
        }
    }
}
