// -----------------------------------------------------------------------
// <copyright file="ScopeExtensions.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MixRadio.Types
{
    /// <summary>
    /// Helper method(s) for Scope type
    /// </summary>
    internal static class ScopeExtensions
    {
        /// <summary>
        /// Generates the scope param.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A string representation of scope flags</returns>
        internal static string AsStringParam(this Scope scope)
        {
            return string.Join(" ", AsStringParams(scope));
        }

        /// <summary>
        /// Generates the scope param.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A string representation of scope flags</returns>
        internal static string[] AsStringParams(this Scope scope)
        {
            // As Scope is a flag enum, we need to get all scopes possible
            // and then check if the scope flags we're converting contains it
            Type scopeType = typeof(Scope);
#if NETFX_CORE || UNIT_TESTS || PORTABLE || SILVERLIGHT || WINDOWS_PHONE
            var allScopes = (Scope[])Enum.GetValues(scopeType);
#else
            IEnumerable<FieldInfo> fields = scopeType.GetFields().Where(field => field.IsLiteral);
            var allScopes = fields.Select(field => field.GetValue(scopeType)).Select(value => (Scope)value);
#endif

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

            return scopes.ToArray();
        }

        /// <summary>
        /// Converts a scope enumeration value into a string representation.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>A string value</returns>
        private static string ConvertScopeEnum(Scope scope)
        {
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