// -----------------------------------------------------------------------
// <copyright file="PlayMixTask.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MixRadio.Types;

namespace MixRadio.Tasks
{
    /// <summary>
    /// Provides a simple way to play a MixRadio Mix
    /// </summary>
    public sealed class PlayMixTask : TaskBase
    {
        private string _mixId = null;
        private string _artistId = null;
        private string _artistName = null;

        /// <summary>
        /// Gets or sets the Artist ID.
        /// </summary>
        /// <value>
        /// The artist ID.
        /// </value>
        /// <remarks>You need to supply an ID or a name</remarks>
        public string ArtistId
        {
            get
            {
                return this._artistId;
            }

            set
            {
                this._artistId = value;
            }
        }

        /// <summary>
        /// Gets or sets the Artist Name.
        /// </summary>
        /// <value>
        /// The artist Name.
        /// </value>
        /// <remarks>You need to supply a Mix ID, an Artist Id or an Artist Name</remarks>
        public string ArtistName
        {
            get
            {
                return this._artistName;
            }

            set
            {
                this._artistName = value;
            }
        }

        /// <summary>
        /// Gets or sets a Mix ID.
        /// </summary>
        /// <value>
        /// The mix ID.
        /// </value>
        /// <remarks>You need to supply a Mix ID, an Artist Id  or an Artist Name</remarks>
        public string MixId
        {
            get
            {
                return this._mixId;
            }

            set
            {
                this._mixId = value;
            }
        }

        /// <summary>
        /// Plays the Mix in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            if (!string.IsNullOrEmpty(this.MixId))
            {
                var mix = new Mix { Id = this.MixId };
                await this.Launch(mix.AppToAppUri, mix.WebUri).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(this._artistId))
            {
                var artist = new Artist { Id = this._artistId };
                await this.Launch(artist.AppToAppPlayUri, artist.WebUri).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(this._artistName))
            {
                var artist = new Artist { Name = this._artistName };
                await this.Launch(artist.AppToAppPlayUri, artist.WebUri).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Please set a mix ID, artist id or artist name before calling Show()");
            }
        }
    }
}
