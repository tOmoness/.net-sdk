// -----------------------------------------------------------------------
// <copyright file="ICountryResolver.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Defines the Nokia Music Country Resolver API
    /// </summary>
    public interface ICountryResolver
    {
        /// <summary>
        /// Validates that the Nokia Music API is available for a country
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="countryCode">The country code.</param>
        void CheckAvailability(Action<Response<bool>> callback, string countryCode);
    }
}
