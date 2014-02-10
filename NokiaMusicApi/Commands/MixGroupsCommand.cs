// -----------------------------------------------------------------------
// <copyright file="MixGroupsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
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
        ///   Gets or sets the mix group exclusivity tokens.
        /// </summary>
        public string[] Exclusivity { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("mixes/groups/");
        }

        /// <summary>
        /// Builds the querystring parameters
        /// </summary>
        /// <returns>The querystring parameters</returns>
        internal List<KeyValuePair<string, string>> BuildQueryString()
        {
            var qs = this.GetPagingParams();

            if (!string.IsNullOrEmpty(this.ExclusiveTag))
            {
                qs.Add(new KeyValuePair<string, string>(ParamExclusive, this.ExclusiveTag));
            }

            if (this.Exclusivity != null)
            {
                qs.AddRange(this.Exclusivity
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new KeyValuePair<string, string>(ParamExclusivity, x)));
            }

            return qs;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                this.BuildQueryString(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, MixGroup.FromJToken, this.Callback)));
        }
    }
}