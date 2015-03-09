// -----------------------------------------------------------------------
// <copyright file="MixesCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    ///   Gets the Mixes available in a group
    /// </summary>
    internal sealed class MixesCommand : JsonMusicClientCommand<ListResponse<Mix>>
    {
        private const string ArrayNameRadioStations = "radiostations";

        /// <summary>
        ///   Gets or sets the mix group exclusive tag.
        /// </summary>
        public string ExclusiveTag { get; set; }

        /// <summary>
        ///   Gets or sets the mix group exclusivity tokens.
        /// </summary>
        public string[] Exclusivity { get; set; }

        /// <summary>
        ///   Gets or sets the mix group id.
        /// </summary>
        public string MixGroupId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the showFeaturedArtist flag is set. 
        /// </summary>
        public bool ShowFeaturedArtists { get; set; } 

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (!string.IsNullOrEmpty(this.MixGroupId))
            {
                // Get mixes in a group
                uri.AppendFormat("mixes/groups/{0}/", this.MixGroupId);
            }
            else
            {
                // Get all mixes
                uri.Append("mixes/stations/");
            }
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

            if (this.ShowFeaturedArtists)
            {
                parameters.Add(new KeyValuePair<string, string>("view", "topftartists"));
            }

            return parameters;
        }

        internal override ListResponse<Mix> HandleRawResponse(Response<Newtonsoft.Json.Linq.JObject> rawResponse)
        {
            if (!string.IsNullOrEmpty(this.MixGroupId))
            {
                // mixes in a group
                return this.ListItemResponseHandler(rawResponse, ArrayNameRadioStations, Mix.FromJToken);
            }
            else
            {
                // all mixes
                return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Mix.FromJToken);
            }
        }
    }
}