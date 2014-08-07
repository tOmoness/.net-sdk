// -----------------------------------------------------------------------
// <copyright file="MixDetailsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    internal sealed class MixDetailsCommand : JsonMusicClientCommand<Response<Mix>>
    {
        /// <summary>
        /// Gets or sets the mix id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                throw new ArgumentNullException("Id", "A mix id must be supplied");
            }

            uri.AppendFormat("mixes/stations/{0}/", this.Id);
        }

        internal override Response<Mix> HandleRawResponse(Response<Newtonsoft.Json.Linq.JObject> rawResponse)
        {
            return this.ItemResponseHandler(rawResponse, Mix.FromJToken);
        }
    }
}
