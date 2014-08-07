// -----------------------------------------------------------------------
// <copyright file="ApiCallCancelledException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music
{
    /// <summary>
    /// Exception when an API call gets cancelled
    /// </summary>
    public class ApiCallCancelledException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCallCancelledException"/> class.
        /// </summary>
        public ApiCallCancelledException()
            : base("API call cancelled")
        {
        }
    }
}
