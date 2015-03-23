// -----------------------------------------------------------------------
// <copyright file="ApiCallCancelledException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio
{
    /// <summary>
    /// Exception when an API call gets cancelled
    /// </summary>
    public class ApiCallCancelledException : MixRadioException
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
