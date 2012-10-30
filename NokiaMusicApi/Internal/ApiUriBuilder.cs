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
    internal sealed class ApiUriBuilder : IApiUriBuilder
    {
        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="appCode">The app code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when an unknown method is used</exception>
        /// <exception cref="CountryCodeRequiredException">Thrown when a CountryCode is required but not supplied</exception>
        /// <exception cref="ApiCredentialsRequiredException">Thrown when an API Key has not been supplied</exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public Uri BuildUri(ApiMethod method, string appId, string appCode, string countryCode, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams)
        {
            // Validate Method and Country Code if it's required
            switch (method)
            {
                case ApiMethod.Unknown:
                    throw new ArgumentOutOfRangeException("method");

                case ApiMethod.CountryLookup:
                    // No country code required
                    break;

                default:
                    if (string.IsNullOrEmpty(countryCode))
                    {
                        throw new CountryCodeRequiredException();
                    }

                    break;
            }

            // Validate API Credentials...
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appCode))
            {
                throw new ApiCredentialsRequiredException();
            }

            // Build API url
            StringBuilder url = new StringBuilder();
            url.Append(@"http://api.ent.nokia.com/1.x/");

            switch (method)
            {
                case ApiMethod.CountryLookup:
                    // nothing extra needed
                    break;

                case ApiMethod.Search:
                    url.AppendFormat("{0}/", countryCode);
                    break;

                case ApiMethod.ArtistProducts:
                    if (pathParams != null && pathParams.ContainsKey("id"))
                    {
                        url.AppendFormat("{0}/creators/{1}/products/", countryCode, pathParams["id"]);
                    }
                    else
                    {
                        throw new ArgumentNullException("id");
                    }

                    break;

                case ApiMethod.SimilarArtists:
                    if (pathParams != null && pathParams.ContainsKey("id"))
                    {
                        url.AppendFormat("{0}/creators/{1}/similar/", countryCode, pathParams["id"]);
                    }
                    else
                    {
                        throw new ArgumentNullException("id");
                    }

                    break;

                case ApiMethod.Genres:
                    url.AppendFormat("{0}/genres/", countryCode);
                    break;

                case ApiMethod.MixGroups:
                    url.AppendFormat("{0}/mixes/groups/", countryCode);
                    break;

                case ApiMethod.Mixes:
                    if (pathParams != null && pathParams.ContainsKey("id"))
                    {
                        url.AppendFormat("{0}/mixes/groups/{1}/", countryCode, pathParams["id"]);
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }

                    break;

                case ApiMethod.ProductChart:
                    if (pathParams != null && pathParams.ContainsKey("category"))
                    {
                        url.AppendFormat("{0}/products/charts/{1}/", countryCode, pathParams["category"]);
                    }
                    else
                    {
                        throw new ArgumentNullException("category");
                    }

                    break;

                case ApiMethod.ProductNewReleases:
                    if (pathParams != null && pathParams.ContainsKey("category"))
                    {
                        url.AppendFormat("{0}/products/new/{1}/", countryCode, pathParams["category"]);
                    }
                    else
                    {
                        throw new ArgumentNullException("category");
                    }

                    break;

                case ApiMethod.Recommendations:
                    url.AppendFormat("{0}/recommendations/", countryCode);
                    break;
            }

            // Add required parameters...
            url.AppendFormat(@"?app_id={0}&app_code={1}&domain=music", appId, appCode);
            
            // Add other parameters...
            if (querystringParams != null)
            {
                foreach (string key in querystringParams.Keys)
                {
                    url.AppendFormat(@"&{0}={1}", key, querystringParams[key]);
                }
            }

            return new Uri(url.ToString());
        }
    }
}
