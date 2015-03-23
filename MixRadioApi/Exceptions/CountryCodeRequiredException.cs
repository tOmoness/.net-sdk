// -----------------------------------------------------------------------
// <copyright file="CountryCodeRequiredException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio
{
    /// <summary>
    /// Exception when no country has been supplied
    /// </summary>
    public class CountryCodeRequiredException : MixRadioException
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
