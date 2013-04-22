// -----------------------------------------------------------------------
// <copyright file="ApiNotAvailableException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music
{
    /// <summary>
    /// Exception when an the API is not available in the current region
    /// </summary>
    public class ApiNotAvailableException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiNotAvailableException" /> class.
        /// </summary>
        public ApiNotAvailableException()
            : base("The Nokia Music API is not available in the current region")
        {
        }
    }
}
