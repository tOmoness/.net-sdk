// -----------------------------------------------------------------------
// <copyright file="TestHttpUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tests
{
    /// <summary>
    /// Bad HTTP URI builder for testing ApiRequestHandler
    /// </summary>
    internal class TestHttpUriBuilder : IApiUriBuilder
    {
        private Uri _uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHttpUriBuilder" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public TestHttpUriBuilder(Uri uri)
        {
            this._uri = uri;
        }

        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        public Uri BuildUri(ApiMethod method, IMusicClientSettings settings, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams)
        {
            return this._uri;
        }
    }
}
