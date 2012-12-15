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
    /// Defines the API URI Builder interface
    /// </summary>
    internal interface IApiUriBuilder
    {
        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="method">The method to call.</param>
        /// <param name="settings">The client settings.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        Uri BuildUri(ApiMethod method, IMusicClientSettings settings, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams);
    }
}
