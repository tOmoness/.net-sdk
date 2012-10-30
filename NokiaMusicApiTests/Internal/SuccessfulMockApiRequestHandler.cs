// -----------------------------------------------------------------------
// <copyright file="SuccessfulMockApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Tests
{
    /// <summary>
    /// Returns mocked happy-path responses
    /// </summary>
    internal class SuccessfulMockApiRequestHandler : IApiRequestHandler
    {
        private const string TestContentType = "application/vnd.nokia.ent.test.json";

        private IApiUriBuilder _uriBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulMockApiRequestHandler" /> class.
        /// </summary>
        public SuccessfulMockApiRequestHandler()
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
            switch (method)
            {
                case ApiMethod.CountryLookup:
                    this.FakeResponse(Resources.country, callback);
                    break;

                case ApiMethod.Search:
                    this.FakeSearchResponse(querystringParams, callback);
                    break;

                case ApiMethod.SimilarArtists:
                    this.FakeResponse(Resources.artist_similar, callback);
                    break;

                case ApiMethod.ArtistProducts:
                case ApiMethod.ProductChart:
                case ApiMethod.ProductNewReleases:
                case ApiMethod.Recommendations:
                    this.FakeResponse(Resources.artist_products, callback);
                    break;

                case ApiMethod.Genres:
                    this.FakeResponse(Resources.genres, callback);
                    break;

                case ApiMethod.MixGroups:
                    this.FakeResponse(Resources.mixgroups, callback);
                    break;

                case ApiMethod.Mixes:
                    this.FakeResponse(Resources.mixes, callback);
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
            Category category = Category.Unknown;

            // See what response to get...
            if (parameters != null && parameters.ContainsKey("category"))
            {
                Enum.TryParse<Category>(parameters["category"], true, out category);
            }

            switch (category)
            {
                case Category.Artist:
                    this.FakeResponse(Resources.search_artists, callback);
                    break;

                default:
                    this.FakeResponse(Resources.search_all, callback);
                    break;
            }
        }

        /// <summary>
        /// Fakes a response from a JSON resource.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="callback">The callback.</param>
        private void FakeResponse(byte[] json, Action<Response<JObject>> callback)
        {
            callback(new Response<JObject>(HttpStatusCode.OK, TestContentType, JObject.Parse(Encoding.UTF8.GetString(json))));
        }
    }
}
