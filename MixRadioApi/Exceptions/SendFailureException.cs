// -----------------------------------------------------------------------
// <copyright file="SendFailureException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music
{
    /// <summary>
    /// Exception thrown when there was a problem with sending request
    /// </summary>
    public class SendFailureException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendFailureException"/> class.
        /// </summary>
        public SendFailureException() : base("Send failure")
        { 
        }
    }
}
