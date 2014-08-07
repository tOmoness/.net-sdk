// -----------------------------------------------------------------------
// <copyright file="NetworkUnavailableException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music
{
    /// <summary>
    /// Exception when attempting to call the API without a working network.
    /// </summary>
    public class NetworkUnavailableException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkUnavailableException"/> class.
        /// </summary>
        public NetworkUnavailableException()
            : base("Network Unavailable")
        {
        }
    
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkUnavailableException"/> class.
        /// </summary>
        /// <param name="message">
        /// A more specific message indicating reasons for the error.
        /// </param>
        public NetworkUnavailableException(string message)
            : base(message)
        {
        }
    }
}