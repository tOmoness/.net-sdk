// -----------------------------------------------------------------------
// <copyright file="HttpMethod.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Phone.Internal.Request
{
    /// <summary>
    /// Currently supported HTTP method types
    /// </summary>
    internal enum HttpMethod
    {
        /// <summary>
        /// Represents an HTTP HEAD request
        /// </summary>
        Head,

        /// <summary>
        /// Represents an HTTP GET
        /// </summary>
        Get,

        /// <summary>
        /// Represents an HTTP POST
        /// </summary>
        Post,

        /// <summary>
        /// Represents an HTTP PUT
        /// </summary>
        Put,

        /// <summary>
        /// Represents an HTTP DELETE
        /// </summary>
        Delete
    }
}
