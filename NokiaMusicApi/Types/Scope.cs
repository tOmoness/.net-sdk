// -----------------------------------------------------------------------
// <copyright file="Scope.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Types
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