// -----------------------------------------------------------------------
// <copyright file="IApiUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Defines API methods available to call
    /// </summary>
    internal enum ApiMethod
    {
        /// <summary>
        /// Unknown API
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Country Lookup API
        /// </summary>
        CountryLookup = 1,

        /// <summary>
        /// Search API
        /// </summary>
        Search = 2,

        /// <summary>
        /// Genre List API
        /// </summary>
        Genres = 3,

        /// <summary>
        /// Similar Artists API
        /// </summary>
        SimilarArtists = 4,

        /// <summary>
        /// Artist Products API
        /// </summary>
        ArtistProducts = 5,

        /// <summary>
        /// Chart API
        /// </summary>
        ProductChart = 6,

        /// <summary>
        /// New Releases API
        /// </summary>
        ProductNewReleases = 7,

        /// <summary>
        /// Recommendations API
        /// </summary>
        Recommendations = 8,

        /// <summary>
        /// Mix Groups API
        /// </summary>
        MixGroups = 20,

        /// <summary>
        /// Mixes API
        /// </summary>
        Mixes = 21,
    }

    /// <summary>
    /// Defines the API URI Builder interface
    /// </summary>
    internal interface IApiUriBuilder
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
        Uri BuildUri(ApiMethod method, string appId, string appCode, string countryCode, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams);
    }
}
