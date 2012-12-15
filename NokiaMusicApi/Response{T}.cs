// -----------------------------------------------------------------------
// <copyright file="Response{T}.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Contains the result or the error if an error occurred.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The result.</param>
        internal Response(HttpStatusCode? statusCode, T result)
            : this(statusCode, null, result)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="contentType">The response Content Type.</param>
        /// <param name="result">The result.</param>
        internal Response(HttpStatusCode? statusCode, string contentType, T result)
        {
            this.ContentType = contentType;
            this.StatusCode = statusCode;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status code</param>
        /// <param name="error">The error.</param>
        internal Response(HttpStatusCode? statusCode, Exception error)
        {
            this.StatusCode = statusCode;
            this.Error = error;
        }

        /// <summary>
        /// Gets the exception if the call was not successful
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the result if the call was successful
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP Status code
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP Content Type
        /// </summary>
        /// <value>
        /// The content type.
        /// </value>
        internal string ContentType { get; set; }
    }
}
