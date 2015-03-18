// -----------------------------------------------------------------------
// <copyright file="ICountryResolver.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nokia.Music
{
    /// <summary>
    /// Defines the MixRadio Country Resolver API
    /// </summary>
    public interface ICountryResolver
    {
        /// <summary>
        /// Validates that the MixRadio API is available for a country
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <param name="cancellationToken">An optional CancellationToken</param>
        /// <returns>A Response containing whether the API is available or not</returns>
        Task<bool> CheckAvailabilityAsync(string countryCode, CancellationToken? cancellationToken = null);
    }
}
