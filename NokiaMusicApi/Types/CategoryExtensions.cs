// -----------------------------------------------------------------------
// <copyright file="CategoryExtensions.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone.Types
{
    internal static class CategoryExtensions
    {
        /// <summary>
        /// Parses a Category id into an enumeration
        /// </summary>
        /// <param name="category">The string representation of the Category</param>
        /// <returns>A Category</returns>
        /// <remarks>Internal for testing purposes</remarks>
        internal static Category ParseCategory(string category)
        {
            try
            {
                return (Category)Enum.Parse(typeof(Category), category, true);
            }
            catch (ArgumentException)
            {
                return Category.Unknown;
            }
        }
    }
}
