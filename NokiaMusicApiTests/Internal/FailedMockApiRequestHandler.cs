// -----------------------------------------------------------------------
// <copyright file="FailedMockApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Tests
{
    /// <summary>
    /// Returns mocked failure-path responses
    /// </summary>
    internal class FailedMockApiRequestHandler : IApiRequestHandler
    {
        private const string TestContentType = "application/vnd.nokia.ent.test.json";

        private IApiUriBuilder _uriBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedMockApiRequestHandler" /> class.
        /// </summary>
        public FailedMockApiRequestHandler()
        {
            this._uriBuilder = new ApiUriBuilder();
        }

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
        /// Makes the API request
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="appCode">The app code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="pathParams">The path params.</param>
        /// <param name="querystringParams">The querystring params.</param>
        /// <param name="callback">The callback to hit when done.</param>
        public void SendRequestAsync(ApiMethod method, string appId, string appCode, string countryCode, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams, Action<Response<JObject>> callback)
        {
            // Check for bad api cred case...
            if (string.Compare(appId, "badkey", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                JObject nullJson = null;
                callback(new Response<JObject>(HttpStatusCode.Forbidden, nullJson));
                return;
            }

            switch (method)
            {
                case ApiMethod.CountryLookup:
                    this.FakeCountryLookupResponse(querystringParams["countrycode"], callback);
                    break;

                case ApiMethod.Search:
                    this.FakeSearchResponse(querystringParams, callback);
                    break;

                default:
                    JObject json = null;
                    callback(new Response<JObject>(HttpStatusCode.InternalServerError, json));
                    break;
            }
        }

        /// <summary>
        /// Fakes the Country Lookup API
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <param name="callback">The callback to hit when done.</param>
        private void FakeCountryLookupResponse(string countryCode, Action<Response<JObject>> callback)
        {
            JObject json = null;

            // See what response to get...
            switch (countryCode.ToLowerInvariant())
            {
                case "gb":
                    callback(new Response<JObject>(HttpStatusCode.GatewayTimeout, json));
                    break;
                default:
                    callback(new Response<JObject>(HttpStatusCode.NotFound, json));
                    break;
            }
        }

        /// <summary>
        /// Fakes the Search API
        /// </summary>
        /// <param name="parameters">The parameters to send.</param>
        /// <param name="callback">The callback to hit when done.</param>
        private void FakeSearchResponse(Dictionary<string, string> parameters, Action<Response<JObject>> callback)
        {
            JObject nullJson = null;
            Category category = Category.Unknown;

            // See what response to get...
            if (parameters != null && parameters.ContainsKey("category"))
            {
                Enum.TryParse<Category>(parameters["category"], true, out category);
            }

            switch (category)
            {
                case Category.Artist:
                    if (parameters != null && parameters.ContainsKey("genre"))
                    {
                        callback(new Response<JObject>(HttpStatusCode.InternalServerError, nullJson));
                    }
                    else
                    {
                        callback(new Response<JObject>(HttpStatusCode.OK, TestContentType, JObject.Parse(Encoding.UTF8.GetString(Resources.search_noresults))));
                    }

                    break;

                default:
                    callback(new Response<JObject>(HttpStatusCode.NotFound, nullJson));
                    break;
            }
        }
    }
}
