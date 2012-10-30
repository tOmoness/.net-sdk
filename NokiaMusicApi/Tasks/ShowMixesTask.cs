// -----------------------------------------------------------------------
// <copyright file="ShowMixesTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia Music Mixes
    /// </summary>
    public sealed class ShowMixesTask : TaskBase
    {
        /// <summary>
        /// Shows Nokia Music Mixes
        /// </summary>
        public void Show()
        {
            this.Launch(new Uri("nokia-music://show/mixes/"), new Uri("http://music.nokia.com/"));
        }
    }
}
