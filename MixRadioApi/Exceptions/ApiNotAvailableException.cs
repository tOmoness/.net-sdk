﻿// -----------------------------------------------------------------------
// <copyright file="ApiNotAvailableException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio
{
    /// <summary>
    /// Exception when an the API is not available in the current region
    /// </summary>
    public class ApiNotAvailableException : MixRadioException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiNotAvailableException" /> class.
        /// </summary>
        public ApiNotAvailableException()
            : base("The MixRadio API is not available in the current region")
        {
        }
    }
}
