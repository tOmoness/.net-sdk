// -----------------------------------------------------------------------
// <copyright file="ShowGigsTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show MixRadio Gigs
    /// </summary>
    public sealed class ShowGigsTask : TaskBase
    {
        private string _searchTerms = null;

        /// <summary>
        /// Gets or sets optional search terms, such as an artist or city.
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
        /// Shows Gigs in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            if (!string.IsNullOrEmpty(this._searchTerms))
            {
                // No need to URI encode this one
                await this.Launch(
                    new Uri("mixradio://search/gigs/" + this.SearchTerms),
                    new Uri("http://www.mixrad.io/")).ConfigureAwait(false);
            }
            else
            {
                await this.Launch(
                    new Uri("mixradio://show/gigs/"),
                    new Uri("http://www.mixrad.io/")).ConfigureAwait(false);
            }
        }
    }
}
