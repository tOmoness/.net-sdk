// -----------------------------------------------------------------------
// <copyright file="CountryCodeRequiredException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Exception when no country has been supplied
    /// </summary>
    public class CountryCodeRequiredException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryCodeRequiredException" /> class.
        /// </summary>
        public CountryCodeRequiredException()
            : base("A country code is required for this method, please supply a country code from GetDeviceCountry or GetAllCountries")
        {
        }
    }
}
