// -----------------------------------------------------------------------
// <copyright file="TestHttpUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Request;

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
        /// <param name="command">The method to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        public Uri BuildUri(MusicClientCommand command, IMusicClientSettings settings, List<KeyValuePair<string, string>> querystringParams)
        {
            return this._uri;
        }
    }
}
