// -----------------------------------------------------------------------
// <copyright file="GetUserTopArtistsCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Internal.Response;
using Nokia.Music.Types;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Command to get user artist chart for the last week
    /// </summary>
    internal class GetUserTopArtistsCommand : SecureMusicClientCommand<ListResponse<Artist>>
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

        /// <summary>
        /// Builds the querystring params.
        /// </summary>
        /// <returns>A list of key/value pairs</returns>
        internal List<KeyValuePair<string, string>> BuildParams()
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

        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                this.BuildParams(),
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, Artist.FromJToken, this.Callback)),
                this.OAuth2.CreateHeaders());
        }
    }
}
