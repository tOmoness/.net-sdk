// -----------------------------------------------------------------------
// <copyright file="MockApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
using Nokia.Music.Tests.Internal;

namespace Nokia.Music.Tests
{
    /// <summary>
    /// Returns mocked failure-path responses
    /// </summary>
    internal class MockApiRequestHandler : IApiRequestHandler
    {
        private IApiUriBuilder _uriBuilder;

        private IMusicClientSettings _lastSettings;

        private List<KeyValuePair<string, string>> _queryString;

        private ResponseInfo _responseInfo;

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

        public DateTime ServerTimeUtc
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        /// Gets the Query string params that were passed with the last request
        /// </summary>
        public List<KeyValuePair<string, string>> LastQueryString
        {
            get { return this._queryString; }
        }

        /// <summary>
        /// Gets the music client settings that were passed with the last request
        /// </summary>
        public IMusicClientSettings LastUsedSettings
        {
            get { return this._lastSettings; }
        }

        public void SetupResponseInfo(ResponseInfo responseInfo)
        {
            this._responseInfo = responseInfo;
        }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The app id.</param>
        /// <param name="querystring">The querystring params.</param>
        /// <param name="callback">The callback to hit when done.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        public void SendRequestAsync<T>(
                                     MusicClientCommand command,
                                     IMusicClientSettings settings,
                                     List<KeyValuePair<string, string>> querystring,
                                     IResponseCallback<T> callback,
                                     Dictionary<string, string> requestHeaders = null)
        {
            this._lastSettings = settings;
            this._queryString = querystring;

            // Ensure URI building is exercised...
            Uri uri = this.UriBuilder.BuildUri(command, settings, querystring);

            // Ensure we call this method to make
            // sure the code gets a run through...
            string body = command.BuildRequestBody();

            if (this._responseInfo != null)
            {
                command.SetAdditionalResponseInfo(this._responseInfo);
            }
            
            this.NextFakeResponse.DoCallback<T>(callback);
        }
    }
}
