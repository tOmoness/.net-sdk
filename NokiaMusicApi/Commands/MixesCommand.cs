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
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        /// <param name="pathParams">The API method parameters</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri, Dictionary<string, string> pathParams)
        {
            if (pathParams != null && pathParams.ContainsKey("id"))
            {
                uri.AppendFormat("mixes/groups/{0}/", pathParams["id"]);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

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
                this,
                this.MusicClientSettings,
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