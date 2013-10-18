// -----------------------------------------------------------------------
// <copyright file="AuthScope.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Defines the access scopes available
    /// </summary>
    [Flags]
    public enum Scope : long
    {
        /// <summary>
        /// No scope is set
        /// </summary>
        None = 0,

        /// <summary>
        /// Read user play history
        /// </summary>
        ReadUserPlayHistory = 1,

        /// <summary>
        /// Read user profile
        /// </summary>
        ReadUserTasteProfile = 1 << 1,
    }

    /// <summary>
    /// Helper method(s) for Scope type
    /// </summary>
    internal static class AuthScope
    {
        /// <summary>
        /// Generates the scope param.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A string representation of scope flags</returns>
        internal static string AsStringParam(this Scope scope)
        {
            // As Scope is a flag enum, we need to get all scopes possible
            // and then check if the scope flags we're converting contains it
            Type scopeType = typeof(Scope);
            var allScopes = (Scope[])Enum.GetValues(scopeType);

            List<string> scopes = new List<string>();

            foreach (Scope targetScope in allScopes)
            {
                if (targetScope != Scope.None)
                {
                    if ((scope & targetScope) == targetScope)
                    {
                        scopes.Add(ConvertScopeEnum(targetScope));
                    }
                }
            }

            return string.Join(" ", scopes);
        }

        /// <summary>
        /// Converts a scope enumeration value into a string representation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A string value</returns>
        private static string ConvertScopeEnum(Scope scope)
        {
            // Main work here is to separate the verb from the action...
            return scope.ToString().ToLowerInvariant()
                                    .Replace("read", "read_")
                                    .Replace("write", "write_")
                                    .Replace("usage", "usage_")
                                    .Replace("playmix", "play_mix")
                                    .Replace("download", "download_")
                                    .Replace("receive", "receive_");
        }
    }
}
