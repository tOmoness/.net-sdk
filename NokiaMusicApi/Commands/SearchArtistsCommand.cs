// -----------------------------------------------------------------------
// <copyright file="SearchArtistsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Searches for an Artist
    /// </summary>
    internal sealed class SearchArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <exception cref="System.ArgumentNullException">SearchTerm;A searchTerm must be supplied</exception>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                throw new ArgumentNullException("SearchTerm", "A searchTerm must be supplied");
            }

            this.InternalSearch(this.SearchTerm, null, Category.Artist, this.StartIndex, this.ItemsPerPage, Artist.FromJToken, this.Callback);
        }
    }
}