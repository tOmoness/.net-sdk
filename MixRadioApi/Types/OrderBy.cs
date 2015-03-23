// -----------------------------------------------------------------------
// <copyright file="OrderBy.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio.Types
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
