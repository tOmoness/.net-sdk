// -----------------------------------------------------------------------
// <copyright file="ApiCallFailedException.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Exception when an API call fails unexpectedly
    /// </summary>
    public class ApiCallFailedException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCallFailedException"/> class.
        /// </summary>
        public ApiCallFailedException()
            : base("Unexpected failure, check connectivity")
        {
        }
    }
}
