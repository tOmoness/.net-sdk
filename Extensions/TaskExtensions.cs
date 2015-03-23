// -----------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using MixRadio.Tasks;
using MixRadio.Types;

namespace MixRadio.Types
{
    /// <summary>
    /// Launcher extensions
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Launches MixRadio to start a mix for the artist using the PlayMixTask
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <returns>An async task to await</returns>
        public static async Task PlayMix(this Artist artist)
        {
            PlayMixTask task = new PlayMixTask() { ArtistId = artist.Id, ArtistName = artist.Name };
            await task.Show().ConfigureAwait(false);
        }

        /// <summary>
        /// Launches MixRadio to show details for the artist using the ShowArtistTask
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <returns>An async task to await</returns>
        public static async Task Show(this Artist artist)
        {
            ShowArtistTask task = new ShowArtistTask() { ArtistId = artist.Id, ArtistName = artist.Name };
            await task.Show().ConfigureAwait(false);
        }

        /// <summary>
        /// Launches MixRadio to start playback of the mix using the PlayMixTask
        /// </summary>
        /// <param name="mix">The mix.</param>
        /// <returns>An async task to await</returns>
        public static async Task Play(this Mix mix)
        {
            if (!string.IsNullOrEmpty(mix.Id))
            {
                PlayMixTask task = new PlayMixTask() { MixId = mix.Id };
                await task.Show().ConfigureAwait(false);
                return;
            }
#if WINDOWS_PHONE
            else if (mix.Seeds.Where(s => s.Type == SeedType.UserId).Count() > 0)
            {
                await new PlayMeTask().Show().ConfigureAwait(false);
                return;
            }
#endif

            if (mix.Seeds != null)
            {
                var artistSeeds = mix.Seeds.Where(s => (s.Type == SeedType.ArtistId || s.Type == SeedType.ArtistName));

                // for now, just take the first artist name - need to support multiple soon though
                var name = artistSeeds.Select(s => s.Name).Where(s => !string.IsNullOrEmpty(s)).FirstOrDefault();
                if (!string.IsNullOrEmpty(name))
                {
                    await new PlayMixTask() { ArtistName = name }.Show().ConfigureAwait(false);
                    return;
                }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Launches MixRadio to show details about the product using the ShowProductTask
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>An async task to await</returns>
        public static async Task Show(this Product product)
        {
            ShowProductTask task = new ShowProductTask() { ProductId = product.Id };
            await task.Show().ConfigureAwait(false);
        }
    }
}
