// -----------------------------------------------------------------------
// <copyright file="MixGroupsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Request;
using Nokia.Music.Phone.Internal.Response;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets the Mix Groups available
    /// </summary>
    internal sealed class MixGroupsCommand : MusicClientCommand<ListResponse<MixGroup>>
    {
        /// <summary>
        ///   Gets or sets the mix group exclusive tag.
        /// </summary>
        public string ExclusiveTag { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("mixes/groups/");
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            var queryParams = this.GetPagingParams();
            if (!string.IsNullOrEmpty(this.ExclusiveTag))
            {
                queryParams.Add(new KeyValuePair<string, string>(ParamExclusive, this.ExclusiveTag));
            }

            this.RequestHandler.SendRequestAsync(
                this,
                this.MusicClientSettings,
                queryParams,
                new JsonResponseCallback(rawResult => this.CatalogItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, MixGroup.FromJToken, this.Callback)));
        }
    }
}