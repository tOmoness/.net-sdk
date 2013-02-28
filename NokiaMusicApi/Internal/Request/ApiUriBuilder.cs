// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nokia.Music.Phone.Commands;

namespace Nokia.Music.Phone.Internal.Request
{
    /// <summary>
    /// Defines the real Uri Builder
    /// </summary>
    internal class ApiUriBuilder : IApiUriBuilder
    {
        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The querystring parameters</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when an unknown method is used</exception>
        /// <exception cref="CountryCodeRequiredException">Thrown when a CountryCode is required but not supplied</exception>
        /// <exception cref="ApiCredentialsRequiredException">Thrown when an API Key has not been supplied</exception>
        /// <exception cref="System.ArgumentNullException"></exception>        
        public Uri BuildUri(MusicClientCommand command, IMusicClientSettings settings, List<KeyValuePair<string, string>> queryParams)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Build API url
            StringBuilder url = new StringBuilder();

            url.Append(command.BaseApiUri);
            AddCountryCode(url, command, settings.CountryCode);
            command.AppendUriPath(url);
            this.AppendQueryString(url, settings, queryParams);

            return new Uri(url.ToString());
        }

        /// <summary>
        /// Adds authorisation parameters to the querystring
        /// </summary>
        /// <param name="url">The url being built.</param>
        /// <param name="settings">The music client settings.</param>
        protected virtual void AddAuthorisationParams(StringBuilder url, IMusicClientSettings settings)
        {
            if (string.IsNullOrEmpty(settings.AppId) || string.IsNullOrEmpty(settings.AppCode))
            {
                throw new ApiCredentialsRequiredException();
            }

            url.AppendFormat(@"?app_id={0}&app_code={1}", settings.AppId, settings.AppCode);
        }

        /// <summary>
        /// Validates and adds country code if required
        /// </summary>
        /// <param name="url">The url being built</param>
        /// <param name="command">The command to call.</param>
        /// <param name="countryCode">The country code.</param>
        private static void AddCountryCode(StringBuilder url, MusicClientCommand command, string countryCode)
        {
            if (command.RequiresCountryCode)
            {
                if (string.IsNullOrEmpty(countryCode))
                {
                    throw new CountryCodeRequiredException();
                }

                url.AppendFormat("{0}/", countryCode);
            }
        }

        /// <summary>
        /// Appends the appropriate query string parameters to the url
        /// </summary>
        /// <param name="url">The url being built.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The query string.</param>
        private void AppendQueryString(StringBuilder url, IMusicClientSettings settings, List<KeyValuePair<string, string>> queryParams)
        {
            // Add required parameters
            this.AddAuthorisationParams(url, settings);
            url.AppendFormat("&domain=music");

            // Add other parameters...
            if (queryParams != null)
            {
                foreach (KeyValuePair<string, string> pair in queryParams)
                {
                    url.AppendFormat("&{0}={1}", pair.Key, pair.Value);
                }
            }
        }
    }
}
