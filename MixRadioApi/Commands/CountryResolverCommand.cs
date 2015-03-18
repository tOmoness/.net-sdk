// -----------------------------------------------------------------------
// <copyright file="CountryResolverCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the country resolver request
    /// </summary>
    internal class CountryResolverCommand : JsonMusicClientCommand<Response<bool>>
    {
        public CountryResolverCommand(string appId, IApiRequestHandler handler)
        {
            this.ClientSettings = new CountryResolverSettings(appId);
            this.RequestHandler = handler;
        }

        /// <summary>
        /// Gets or sets the supplied country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        ///     We don't want /gb/ in the url
        /// </summary>
        internal override bool RequiresCountryCode
        {
            get { return false; }
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("countrycode", this.CountryCode.ToLowerInvariant())
            };
        }

        internal override Response<bool> HandleRawResponse(Response<JObject> rawResponse)
        {
            // Parse the result if we got one...
            if (rawResponse.Succeeded && rawResponse.StatusCode.HasValue)
            {
                switch (rawResponse.StatusCode.Value)
                {
                    case HttpStatusCode.OK:
                        if (rawResponse.Result != null)
                        {
                            JArray items = rawResponse.Result.Value<JArray>(MusicClientCommand.ArrayNameItems);
                            if (items != null && items.Count == 1)
                            {
                                return new Response<bool>(rawResponse.StatusCode, true, RequestId);
                            }
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (rawResponse.Result != null)
                        {
                            return new Response<bool>(rawResponse.StatusCode, false, RequestId);
                        }

                        break;

                    case HttpStatusCode.Forbidden:
                        return new Response<bool>(rawResponse.StatusCode, new InvalidApiCredentialsException(), null, RequestId);
                }
            }

            // If the API return an expected result, set an error...
            return this.ItemErrorResponseHandler<bool>(rawResponse);
        }

        /// <summary>
        /// Implementation of MusicClientSettings for use with country resolver request
        /// </summary>
        private class CountryResolverSettings : IMusicClientSettings
        {
            private readonly string _clientId;

            public CountryResolverSettings(string clientId)
            {
                this._clientId = clientId;
            }

            public string ClientId
            {
                get { return this._clientId; }
            }

            public string CountryCode
            {
                get { return null; }
            }

            public bool CountryCodeBasedOnRegionInfo
            {
                get { return false; }
            }

            public string Language
            {
                get { return null; }
            }

            public string ApiBaseUrl
            {
                get
                {
                    return MusicClientCommand.DefaultBaseApiUri;
                }
            }

            public string SecureApiBaseUrl
            {
                get
                {
                    return MusicClientCommand.DefaultSecureBaseApiUri;
                }
            }
        }
    }
}
