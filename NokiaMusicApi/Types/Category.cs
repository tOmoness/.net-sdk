// -----------------------------------------------------------------------
// <copyright file="Category.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Defines the API item category
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// The item is unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The item is an Artist
        /// </summary>
        Artist = 1,

        /// <summary>
        /// The item is an Album
        /// </summary>
        Album = 2,

        /// <summary>
        /// The item is a Single
        /// </summary>
        Single = 3,

        /// <summary>
        /// The item is a Track
        /// </summary>
        Track = 4
    }
}
