// -----------------------------------------------------------------------
// <copyright file="ApiCredentialsRequiredException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music
{
    /// <summary>
    /// Exception when no API key has been supplied
    /// </summary>
    public class ApiCredentialsRequiredException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCredentialsRequiredException" /> class.
        /// </summary>
        public ApiCredentialsRequiredException() : base("API Credentials are required for all method calls")
        {
        }
    }
}
