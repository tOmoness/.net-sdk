// -----------------------------------------------------------------------
// <copyright file="TokenResponse.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

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
        /// Gets or sets when the token expires - this is derived from ExpiresIn.
        /// </summary>
        /// <value>
        /// The seconds until the token expires.
        /// </value>
        [JsonProperty("expires")]
        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets when the token expires.
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
        /// Gets or sets the yielded scopes from the response
        /// </summary>
        [JsonProperty("scope")]
        [JsonConverter(typeof(ScopeJsonConverter))]
        public Scope Scopes { get; set; }

        /// <summary>
        /// Deserializes from JSON
        /// </summary>
        /// <param name="item">The json</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// An AccessToken instance
        /// </returns>
        internal static TokenResponse FromJToken(JToken item, IMusicClientSettings settings)
        {
            return JsonConvert.DeserializeObject<TokenResponse>(item.ToString());
        }

        /// <summary>
        /// Sets an explicit expiry datetime and allow a bit of buffer when we do it (1 minute)...
        /// </summary>
        /// <param name="serverTimeUtc">The current server time</param>
        internal void UpdateExpiresUtc(DateTime serverTimeUtc)
        {
            this.ExpiresUtc = serverTimeUtc.AddSeconds(this.ExpiresIn - 60);
        }

        /// <summary>
        /// Converts this object to a JSON representation
        /// </summary>
        /// <returns>JSON representation</returns>
        internal JToken ToJToken()
        {
            return JToken.Parse(JsonConvert.SerializeObject(this));
        }

        internal class ScopeJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Scope);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var array = serializer.Deserialize(reader) as JArray;

                if (array == null)
                {
                    // Scope could not be parsed
                    return Scope.None;
                }

                string scopeList = string.Join(",", array.Select(x => x.Value<string>().Replace("_", string.Empty)));

                Scope output;
                if (!Enum.TryParse(scopeList, true, out output))
                {
                    output = Scope.None;
                }

                return output;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var scope = (Scope)value;
                var serialisedScopes = scope.AsStringParams().ToArray<object>();
                serializer.Serialize(writer, new JArray(serialisedScopes));
            }
        }
    }
}
