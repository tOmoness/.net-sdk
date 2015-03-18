// -----------------------------------------------------------------------
// <copyright file="ResponseInfo.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nokia.Music.Internal.Response
{
#if OPEN_INTERNALS
    /// <summary>
    /// Represents a response
    /// </summary>
    public
#else
    internal
#endif
    class ResponseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseInfo"/> class.
        /// </summary>
        /// <param name="responseUri">The response URI.</param>
        /// <param name="headers">The headers.</param>
        public ResponseInfo(Uri responseUri, Dictionary<string, IEnumerable<string>> headers)
        {
            this.ResponseUri = responseUri;
            this.Headers = headers;
        }

        /// <summary>
        /// Gets the response URI.
        /// </summary>
        /// <value>
        /// The response URI.
        /// </value>
        public Uri ResponseUri { get; private set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public Dictionary<string, IEnumerable<string>> Headers { get; private set; }
    }
}
