// -----------------------------------------------------------------------
// <copyright file="Response.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace Nokia.Music
{
    /// <summary>
    /// Contains the result status code and the error if an error occurred.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
#if OPEN_INTERNALS
        public
#else
        internal
#endif 
        Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="requestId">The request id</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        Response(HttpStatusCode? statusCode, Guid requestId)
            : this(statusCode, null, null, requestId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status code</param>
        /// <param name="error">The error.</param>
        /// <param name="responseBody">The response body.</param>
        /// <param name="requestId">The request id.</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        Response(HttpStatusCode? statusCode, Exception error, string responseBody, Guid requestId)
        {
            this.StatusCode = statusCode;
            this.Error = error;
            this.ErrorResponseBody = responseBody;
            this.RequestId = requestId;
        }

        /// <summary>
        /// Gets or sets the exception if the call was not successful
        /// </summary>
        public Exception Error { get; protected set; }

        /// <summary>
        /// Gets or sets the response body supplied for an error response
        /// </summary>
        /// <value>
        /// The raw response body.
        /// </value>
        public string ErrorResponseBody { get; set; }

        /// <summary>
        /// Gets a value indicating whether the call succeeded
        /// </summary>
        public bool Succeeded
        {
            get { return this.Error == null; }
        }

        /// <summary>
        /// Gets or sets the id of this request
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Status code
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }
    }
}
