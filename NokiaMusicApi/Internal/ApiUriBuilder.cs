// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Defines the real Uri Builder
    /// </summary>
    internal class ApiUriBuilder : IApiUriBuilder
    {
        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when an unknown method is used</exception>
        /// <exception cref="CountryCodeRequiredException">Thrown when a CountryCode is required but not supplied</exception>
        /// <exception cref="ApiCredentialsRequiredException">Thrown when an API Key has not been supplied</exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Uri BuildUri(ApiMethod method, IMusicClientSettings settings, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Build API url
            StringBuilder url = new StringBuilder();
            url.Append(@"http://api.ent.nokia.com/1.x/");

            AddCountryCode(url, method, settings.CountryCode);
            method.AppendUriPath(url, pathParams);
            this.AppendQueryString(url, settings, querystringParams);

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
        /// <param name="method">The method to call.</param>
        /// <param name="countryCode">The country code.</param>
        private static void AddCountryCode(StringBuilder url, ApiMethod method, string countryCode)
        {
            if (method.RequiresCountryCode)
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
        /// <param name="querystringParams">The querystring parameters.</param>
        private void AppendQueryString(StringBuilder url, IMusicClientSettings settings, Dictionary<string, string> querystringParams)
        {
            // Add required parameters
            this.AddAuthorisationParams(url, settings);
            url.AppendFormat(@"&domain=music");

            // Add other parameters...
            if (querystringParams != null)
            {
                foreach (string key in querystringParams.Keys)
                {
                    if (!string.IsNullOrEmpty(querystringParams[key]))
                    {
                        url.AppendFormat(@"&{0}={1}", key, querystringParams[key]);
                    }
                }
            }
        }
    }
}
