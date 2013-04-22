// -----------------------------------------------------------------------
// <copyright file="LocationSource.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Types
{
    /// <summary>
    /// The location data source
    /// </summary>
    public enum LocationSource
    {
        /// <summary>
        /// The location data was obtained from a celullar source
        /// </summary>
        Cellular,

        /// <summary>
        /// The location data was obtained from a satellite source
        /// </summary>
        Satellite,

        /// <summary>
        /// The location data was obtained from a wifi source
        /// </summary>
        Wifi
    }
}
