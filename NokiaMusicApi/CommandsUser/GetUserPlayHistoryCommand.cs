// -----------------------------------------------------------------------
// <copyright file="GetUserPlayHistoryCommand.cs" company="Nokia">
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
    /// Command to get user playback history
    /// </summary>
    internal class GetUserPlayHistoryCommand : SecureMusicClientCommand<ListResponse<UserEvent>>
    {
        /// <summary>
        /// Gets or sets the action type to filter results by.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        internal UserEventAction? Action { get; set; }

        /// <summary>
        /// Gets the target type to filter results by.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        internal UserEventTarget Target
        {
            get
            {
                return UserEventTarget.Track;
            }
        }
        
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
                uri.AppendFormat("users/{0}/history/", this.UserId);
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

            if (this.Action != null && this.Action.HasValue && this.Action.Value != UserEventAction.Unknown)
            {
                parameters.Add(new KeyValuePair<string, string>("action", this.Action.Value.ToString().ToLowerInvariant()));
            }

            if (this.Target != UserEventTarget.Unknown)
            {
                parameters.Add(new KeyValuePair<string, string>("target", this.Target.ToString().ToLowerInvariant()));
            }

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
                new JsonResponseCallback(rawResult => this.ListItemResponseHandler(rawResult, MusicClientCommand.ArrayNameItems, UserEvent.FromJToken, this.Callback)),
                this.OAuth2.CreateHeaders());
        }
    }
}
