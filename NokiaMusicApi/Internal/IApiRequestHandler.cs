// -----------------------------------------------------------------------
// <copyright file="IApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Defines the raw API interface for making requests
    /// </summary>
    internal interface IApiRequestHandler
    {
        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        IApiUriBuilder UriBuilder { get; }

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
        void SendRequestAsync(ApiMethod method, string appId, string appCode, string countryCode, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams, Action<Response<JObject>> callback);
    }
}
