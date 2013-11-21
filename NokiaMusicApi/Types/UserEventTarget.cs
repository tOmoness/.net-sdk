// -----------------------------------------------------------------------
// <copyright file="UserEventTarget.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents the target type that the UserEvent relates to
    /// </summary>
    public enum UserEventTarget
    {
        /// <summary>
        /// Unset property
        /// </summary>
        Unknown,

        /// <summary>
        /// Track event
        /// </summary>
        Track,

        /// <summary>
        /// Mix event
        /// </summary>
        Mix,

        /// <summary>
        /// The event targets an artist mix
        /// </summary>
        Create,

        /// <summary>
        /// The event targets a Play Me mix
        /// </summary>
        PlayMe,

        /// <summary>
        /// The event targets a shared artist mix
        /// </summary>
        SharedCreate,

        /// <summary>
        /// The event targets a shared Play Me mix
        /// </summary>
        SharedPlayMe
    }
}
