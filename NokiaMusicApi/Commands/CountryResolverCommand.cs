// -----------------------------------------------------------------------
// <copyright file="CountryResolverCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the country resolver request
    /// </summary>
    internal class CountryResolverCommand : MusicClientCommand<Response<bool>>
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

        internal override bool RequiresCountryCode
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the command should throw upon error responses.
        /// <remarks>This will be the default behaviour in the next major version of the API, but is experimental and only used in the CountryResolver class for now</remarks>
        /// </summary>
        internal override bool ThrowOnError
        {
            get
            {
                return true;
            }
        }

        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("countrycode", this.CountryCode.ToLowerInvariant()) },
                new JsonResponseCallback(
                (Response<JObject> rawResult) =>
                {
                    Response<bool> response = null;

                    // Parse the result if we got one...
                    if (rawResult.StatusCode != null)
                    {
                        switch (rawResult.StatusCode.Value)
                        {
                            case HttpStatusCode.OK:
                                if (rawResult.Result != null)
                                {
                                    JArray items = rawResult.Result.Value<JArray>(MusicClientCommand.ArrayNameItems);
                                    if (items != null && items.Count == 1)
                                    {
                                        response = new Response<bool>(rawResult.StatusCode, true, RequestId);
                                    }
                                }

                                break;

                            case HttpStatusCode.NotFound:
                                if (rawResult.Result != null)
                                {
                                    response = new Response<bool>(rawResult.StatusCode, false, RequestId);
                                }

                                break;

                            case HttpStatusCode.Forbidden:
                                response = new Response<bool>(rawResult.StatusCode, new InvalidApiCredentialsException(), null, RequestId);
                                break;
                        }
                    }

                    // If the API return an expected result, set an error...
                    if (response == null)
                    {
                        response = new Response<bool>(rawResult.StatusCode, new ApiCallFailedException(), null, RequestId);
                    }

                    if (Callback != null)
                    {
                        Callback(response);
                    }
                }));
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
        }
    }
}
