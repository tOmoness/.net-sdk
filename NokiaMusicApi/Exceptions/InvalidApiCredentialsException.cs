// -----------------------------------------------------------------------
// <copyright file="InvalidApiCredentialsException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Exception when invalid API credentials have been supplied
    /// </summary>
    public class InvalidApiCredentialsException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApiCredentialsException" /> class.
        /// </summary>
        public InvalidApiCredentialsException()
            : base("The API Credentials (AppId and AppCode) appear to be invalid")
        {
        }
    }
}
