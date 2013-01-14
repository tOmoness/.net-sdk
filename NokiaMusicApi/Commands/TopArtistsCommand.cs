// -----------------------------------------------------------------------
// <copyright file="TopArtistsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    ///   Gets the top artists for a genre
    /// </summary>
    internal sealed class TopArtistsCommand : SearchCatalogCommand<Artist>
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.InternalSearch(null, null, Category.Artist, null, null, this.StartIndex, this.ItemsPerPage, Artist.FromJToken, this.Callback);
        }
    }
}