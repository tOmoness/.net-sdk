// -----------------------------------------------------------------------
// <copyright file="InvalidCountryCodeException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio
{
    /// <summary>
    /// Exception when an invalid country code has been supplied
    /// </summary>
    public class InvalidCountryCodeException : MixRadioException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCountryCodeException" /> class.
        /// </summary>
        public InvalidCountryCodeException()
            : base("A valid ISO 3166-2 country code is required")
        {
        }
    }
}
