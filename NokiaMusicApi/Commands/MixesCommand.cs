// -----------------------------------------------------------------------
// <copyright file="MixesCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets the Mixes available in a group
    /// </summary>
    internal sealed class MixesCommand : MusicClientCommand<ListResponse<Mix>>
    {
        private const string ArrayNameRadioStations = "radiostations";

        /// <summary>
        ///   Gets or sets the mix group exclusive tag.
        /// </summary>
        public string ExclusiveTag { get; set; }

        /// <summary>
        ///   Gets or sets the mix group id.
        /// </summary>
        public string MixGroupId { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("mixes/groups/{0}/", this.MixGroupId);
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
                this.BuildQueryString(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, ArrayNameRadioStations, Mix.FromJToken, this.Callback)));
        }

        /// <summary>
        /// Builds the querystring parameters
        /// </summary>
        /// <returns>The querystring parameters</returns>
        private List<KeyValuePair<string, string>> BuildQueryString()
        {
            var qs = this.GetPagingParams();
            if (!string.IsNullOrEmpty(this.ExclusiveTag))
            {
                qs.Add(new KeyValuePair<string, string>(ParamExclusive, this.ExclusiveTag));
            }

            return qs;
        }
    }
}