// -----------------------------------------------------------------------
// <copyright file="MixGroupsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets the Mix Groups available
    /// </summary>
    internal sealed class MixGroupsCommand : MusicClientCommand<ListResponse<MixGroup>>
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                ApiMethod.MixGroups,
                this.MusicClientSettings.AppId,
                this.MusicClientSettings.AppCode,
                this.MusicClientSettings.CountryCode,
                null,
                new Dictionary<string, string>
                    {
                        { PagingStartIndex, StartIndex.ToString(CultureInfo.InvariantCulture) },
                        { PagingItemsPerPage, ItemsPerPage.ToString(CultureInfo.InvariantCulture) }
                    },
                rawResult => this.CatalogItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, MixGroup.FromJToken, this.Callback));
        }
    }
}