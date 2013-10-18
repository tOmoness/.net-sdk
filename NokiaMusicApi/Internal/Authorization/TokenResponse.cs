// -----------------------------------------------------------------------
// <copyright file="TokenResponse.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Represents an Access Token Response
    /// </summary>
#if OPEN_INTERNALS
    public sealed class TokenResponse
#else
    internal sealed class TokenResponse
#endif
    {
        /// <summary>
        /// Gets or sets the actual access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets when the token expires.
        /// </summary>
        /// <value>
        /// The seconds until the token expires.
        /// </value>
        [JsonProperty("expires")]
        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets when the token expires - this is derived from ExpiresIn.
        /// </summary>
        /// <value>
        /// The expires date time.
        /// </value>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the territory.
        /// </summary>
        /// <value>
        /// The territory.
        /// </value>
        [JsonProperty("territory")]
        public string Territory { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [JsonProperty("userid")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Deserializes from JSON
        /// </summary>
        /// <param name="item">The json</param>
        /// <returns>An AccessToken instance</returns>
        internal static TokenResponse FromJToken(JToken item)
        {
            return JsonConvert.DeserializeObject<TokenResponse>(item.ToString());
        }

        /// <summary>
        /// Converts this object to a JSON representation
        /// </summary>
        /// <returns>JSON representation</returns>
        internal JToken ToJToken()
        {
            return JToken.Parse(JsonConvert.SerializeObject(this));
        }
    }
}
