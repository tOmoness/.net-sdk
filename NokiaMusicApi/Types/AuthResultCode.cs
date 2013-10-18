// -----------------------------------------------------------------------
// <copyright file="AuthResultCode.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Types
{
    /// <summary>
    /// The OAuth result code
    /// <remarks>
    /// see http://tools.ietf.org/html/rfc6749#section-4.1.2.1
    /// </remarks>
    /// </summary>
    public enum AuthResultCode
    {
        /// <summary>
        /// Unknown error
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Completed Successfully
        /// </summary>
        Success = 1,

        /// <summary>
        /// Cancelled by user
        /// </summary>
        Cancelled = 2,

        /// <summary>
        /// Access denied by user
        /// </summary>
        AccessDenied = 3,

        /// <summary>
        /// The client id was not valid
        /// </summary>
        UnauthorizedClient = 4,

        /// <summary>
        /// An invalid scope was specified
        /// </summary>
        InvalidScope = 5,

        /// <summary>
        /// A server error occurred
        /// </summary>
        ServerError = 6,

        /// <summary>
        /// An error occurred trying to refresh an existing token
        /// </summary>
        FailedToRefresh = 7,

        /// <summary>
        /// A silent refresh from an existing token was not possible as there is no cached token
        /// </summary>
        NoCachedToken = 8,
    }
}
