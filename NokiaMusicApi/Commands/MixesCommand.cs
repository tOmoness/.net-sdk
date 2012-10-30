// -----------------------------------------------------------------------
// <copyright file="MixesCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    ///   Gets the Mixes available in a group
    /// </summary>
    internal sealed class MixesCommand : MusicClientCommand<ListResponse<Mix>>
    {
        private const string ArrayNameRadioStations = "radiostations";
        
        /// <summary>
        ///   Gets or sets the mix group id.
        /// </summary>
        public string MixGroupId { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            if (string.IsNullOrEmpty(this.MixGroupId))
            {
                throw new ArgumentNullException("MixGroupId", "A group id must be supplied");
            }

            RequestHandler.SendRequestAsync(
                ApiMethod.Mixes,
                MusicClientSettings.AppId,
                MusicClientSettings.AppCode,
                MusicClientSettings.CountryCode,
                new Dictionary<string, string>
                    {
                        { ParamId, this.MixGroupId }
                    },
                new Dictionary<string, string>
                    {
                        { PagingStartIndex, StartIndex.ToString(CultureInfo.InvariantCulture) },
                        { PagingItemsPerPage, ItemsPerPage.ToString(CultureInfo.InvariantCulture) }
                    },
                rawResult => this.CatalogItemResponseHandler(rawResult, ArrayNameRadioStations, Mix.FromJToken, this.Callback));
        }
    }
}