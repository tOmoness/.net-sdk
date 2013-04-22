// -----------------------------------------------------------------------
// <copyright file="IMusicClientSettings.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Internal
{
    /// <summary>
    /// Defines the MusicClient Settings
    /// </summary>
    internal interface IMusicClientSettings
    {
        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>
        /// The app id.
        /// </value>
        string AppId { get; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        string CountryCode { get; }

        /// <summary>
        /// Gets a value indicating whether the country code was based on region info.
        /// </summary>
        /// <value>
        /// <c>true</c> if the country code was based on region info; otherwise, <c>false</c>.
        /// </value>
        bool CountryCodeBasedOnRegionInfo { get; }
    }
}