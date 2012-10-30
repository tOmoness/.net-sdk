// -----------------------------------------------------------------------
// <copyright file="LaunchTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia Music
    /// </summary>
    public sealed class LaunchTask : TaskBase
    {
        /// <summary>
        /// Shows Nokia Music
        /// </summary>
        public void Show()
        {
            this.Launch(new Uri("nokia-music://"), new Uri("http://music.nokia.com/"));
        }
    }
}
