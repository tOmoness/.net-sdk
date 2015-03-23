// -----------------------------------------------------------------------
// <copyright file="Scope.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MixRadio.Types
{
    /// <summary>
    ///     Defines the access scopes available
    /// </summary>
    [Flags]
    public enum Scope : long
    {
        /// <summary>
        ///     No scope is set
        /// </summary>
        None = 0,

        /// <summary>
        ///     Read user play history
        /// </summary>
        ReadUserPlayHistory = 1,
    }
}