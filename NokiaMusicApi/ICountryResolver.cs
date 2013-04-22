// -----------------------------------------------------------------------
// <copyright file="ICountryResolver.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
#if SUPPORTS_ASYNC
using System.Threading.Tasks;
#endif

namespace Nokia.Music
{
    /// <summary>
    /// Defines the Nokia Music Country Resolver API
    /// </summary>
    public interface ICountryResolver
    {
#if !NETFX_CORE
        /// <summary>
        /// Validates that the Nokia Music API is available for a country
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="countryCode">The country code.</param>
        void CheckAvailability(Action<Response<bool>> callback, string countryCode);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Validates that the Nokia Music API is available for a country
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns>A Response containing whether the API is available or not</returns>
        Task<Response<bool>> CheckAvailabilityAsync(string countryCode);
#endif
    }
}
