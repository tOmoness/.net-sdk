// -----------------------------------------------------------------------
// <copyright file="UserEvent.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a user playback event
    /// </summary>
    public sealed partial class UserEvent
    {
        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the client version.
        /// </summary>
        /// <value>
        /// The client version.
        /// </value>
        public string ClientVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the client type.
        /// </summary>
        /// <value>
        /// A value from the UserEventClientType enumeration.
        /// </value>
        public UserEventClientType ClientType { get; set; }

        /// <summary>
        /// Gets or sets the date time of the event in UTC.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the action type.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public UserEventAction Action { get; set; }

        /// <summary>
        /// Gets or sets the target type.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public UserEventTarget Target { get; set; }

        /// <summary>
        /// Gets or sets the offset the event happened at.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the location the event happened at.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Location Location { get; set; }

        /// <summary>
        /// Gets or sets the product the event is about.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the mix the event is about.
        /// </summary>
        /// <value>
        /// The mix.
        /// </value>
        public Mix Mix { get; set; }

        /// <summary>
        /// Creates a UserEvent from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A UserEvent object</returns>
        internal static UserEvent FromJToken(JToken item)
        {
            return new UserEvent
            {
                Action = ParseHelper.ParseEnumOrDefault<UserEventAction>(item.Value<string>("action")),
                Client = item.Value<string>("client"),
                ClientVersion = item.Value<string>("clientversion"),
                DateTime = item.Value<DateTime>("datetime"),
                Location = Location.FromJToken(item["location"]),
                Mix = Mix.FromJToken(item["mix"]),
                Offset = item.Value<int>("offset"),
                Product = Product.FromJToken(item["product"]),
                Target = ParseHelper.ParseEnumOrDefault<UserEventTarget>(item.Value<string>("target")),
                ClientType = ParseHelper.ParseEnumOrDefault<UserEventClientType>(item.Value<string>("clienttype"))
            };
        }
    }
}