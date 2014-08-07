﻿// -----------------------------------------------------------------------
// <copyright file="MusicSearchTask.cs" company="Nokia">
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
    /// Provides a simple way to show MixRadio Search Results
    /// </summary>
    public sealed class MusicSearchTask : TaskBase
    {
        private string _searchTerms = null;

        /// <summary>
        /// Gets or sets the search terms.
        /// </summary>
        /// <value>
        /// The search terms.
        /// </value>
        public string SearchTerms
        {
            get
            {
                return this._searchTerms;
            }

            set
            {
                this._searchTerms = value;
            }
        }

        /// <summary>
        /// Shows the Search Page in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            if (!string.IsNullOrEmpty(this._searchTerms))
            {
#if WINDOWS_APP
                var appUri = new Uri("nokia-music://search/anything/?term=" + this._searchTerms);
#else
                var appUri = new Uri("mixradio://search/anything/" + this._searchTerms);
#endif
                // Fall back to artist mix
                await this.Launch(
                    appUri,
                    new Uri(string.Format(Artist.WebPlayUriByName, this._searchTerms.Replace("&", string.Empty)))).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Please set the search term before calling Show()");
            }
        }
    }
}
