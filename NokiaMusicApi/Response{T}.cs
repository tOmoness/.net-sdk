// -----------------------------------------------------------------------
// <copyright file="Response{T}.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace Nokia.Music
{
    /// <summary>
    /// Contains the result or the error if an error occurred.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The result.</param>
        /// <param name="requestId">The request id</param>
        /// <param name="foundMixRadioHeader">The X-Mix-Radio header state.</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        Response(HttpStatusCode? statusCode, T result, Guid requestId, bool? foundMixRadioHeader = null)
            : this(statusCode, null, result, requestId, foundMixRadioHeader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="contentType">The response Content Type.</param>
        /// <param name="result">The result.</param>
        /// <param name="requestId">The request id</param>
        /// <param name="foundMixRadioHeader">The X-Mix-Radio header state.</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        Response(HttpStatusCode? statusCode, string contentType, T result, Guid requestId, bool? foundMixRadioHeader = null)
            : base(statusCode, requestId, foundMixRadioHeader)
        {
            this.ContentType = contentType;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status code</param>
        /// <param name="error">The error.</param>
        /// <param name="responseBody">The response body.</param>
        /// <param name="requestId">The request id.</param>
        /// <param name="foundMixRadioHeader">The X-Mix-Radio header state.</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif 
        Response(HttpStatusCode? statusCode, Exception error, string responseBody, Guid requestId, bool? foundMixRadioHeader = null)
            : base(statusCode, error, responseBody, requestId, foundMixRadioHeader)
        {
        }

        /// <summary>
        /// Gets the result if the call was successful
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP Content Type
        /// </summary>
        /// <value>
        /// The content type.
        /// </value>
        internal string ContentType { get; set; }
    }
}
