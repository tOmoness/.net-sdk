// -----------------------------------------------------------------------
// <copyright file="UserEventAction.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents the action that the UserEvent relates to
    /// </summary>
    public enum UserEventAction
    {
        /// <summary>
        /// Unset property
        /// </summary>
        Unknown,

        /// <summary>
        /// Track completed playing
        /// </summary>
        Complete,

        /// <summary>
        /// Skipped previous midway through track
        /// </summary>
        SkipPrev,

        /// <summary>
        /// Skipped next midway through track
        /// </summary>
        SkipNext,

        /// <summary>
        /// Playback stopped
        /// </summary>
        Stop,
    }
}
