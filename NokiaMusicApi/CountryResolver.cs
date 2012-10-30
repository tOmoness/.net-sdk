// -----------------------------------------------------------------------
// <copyright file="CountryResolver.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// The CountryResolver validates a country has availability for the Nokia Music API
    /// </summary>
    public sealed class CountryResolver : ICountryResolver
    {
        private string _appId;
        private string _appCode;
        private IApiRequestHandler _requestHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="appId">The AppID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The AppCode obtained from api.developer.nokia.com</param>
        public CountryResolver(string appId, string appCode)
            : this(appId, appCode, new ApiRequestHandler(new ApiUriBuilder()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="appId">The App ID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The App Code obtained from api.developer.nokia.com</param>
        /// <param name="requestHandler">The request handler.</param>
        /// <remarks>
        /// Allows custom requestHandler for testing purposes
        /// </remarks>
        internal CountryResolver(string appId, string appCode, IApiRequestHandler requestHandler)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appCode))
            {
                throw new ApiCredentialsRequiredException();
            }

            this._appId = appId;
            this._appCode = appCode;
            this._requestHandler = requestHandler;
        }

        /// <summary>
        /// Gets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler
        {
            get
            {
                return this._requestHandler;
            }
        }

        /// <summary>
        /// Validates that the Nokia Music API is available for a country
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="countryCode">The country code.</param>
        public void CheckAvailability(Action<Response<bool>> callback, string countryCode)
        {
            if (!this.ValidateCountryCode(countryCode))
            {
                throw new InvalidCountryCodeException();
            }

            this.ValidateCallback(callback);

            this.RequestHandler.SendRequestAsync(
                ApiMethod.CountryLookup,
                this._appId,
                this._appCode,
                null,
                null,
                new Dictionary<string, string>() { { "countrycode", countryCode } },
                (Response<JObject> rawResult) =>
                {
                    Response<bool> response = null;

                    // Parse the result if we got one...
                    if (rawResult.StatusCode != null && rawResult.StatusCode.HasValue)
                    {
                        switch (rawResult.StatusCode.Value)
                        {
                            case HttpStatusCode.OK:
                                if (rawResult.Result != null)
                                {
                                    JArray items = rawResult.Result.Value<JArray>("items");
                                    if (items != null && items.Count == 1)
                                    {
                                        response = new Response<bool>(rawResult.StatusCode, true);
                                    }
                                }

                                break;

                            case HttpStatusCode.NotFound:
                                response = new Response<bool>(rawResult.StatusCode, false);
                                break;

                            case HttpStatusCode.Forbidden:
                                response = new Response<bool>(rawResult.StatusCode, new InvalidApiCredentialsException());
                                break;
                        }
                    }

                    // If the API return an expected result, set an error...
                    if (response == null)
                    {
                        response = new Response<bool>(rawResult.StatusCode, new ApiCallFailedException());
                    }

                    if (callback != null)
                    {
                        callback(response);
                    }
                });
        }

        /// <summary>
        /// Checks that a callback has been set
        /// </summary>
        /// <param name="callback">The callback</param>
        internal void ValidateCallback(object callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("A callback must be supplied", "callback");
            }
        }

        /// <summary>
        /// Validates a country code.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns>A Boolean indicating that the country code is valid</returns>
        internal bool ValidateCountryCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                return countryCode.Length == 2;
            }
            else
            {
                return false;
            }
        }
    }
}
