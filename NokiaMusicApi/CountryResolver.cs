// -----------------------------------------------------------------------
// <copyright file="CountryResolver.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal.Request;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// The CountryResolver validates a country has availability for the Nokia Music API
    /// </summary>
    public sealed class CountryResolver : ICountryResolver
    {
        private readonly IApiRequestHandler _requestHandler;
        private readonly CountryResolverCommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="appId">The AppID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The AppCode obtained from api.developer.nokia.com</param>
        /// <param name="requestId">A unique id to associate with this request</param>
        public CountryResolver(string appId, string appCode, Guid? requestId = null)
            : this(appId, appCode, new ApiRequestHandler(new ApiUriBuilder()), requestId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="appId">The App ID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The App Code obtained from api.developer.nokia.com</param>
        /// <param name="requestHandler">The request handler.</param>
        /// <param name="requestId">A unique id to associate with this request.</param>
        /// <remarks>
        /// Allows custom requestHandler for testing purposes
        /// </remarks>
        internal CountryResolver(string appId, string appCode, IApiRequestHandler requestHandler, Guid? requestId = null)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appCode))
            {
                throw new ApiCredentialsRequiredException();
            }

            this._requestHandler = requestHandler;
            this._command = new CountryResolverCommand(appId, appCode, requestHandler);
            if (requestId.HasValue)
            {
                this._command.RequestId = requestId.Value;
            }
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

            this._command.CountryCode = countryCode;
            this._command.Invoke(callback);
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
