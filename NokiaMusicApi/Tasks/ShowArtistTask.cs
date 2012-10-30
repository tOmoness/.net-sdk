// -----------------------------------------------------------------------
// <copyright file="ShowArtistTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia Music Artists
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
        /// Shows the Artist Page in Nokia Music
        /// </summary>
        public void Show()
        {
            if (!string.IsNullOrEmpty(this._artistId))
            {
                this.Launch(
                    new Uri("nokia-music://show/artist/?id=" + this._artistId),
                    new Uri("http://music.nokia.com/r/artist/-/" + this._artistId));
            }
            else if (!string.IsNullOrEmpty(this._artistName))
            {
                this.Launch(
                    new Uri("nokia-music://show/artist/?name=" + this._artistName),
                    new Uri("http://music.nokia.com/r/search/" + this._artistName));
            }
            else
            {
                throw new InvalidOperationException("Please set an artist ID or name before calling Show()");
            }
        }
    }
}
