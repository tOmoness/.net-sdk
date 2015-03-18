﻿// -----------------------------------------------------------------------
// <copyright file="ApiCallFailedException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Net;

namespace Nokia.Music
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCallFailedException"/> class.
        /// </summary>
        /// <param name="statusCode">Details to append to the exception message</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        ApiCallFailedException(HttpStatusCode? statusCode)
            : base(string.Format(CultureInfo.InvariantCulture, "Unexpected failure, check connectivity. Result: {0}", statusCode.HasValue ? statusCode.ToString() : "timeout"))
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the HTTP status code that caused the exception to be constructed
        /// </summary>
        public HttpStatusCode? StatusCode { get; private set; }
    }
}
