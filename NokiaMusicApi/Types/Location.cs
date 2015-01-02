// -----------------------------------------------------------------------
// <copyright file="Location.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Location
    /// </summary>
    public sealed partial class Location
    {
        internal const string LocationFormat = "{0:#0.00000},{1:#0.00000}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Location" /> class.
        /// </summary>
        public Location()
        {
        }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the name of the location if available.
        /// </summary>
        /// <value>
        /// The name of the location if available.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Returns a string representation of the location object
        /// </summary>
        /// <returns>A string representation of the location object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, LocationFormat, this.Latitude, this.Longitude);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Location target = obj as Location;
            if (target != null)
            {
                return string.Compare(target.ToString(), this.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Creates a Location from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Location object</returns>
        internal static Location FromJToken(JToken item)
        {
            if (item == null || item.Type == JTokenType.Null)
            {
                return null;
            }

            return new Location()
            {
                Latitude = item.Value<double>("lat"),
                Longitude = item.Value<double>("lng"),
                Name = item.Value<string>("name")
            };
        }
    }
}
