// -----------------------------------------------------------------------
// <copyright file="NokiaMusicException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Generic Nokia Music Exception.
    /// </summary>
    public class NokiaMusicException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NokiaMusicException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public NokiaMusicException(string message) : base(message)
        {
        }
    }
}
