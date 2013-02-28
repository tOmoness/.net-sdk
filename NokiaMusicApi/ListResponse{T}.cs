// -----------------------------------------------------------------------
// <copyright file="ListResponse{T}.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Contains the result or the error if an error occurred.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class ListResponse<T> : Response<List<T>>, IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListResponse{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="result">The result.</param>
        /// <param name="startIndex">The start index asked for.</param>
        /// <param name="itemsPerPage">The items per page asked for.</param>
        /// <param name="totalResults">The total results available.</param>
        /// <param name="requestId">The request id.</param>
        internal ListResponse(HttpStatusCode? statusCode, List<T> result, int? startIndex, int? itemsPerPage, int? totalResults, Guid requestId)
            : base(statusCode, result, requestId)
        {
            this.StartIndex = startIndex;
            this.ItemsPerPage = itemsPerPage;
            this.TotalResults = totalResults;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListResponse{T}" /> class.
        /// </summary>
        /// <param name="statusCode">The HTTP Status code</param>
        /// <param name="error">The error.</param>
        /// <param name="errorResponseBody">The response body</param>
        /// <param name="requestId">The request id.</param>
        internal ListResponse(HttpStatusCode? statusCode, Exception error, string errorResponseBody, Guid requestId)
            : base(statusCode, error, errorResponseBody, requestId)
        {
        }

        /// <summary>
        /// Gets the items per page the API call was asked for.
        /// </summary>
        /// <value>
        /// The items per page.
        /// </value>
        public int? ItemsPerPage { get; private set; }

        /// <summary>
        /// Gets the start index the API call was asked for.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int? StartIndex { get; private set; }

        /// <summary>
        /// Gets the total results available.
        /// </summary>
        /// <value>
        /// The total results.
        /// </value>
        public int? TotalResults { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.Result != null)
            {
                return this.Result.GetEnumerator();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.Result != null)
            {
                return this.Result.GetEnumerator();
            }
            else
            {
                return null;
            }
        }
    }
}
