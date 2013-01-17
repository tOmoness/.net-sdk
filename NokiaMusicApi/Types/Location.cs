// -----------------------------------------------------------------------
// <copyright file="Location.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Location
    /// </summary>
    public class Location
    {
        internal const string LocationFormat = "{0:#0.00000},{1:#0.00000}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Location" /> class.
        /// </summary>
        internal Location()
        {
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude { get; internal set; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public double Longitude { get; internal set; }

        /// <summary>
        /// Returns a string representation of the location object
        /// </summary>
        /// <returns>A string representation of the location object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, LocationFormat, this.Latitude, this.Longitude);
        }
    }
}
