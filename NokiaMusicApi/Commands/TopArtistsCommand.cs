// -----------------------------------------------------------------------
// <copyright file="TopArtistsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets top artists
    /// </summary>
    internal sealed class TopArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Gets or sets the genre ID to get results for.
        /// </summary>
        public string GenreId { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.InternalSearch(null, this.GenreId, null, Types.Category.Artist, null, null, null, null, this.StartIndex, this.ItemsPerPage, Artist.FromJToken, this.Callback);
        }
    }
}