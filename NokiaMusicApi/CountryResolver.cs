// -----------------------------------------------------------------------
// <copyright file="CountryResolver.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Request;

namespace Nokia.Music
{
    /// <summary>
    /// The CountryResolver validates a country has availability for the Nokia MixRadio API
    /// </summary>
    public sealed class CountryResolver : ICountryResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        public CountryResolver(string clientId)
            : this(clientId, ApiRequestHandlerFactory.Create())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryResolver" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="requestHandler">The request handler.</param>
        /// <remarks>
        /// Allows custom requestHandler for testing purposes
        /// </remarks>
        internal CountryResolver(string clientId, IApiRequestHandler requestHandler)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ApiCredentialsRequiredException();
            }

            this.ClientId = clientId;
            this.RequestHandler = requestHandler;
        }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        public string ClientId { get; private set; }

        /// <summary>
        /// Gets the request handler in use for testing purposes.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; private set; }

        /// <summary>
        /// Validates that the Nokia MixRadio API is available for a country
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns>
        /// A Response containing whether the API is available or not
        /// </returns>
        public async Task<bool> CheckAvailabilityAsync(string countryCode)
        {
            if (!this.ValidateCountryCode(countryCode))
            {
                throw new InvalidCountryCodeException();
            }

            CountryResolverCommand command = new CountryResolverCommand(this.ClientId, this.RequestHandler)
            {
                CountryCode = countryCode,
                RequestId = new Guid()
            };

            var response = await command.InvokeAsync();
            return response.Result;
        }

        /// <summary>
        /// Validates a country code.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns>A Boolean indicating that the country code is valid</returns>
        private bool ValidateCountryCode(string countryCode)
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
