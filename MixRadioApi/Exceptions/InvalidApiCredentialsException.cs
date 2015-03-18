﻿// -----------------------------------------------------------------------
// <copyright file="InvalidApiCredentialsException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music
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
            : base("The API Credentials appear to be invalid. For user-secured resources, we recommend calling DeleteAuthenticationToken and getting the user to re-authenticate.")
        {
        }
    }
}
