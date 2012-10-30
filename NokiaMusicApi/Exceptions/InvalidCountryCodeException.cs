// -----------------------------------------------------------------------
// <copyright file="InvalidCountryCodeException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Exception when an invalid country code has been supplied
    /// </summary>
    public class InvalidCountryCodeException : NokiaMusicException
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
