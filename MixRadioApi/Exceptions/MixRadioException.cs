// -----------------------------------------------------------------------
// <copyright file="MixRadioException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio
{
    /// <summary>
    /// Generic MixRadio Exception.
    /// </summary>
    public class MixRadioException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MixRadioException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public MixRadioException(string message) : base(message)
        {
        }
    }
}
