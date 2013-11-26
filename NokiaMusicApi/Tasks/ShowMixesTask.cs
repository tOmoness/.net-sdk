// -----------------------------------------------------------------------
// <copyright file="ShowMixesTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Internal;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia MixRadio Mixes
    /// </summary>
    public sealed class ShowMixesTask : TaskBase
    {
        /// <summary>
        /// Shows Nokia MixRadio Mixes
        /// </summary>
        public void Show()
        {
            this.Launch(new Uri("nokia-music://show/mixes/"), new Uri("http://www.mixrad.io/mixes"));
        }
    }
}
