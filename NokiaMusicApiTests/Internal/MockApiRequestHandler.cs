// -----------------------------------------------------------------------
// <copyright file="MockApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Internal;

namespace Nokia.Music.Phone.Tests
{
    /// <summary>
    /// Returns mocked failure-path responses
    /// </summary>
    internal class MockApiRequestHandler : IApiRequestHandler
    {
        private IApiUriBuilder _uriBuilder;

        private IMusicClientSettings _lastSettings;

        private Dictionary<string, string> _queryStringParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockApiRequestHandler" /> class.
        /// </summary>
        /// <param name="response">The fake response that should be returned for the first request</param>
        public MockApiRequestHandler(FakeResponse response)
        {
            this._uriBuilder = new ApiUriBuilder();
            this.NextFakeResponse = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockApiRequestHandler" /> class.
        /// </summary>
        /// <param name="successResponse">The response body that should be returned for the first request</param>
        public MockApiRequestHandler(byte[] successResponse) : this(FakeResponse.Success(successResponse))
        {
        }

        /// <summary>
        /// Gets or sets the fake response that should be returned for the next request
        /// </summary>
        public FakeResponse NextFakeResponse { private get; set; }

        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        public IApiUriBuilder UriBuilder
        {
            get
            {
                return this._uriBuilder;
            }
        }

        /// <summary>
        /// Gets the Query string params that were passed with the last request
        /// </summary>
        public Dictionary<string, string> LastQueryStringParams
        {
            get { return this._queryStringParams; }
        }

        /// <summary>
        /// Gets the music client settings that were passed with the last request
        /// </summary>
        public IMusicClientSettings LastUsedSettings
        {
            get { return this._lastSettings; }
        }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="settings">The app id.</param>
        /// <param name="pathParams">The path params.</param>
        /// <param name="querystringParams">The querystring params.</param>
        /// <param name="callback">The callback to hit when done.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        public void SendRequestAsync(
                                     ApiMethod method,
                                     IMusicClientSettings settings,
                                     Dictionary<string, string> pathParams,
                                     Dictionary<string, string> querystringParams,
                                     Action<Response<JObject>> callback,
                                     Dictionary<string, string> requestHeaders = null)
        {
            this._lastSettings = settings;
            this._queryStringParams = querystringParams;
            this.NextFakeResponse.DoCallback(callback);
        }
    }
}
