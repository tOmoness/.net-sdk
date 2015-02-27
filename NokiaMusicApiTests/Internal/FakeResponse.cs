// -----------------------------------------------------------------------
// <copyright file="FakeResponse.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Tests.Internal
{
    internal class FakeResponse
    {
        private const string TestContentType = "application/vnd.nokia.ent.test.json";
        private const string ExceptionContent = "ExceptionContent";
        private readonly object _response;

        /// <summary>
        /// Private constructor, use a helper method to create
        /// </summary>
        /// <param name="response">The response to callback with</param>
        private FakeResponse(object response)
        {
            this._response = response;
        }

        public static FakeResponse Success(byte[] successResponse)
        {
            return Success(successResponse, TestContentType);
        }

        public static FakeResponse Success(byte[] successResponse, string contentType)
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.OK, contentType, successResponse != null ? JObject.Parse(Encoding.UTF8.GetString(successResponse)) : null, Guid.Empty));
        }

        public static FakeResponse RawSuccess(string successResponse)
        {
            return new FakeResponse(new Response<string>(HttpStatusCode.OK, TestContentType, successResponse, Guid.Empty));
        }

        public static FakeResponse BadRequest()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.BadRequest, (JObject)null, Guid.Empty));
        }

        public static FakeResponse Unauthorized()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.Unauthorized, (JObject)null, Guid.Empty));
        }

        public static FakeResponse RawUnauthorized()
        {
            return new FakeResponse(new Response<string>(HttpStatusCode.Unauthorized, (string)null, Guid.Empty, true));
        }

        public static FakeResponse Forbidden()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.Forbidden, (JObject)null, Guid.Empty));
        }

        public static FakeResponse NotFound(string response = null)
        {
            if (response != null)
            {
                return new FakeResponse(new Response<JObject>(HttpStatusCode.NotFound, JObject.Parse(response), Guid.Empty));
            }

            return new FakeResponse(new Response<JObject>(HttpStatusCode.NotFound, (JObject)null, Guid.Empty));
        }

        public static FakeResponse InternalServerError(string response = null)
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.InternalServerError, (JObject)null, Guid.Empty));
        }

        public static FakeResponse ConflictServerError(string response = null)
        {
            if (response != null)
            {
                return new FakeResponse(new Response<JObject>(HttpStatusCode.Conflict, new Exception(ExceptionContent), response, Guid.Empty));
            }

            return new FakeResponse(new Response<JObject>(HttpStatusCode.Conflict, (JObject)null, Guid.Empty));
        }

        public static FakeResponse GatewayTimeout()
        {
            return new FakeResponse(new Response<JObject>(HttpStatusCode.GatewayTimeout, (JObject)null, Guid.Empty));
        }

        /// <summary>
        /// Performs the callback specified with the previously queued up response
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <returns>A response</returns>
        public Response<T> GetResponseOf<T>()
        {
            return this._response as Response<T>;
        }
    }
}
