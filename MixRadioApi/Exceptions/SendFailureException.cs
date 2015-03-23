// -----------------------------------------------------------------------
// <copyright file="SendFailureException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio
{
    /// <summary>
    /// Exception thrown when there was a problem with sending request
    /// </summary>
    public class SendFailureException : MixRadioException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendFailureException"/> class.
        /// </summary>
        public SendFailureException() : base("Send failure")
        { 
        }
    }
}
