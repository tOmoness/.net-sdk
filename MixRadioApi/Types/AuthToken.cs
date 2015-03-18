// -----------------------------------------------------------------------
// <copyright file="AuthToken.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Authorization;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents an Access Token Response
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        /// Gets or sets the actual access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets when the token expires.
        /// </summary>
        /// <value>
        /// The seconds until the token expires.
        /// </value>
        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the territory.
        /// </summary>
        /// <value>
        /// The territory.
        /// </value>
        public string Territory { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Restores an AuthToken from json
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns>An AuthToken</returns>
        public static AuthToken FromJson(string jsonString)
        {
            var json = JObject.Parse(jsonString);
            return new AuthToken
            {
                AccessToken = json.Value<string>("access_token"),
                ExpiresUtc = json.Value<DateTime>("expires"),
                RefreshToken = json.Value<string>("refresh_token"),
                Territory = json.Value<string>("territory"),
                UserId = Guid.Parse(json.Value<string>("userid"))
            };
        }

        /// <summary>
        /// Creates an AuthToken from a dictionary of properties returned by
        /// the Xamarin.Auth component
        /// </summary>
        /// <param name="authProperties">The authentication properties.</param>
        /// <returns>An AuthToken</returns>
        public static AuthToken FromXamarinDictionary(Dictionary<string, string> authProperties)
        {
            if (authProperties != null)
            {
                var expiresIn = Convert.ToInt32(authProperties["expires_in"]);
                return new AuthToken
                {
                    AccessToken = authProperties["access_token"],
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(-1).AddSeconds(expiresIn),
                    RefreshToken = authProperties["refresh_token"],
                    UserId = Guid.Parse(authProperties["userid"]),
                    Territory = authProperties["territory"]
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return new JObject(
                new JProperty("access_token", this.AccessToken),
                new JProperty("expires", this.ExpiresUtc),
                new JProperty("refresh_token", this.RefreshToken),
                new JProperty("userid", this.UserId),
                new JProperty("territory", this.Territory)).ToString();
        }

        /// <summary>
        /// Creates an AuthToken from a TokenResponse.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>An AuthToken</returns>
        internal static AuthToken FromTokenResponse(TokenResponse response)
        {
            if (response != null)
            {
                return new AuthToken
                {
                    AccessToken = response.AccessToken,
                    ExpiresUtc = response.ExpiresUtc,
                    RefreshToken = response.RefreshToken,
                    Territory = response.Territory,
                    UserId = response.UserId
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts an AuthToken to a TokenResponse
        /// </summary>
        /// <returns>A TokenResponse</returns>
        internal TokenResponse ToTokenResponse()
        {
            return new TokenResponse
            {
                AccessToken = this.AccessToken,
                ExpiresUtc = this.ExpiresUtc,
                RefreshToken = this.RefreshToken,
                UserId = this.UserId,
                Territory = this.Territory
            };
        }
    }
}
