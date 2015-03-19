// -----------------------------------------------------------------------
// <copyright file="PlayMeTask.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to start a MixRadio PlayMe Mix
    /// </summary>
    public sealed class PlayMeTask : TaskBase
    {
        /// <summary>
        /// Starts PlayMe in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            await this.Launch(
                new Uri("mixradio://play/me/"),
                new Uri("http://www.mixrad.io/mixes/seeded/")).ConfigureAwait(false);
        }
    }
}
