// -----------------------------------------------------------------------
// <copyright file="GetUserTopArtistsCommand.cs" company="MixRadio">
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
    /// Command to get user artist chart for the last week
    /// </summary>
    internal class GetUserTopArtistsCommand : JsonMusicClientCommand<ListResponse<Artist>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserTopArtistsCommand"/> class.
        /// </summary>
        public GetUserTopArtistsCommand()
        {
            this.StartDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;
        }

        /// <summary>
        /// Gets or sets the end date used to build the chart.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        /// <remarks>Charts are available for the last week</remarks>
        internal DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date used to build the chart.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        /// <remarks>Charts are available for the last week</remarks>
        internal DateTime StartDate { get; set; }

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
                uri.AppendFormat("users/{0}/history/creators/", this.UserId);
            }
            else
            {
                throw new ArgumentNullException("UserId");
            }
        }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            var parameters = this.GetPagingParams();

            DateTime earliestStartDate = DateTime.UtcNow.Date.AddDays(-7);
            DateTime latestEndDate = DateTime.UtcNow.Date.AddDays(-1);

            if (this.StartDate < earliestStartDate)
            {
                this.StartDate = earliestStartDate;
            }

            if (this.EndDate > latestEndDate)
            {
                this.EndDate = latestEndDate;
            }

            parameters.Add(new KeyValuePair<string, string>("startdate", this.StartDate.ToString("yyyy-MM-dd")));
            parameters.Add(new KeyValuePair<string, string>("enddate", this.EndDate.ToString("yyyy-MM-dd")));
         
            return parameters;
        }

        internal override ListResponse<Artist> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, Artist.FromJToken);
        }
    }
}
