// -----------------------------------------------------------------------
// <copyright file="GetUserPlayHistoryCommand.cs" company="MixRadio">
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
    /// Command to get user playback history
    /// </summary>
    internal class GetUserPlayHistoryCommand : JsonMusicClientCommand<ListResponse<UserEvent>>
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

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            var parameters = this.GetPagingParams();
            this.AddActionParameters(parameters);

            if (this.Target != UserEventTarget.Unknown)
            {
                parameters.Add(new KeyValuePair<string, string>("target", this.Target.ToString().ToLowerInvariant()));
            }

            return parameters;
        }
        
        /// <summary>
        /// Adds all the action parameters as the Enum is a flag
        /// </summary>
        /// <param name="queryParams">The parameters</param>
        internal void AddActionParameters(List<KeyValuePair<string, string>> queryParams)
        {
            if (this.Action != null && this.Action.HasValue && this.Action.Value != UserEventAction.Unknown)
            {
                // We need to get all possible actions and then check if the requested action contains it
                Type type = typeof(UserEventAction);
#if NETFX_CORE || UNIT_TESTS || PORTABLE || SILVERLIGHT || WINDOWS_PHONE
                var allActions = (UserEventAction[])Enum.GetValues(type);
#else
                IEnumerable<FieldInfo> fields = type.GetFields().Where(field => field.IsLiteral);
                var allActions = fields.Select(field => field.GetValue(type)).Select(value => (UserEventAction)value);
#endif

                foreach (UserEventAction action in allActions)
                {
                    if (action != UserEventAction.Unknown)
                    {
                        if ((this.Action.Value & action) == action)
                        {
                            queryParams.Add(new KeyValuePair<string, string>("action", action.ToString().ToLowerInvariant()));
                        }
                    }
                }
            }
        }

        internal override ListResponse<UserEvent> HandleRawResponse(Response<JObject> rawResponse)
        {
            return this.ListItemResponseHandler(rawResponse, MusicClientCommand.ArrayNameItems, UserEvent.FromJToken);
        }
    }
}
