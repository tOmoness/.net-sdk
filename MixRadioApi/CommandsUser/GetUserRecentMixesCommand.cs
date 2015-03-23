// -----------------------------------------------------------------------
// <copyright file="GetUserRecentMixesCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio.Commands
{
    /// <summary>
    /// Command to get user recent mixes for the last week
    /// </summary>
    internal class GetUserRecentMixesCommand : JsonMusicClientCommand<ListResponse<Mix>>
    {
        /// <summary>
        /// Gets a value indicating whether the API method requires a country code to be specified.
        /// API methods require a country code by default. Override this method for calls that do not.
        /// </summary>
        internal override bool RequiresCountryCode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// By default, no path is added, override this to add a uri subpath for a method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(StringBuilder uri)
        {
            if (!string.IsNullOrEmpty(this.UserId))
            {
                uri.AppendFormat("users/{0}/history/mixes/", this.UserId);
            }
            else
            {
                throw new ArgumentNullException("UserId");
            }
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return this.GetPagingParams();
        }

        internal override ListResponse<Mix> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Mix.FromJToken);
        }
    }
}