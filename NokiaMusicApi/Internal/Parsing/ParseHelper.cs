// -----------------------------------------------------------------------
// <copyright file="ParseHelper.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone.Internal.Parsing
{
    internal static class ParseHelper
    {
        /// <summary>
        /// Returns a matching enum value for specified string
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The response value</param>
        /// <returns>The matching value or the default value which should be unknown or none</returns>
        internal static T ParseEnumOrDefault<T>(string value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
