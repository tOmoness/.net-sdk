// -----------------------------------------------------------------------
// <copyright file="ApiCredentialsRequiredException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Exception when no API key has been supplied
    /// </summary>
    public class ApiCredentialsRequiredException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCredentialsRequiredException" /> class.
        /// </summary>
        public ApiCredentialsRequiredException() : base("API Credentials (AppId and AppCode) are required for all method calls")
        {
        }
    }
}
