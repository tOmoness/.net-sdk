// -----------------------------------------------------------------------
// <copyright file="MusicSearchTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia Music Search Results
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
        /// Shows the Search Page in Nokia Music
        /// </summary>
        public void Show()
        {
            if (!string.IsNullOrEmpty(this._searchTerms))
            {
                string encodedSearch = Uri.EscapeDataString(this.SearchTerms);
                this.Launch(
                    new Uri("nokia-music://search/anything/?term=" + encodedSearch),
                    new Uri("http://music.nokia.com/r/search/" + encodedSearch));
            }
            else
            {
                throw new InvalidOperationException("Please set the search term before calling Show()");
            }
        }
    }
}
