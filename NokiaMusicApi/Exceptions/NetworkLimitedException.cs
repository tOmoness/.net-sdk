// -----------------------------------------------------------------------
// <copyright file="NetworkLimitedException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music
{
    /// <summary>
    /// Exception when attempting to call the API with a limited network.
    /// </summary>
    public class NetworkLimitedException : NokiaMusicException
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