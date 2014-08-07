// -----------------------------------------------------------------------
// <copyright file="LaunchTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show MixRadio
    /// </summary>
    public sealed class LaunchTask : TaskBase
    {
        /// <summary>
        /// Shows MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
#if WINDOWS_APP
            await this.Launch(new Uri("nokia-music://"), new Uri("http://www.mixrad.io/")).ConfigureAwait(false);
#else
            await this.Launch(new Uri("mixradio://"), new Uri("http://www.mixrad.io/")).ConfigureAwait(false);
#endif
        }
    }
}
