// -----------------------------------------------------------------------
// <copyright file="UserEventAction.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace MixRadio.Types
{
    /// <summary>
    /// Represents the action that the UserEvent relates to
    /// </summary>
    [Flags]
    public enum UserEventAction
    {
        /// <summary>
        /// Unset property
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Track completed playing
        /// </summary>
        Complete = 1,

        /// <summary>
        /// Skipped previous midway through track
        /// </summary>
        SkipPrev = 2,

        /// <summary>
        /// Skipped next midway through track
        /// </summary>
        SkipNext = 4,

        /// <summary>
        /// Playback stopped
        /// </summary>
        Stop = 8,

        /// <summary>
        /// User likes a track
        /// </summary>
        Like = 16,

        /// <summary>
        /// User dislikes a track
        /// </summary>
        Dislike = 32,
    }
}
