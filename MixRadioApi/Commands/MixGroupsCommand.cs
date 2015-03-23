// -----------------------------------------------------------------------
// <copyright file="MixGroupsCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
{
    /// <summary>
    /// Gets the Mix Groups available
    /// </summary>
    internal sealed class MixGroupsCommand : JsonMusicClientCommand<ListResponse<MixGroup>>
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

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            var parameters = this.GetPagingParams();

            if (!string.IsNullOrEmpty(this.ExclusiveTag))
            {
                parameters.Add(new KeyValuePair<string, string>(ParamExclusive, this.ExclusiveTag));
            }

            if (this.Exclusivity != null)
            {
                parameters.AddRange(this.Exclusivity
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new KeyValuePair<string, string>(ParamExclusivity, x)));
            }

            return parameters;
        }

        internal override ListResponse<MixGroup> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, MixGroup.FromJToken);
        }
    }
}