// -----------------------------------------------------------------------
// <copyright file="ShowMixesTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show MixRadio Mixes
    /// </summary>
    public sealed class ShowMixesTask : TaskBase
    {
        /// <summary>
        /// Shows MixRadio Mixes
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
#if WINDOWS_APP
            var appUri = new Uri("nokia-music://show/mixes/");
#else
            var appUri = new Uri("mixradio://show/mixes/");
#endif
            await this.Launch(appUri, new Uri("http://www.mixrad.io/mixes")).ConfigureAwait(false);
        }
    }
}
