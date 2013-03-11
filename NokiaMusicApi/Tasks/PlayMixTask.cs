// -----------------------------------------------------------------------
// <copyright file="PlayMixTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to play a Nokia Music Mix
    /// </summary>
    public sealed class PlayMixTask : TaskBase
    {
        private string _mixId = null;
        private string _artistName = null;

        /// <summary>
        /// Gets or sets the Artist Name.
        /// </summary>
        /// <value>
        /// The artist Name.
        /// </value>
        /// <remarks>You need to supply a Mix ID or an Artist Name</remarks>
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
        /// <remarks>You need to supply a Mix ID or an Artist Name</remarks>
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
        /// Plays the Mix in Nokia Music
        /// </summary>
        public void Show()
        {
            if (!string.IsNullOrEmpty(this._mixId))
            {
                this.Launch(
                    new Uri(string.Format(Mix.AppToAppShow, this._mixId)),
                    new Uri("http://music.nokia.com/"));
            }
            else if (!string.IsNullOrEmpty(this._artistName))
            {
                this.Launch(
                    new Uri("nokia-music://play/artist/?artist=" + this._artistName),
                    new Uri("http://music.nokia.com/r/search/" + this._artistName));
            }
            else
            {
                throw new InvalidOperationException("Please set a mix ID or artist name before calling Show()");
            }
        }
    }
}
