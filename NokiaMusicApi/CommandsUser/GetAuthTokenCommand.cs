// -----------------------------------------------------------------------
// <copyright file="GetAuthTokenCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Authorization;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Gets a bearer token from the API
    /// </summary>
    internal class GetAuthTokenCommand : JsonMusicClientCommand<Response<TokenResponse>>
    {
        internal const string GrantTypeAuthorizationCode = "authorization_code";
        internal const string GrantTypeRefreshToken = "refresh_token";

        protected readonly string Nonce = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        /// <value>
        /// The authorization code.
        /// </value>
        internal string AuthorizationCode { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>
        /// The client id.
        /// </value>
        internal string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        internal string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        internal string RefreshToken { get; set; }

        /// <summary>
        /// Gets the grant type.
        /// </summary>
        /// <value>
        /// The grant type.
        /// </value>
        internal virtual string GrantType
        {
            get
            {
                if (!string.IsNullOrEmpty(this.AuthorizationCode))
                {
                    return GrantTypeAuthorizationCode;
                }

                if (!string.IsNullOrEmpty(this.RefreshToken))
                {
                    return GrantTypeRefreshToken;
                }

                throw new ArgumentNullException("AuthorizationCode", "Expected an AuthorizationCode or RefreshToken");
            }
        }

        internal override string ContentType
        {
            get { return MusicClientCommand.ContentTypeFormPost; }
        }

        internal override bool RequiresCountryCode
        {
            get { return false; }
        }

        internal override bool RequiresEmptyQuerystring
        {
            get { return true; }
        }

        internal override HttpMethod HttpMethod
        {
            get { return HttpMethod.Post; }
        }

        internal override void AppendUriPath(StringBuilder uri)
        {
            uri.AppendFormat("token/");
        }

        internal override string BuildRequestBody()
        {
            return this.BuildParams(false);
        }

        /// <summary>
        /// Builds the request params.
        /// </summary>
        /// <param name="forHeader">if set to <c>true</c> then the value is used for a header.</param>
        /// <returns>A string version of the params</returns>
        internal virtual string BuildParams(bool forHeader)
        {
            var request = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", this.GrantType)
                };

            // Add the auth code if we have one
            if (!string.IsNullOrEmpty(this.AuthorizationCode))
            {
                request.Add(new KeyValuePair<string, string>("code", this.AuthorizationCode));
            }

            // Add the refresh token if we have one
            if (!string.IsNullOrEmpty(this.RefreshToken))
            {
                request.Add(new KeyValuePair<string, string>("refresh_token", this.RefreshToken));
            }

            return this.ConvertParamsToString(request, forHeader);
        }

        /// <summary>
        /// Converts request parameters to a string form.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="forHeader">if set to <c>true</c>, the conversion is for header usage.</param>
        /// <returns>A string version of the parameters</returns>
        internal string ConvertParamsToString(List<KeyValuePair<string, string>> request, bool forHeader)
        {
            var encodedBody = new StringBuilder();

            // We need to sort the parameters alphabetically
            // for when we are generating the header case as it is used
            // for a hash. We might as well do it for both cases rather
            // than complicate the code further...
            var sortedValues = request.Where(val => !string.IsNullOrEmpty(val.Value)).OrderBy(o => o.Key).ToList();

            foreach (var val in sortedValues)
            {
                encodedBody.Append(val.Key);
                encodedBody.Append("=");
                var encodedValue = Uri.EscapeDataString(val.Value);
                encodedBody.Append(forHeader ? encodedValue : encodedValue.Replace("%20", "+"));

                if (sortedValues.Last().Key != val.Key)
                {
                    encodedBody.Append("&");
                }
            }

            var returnBody = encodedBody.ToString();
#if DEBUG
            Debug.WriteLine("Built parameter list for {0}: {1}", forHeader ? "OAuth header" : "request body", returnBody);
#endif
            return returnBody;
        }

        /// <summary>
        /// Constructs the auth header.
        /// </summary>
        /// <returns>An auth header value</returns>
        internal virtual string ConstructAuthHeader()
        {
            if (string.IsNullOrEmpty(this.ClientId) || string.IsNullOrEmpty(this.ClientSecret))
            {
                throw new ArgumentNullException("ClientId or ClientSecret must be set");
            }

            return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(this.ClientId + ":" + this.ClientSecret));
        }

        internal override Task<Dictionary<string, string>> BuildRequestHeadersAsync()
        {
            var requestHeaders = new Dictionary<string, string>
            {
                { "Authorization", this.ConstructAuthHeader() }
            };

            return Task.FromResult(requestHeaders);
        }

        internal override Response<TokenResponse> HandleRawResponse(Response<JObject> rawResponse)
        {
            var response = this.ItemResponseHandler(rawResponse, TokenResponse.FromJToken);

            if (response.Result != null)
            {
                // Set an explicit expiry datetime and allow a bit of buffer when we do it (1 minute)...
                response.Result.ExpiresUtc = this.RequestHandler.ServerTimeUtc.AddSeconds(response.Result.ExpiresIn - 60);
            }

            return response;
        }
    }
}
