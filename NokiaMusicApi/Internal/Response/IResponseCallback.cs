// -----------------------------------------------------------------------
// <copyright file="IResponseCallback.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Internal.Response
{
    /// <summary>
    /// Defines a response callback
    /// </summary>
    /// <typeparam name="T">The type that the response callback is for</typeparam>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
    interface IResponseCallback<T>
    {
        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        Action<Response<T>> Callback { get; }

        /// <summary>
        /// Converts from a raw response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A converted response</returns>
        T ConvertFromRawResponse(string response);
    }
}
