// -----------------------------------------------------------------------
// <copyright file="ShowArtistTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show MixRadio Artists
    /// </summary>
    public sealed class ShowArtistTask : TaskBase
    {
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
        /// <remarks>You need to supply an ID or a name</remarks>
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
        /// Shows the Artist Page in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            if (!string.IsNullOrEmpty(this._artistId))
            {
                await this.Launch(
                    new Uri(string.Format(Artist.AppToAppShowUri, this._artistId)),
                    new Uri(string.Format(Artist.WebShowUri, this._artistId))).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(this._artistName))
            {
                await this.Launch(
                    new Uri(string.Format(Artist.AppToAppShowUriByName, this._artistName.Replace("&", string.Empty))),
                    new Uri(string.Format(Artist.WebPlayUriByName, this._artistName.Replace("&", string.Empty)))).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Please set an artist ID or name before calling Show()");
            }
        }
    }
}
