// -----------------------------------------------------------------------
// <copyright file="ApiCredentialsRequiredException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio
{
    /// <summary>
    /// Exception when no API key has been supplied
    /// </summary>
    public class ApiCredentialsRequiredException : MixRadioException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCredentialsRequiredException" /> class.
        /// </summary>
        public ApiCredentialsRequiredException() : base("API Credentials are required for all method calls")
        {
        }
    }
}
