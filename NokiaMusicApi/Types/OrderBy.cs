// -----------------------------------------------------------------------
// <copyright file="OrderBy.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Types
{
    /// <summary>
    /// Order by enumeration for ordering products etc.
    /// </summary>
    public enum OrderBy
    {
        /// <summary>
        /// Items are ordered by relevance (default)
        /// </summary>
        Relevance = 0,

        /// <summary>
        /// Items are ordered by name
        /// </summary>
        Name = 1,
        
        /// <summary>
        /// Items are ordered by release date
        /// </summary>
        ReleaseDate = 2
    }
}
