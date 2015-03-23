// -----------------------------------------------------------------------
// <copyright file="NetworkLimitedException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio
{
    /// <summary>
    /// Exception when attempting to call the API with a limited network.
    /// </summary>
    public class NetworkLimitedException : MixRadioException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkLimitedException"/> class.
        /// </summary>
        public NetworkLimitedException()
            : base("Network Limited")
        {
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkLimitedException"/> class.
        /// </summary>
        /// <param name="message">
        /// A more specific message indicating reasons for the error.
        /// </param>
        public NetworkLimitedException(string message)
            : base(message)
        {
        }
    }
}