// -----------------------------------------------------------------------
// <copyright file="TopArtistsForGenreCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets the top artists for a genre
    /// </summary>
    internal sealed class TopArtistsForGenreCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the genre ID to get results for.
        /// </summary>
        public string GenreId { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <exception cref="System.ArgumentNullException">GenreId;A genre ID must be supplied</exception>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.GenreId))
            {
                throw new ArgumentNullException("GenreId", "A genre ID must be supplied");
            }

            this.InternalSearch(null, this.GenreId, Category.Artist, null, null, this.StartIndex, this.ItemsPerPage, Artist.FromJToken, this.Callback);
        }
    }
}