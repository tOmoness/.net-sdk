// -----------------------------------------------------------------------
// <copyright file="FakeResponse.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Tests.Properties;

namespace Nokia.Music.Phone.Tests.Internal
{
    internal class FakeResponse
    {
        private const string TestContentType = "application/vnd.nokia.ent.test.json";
        private readonly Response<JObject> _response;

        /// <summary>
        /// Private constructor, use a helper method to create
        /// </summary>
        /// <param name="response">The response to callback with</param>
        private FakeResponse(Response<JObject> response)
        {
            this._response = response;
        }

        public static FakeResponse InternalServerError()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.InternalServerError, (JObject)null));
        }

        public static FakeResponse Success(byte[] successResponse)
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.OK, TestContentType, JObject.Parse(Encoding.UTF8.GetString(successResponse))));
        }

        public static FakeResponse GatewayTimeout()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.GatewayTimeout, (JObject)null));
        }

        public static FakeResponse NotFound()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.NotFound, (JObject)null));
        }

        public static FakeResponse Forbidden()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.Forbidden, (JObject)null));
        }

        /// <summary>
        /// Performs the callback specified with the previously queued up response
        /// </summary>
        /// <param name="callback">The callback action</param>
        public void DoCallback(Action<Response<JObject>> callback)
        {
            callback(this._response);
        }
    }
}
