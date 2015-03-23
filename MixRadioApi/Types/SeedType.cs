// -----------------------------------------------------------------------
// <copyright file="SeedType.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio.Types
{
    /// <summary>
    /// The type of Mix Seed
    /// </summary>
    public enum SeedType
    {
        /// <summary>
        /// Unknown seed
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Seed from artist name
        /// </summary>
        ArtistName = 0,

        /// <summary>
        /// Seed from artist entity Id
        /// </summary>
        ArtistId = 1,

        /// <summary>
        /// Seed from a user Id
        /// </summary>
        UserId = 2,

        /// <summary>
        /// Seed from a mix Id
        /// </summary>
        MixId = 3
    }
}
