// -----------------------------------------------------------------------
// <copyright file="Location.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Types
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
        /// Gets or sets the name of the locaiton if available.
        /// </summary>
        /// <value>
        /// The name of the location if available.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the altitude.
        /// </summary>
        /// <value>
        /// The altitude.
        /// </value>
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        /// <value>
        /// The course.
        /// </value>
        public double? Course { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public double? Speed { get; set; }

        /// <summary>
        /// Gets or sets the horizontal accuracy.
        /// </summary>
        /// <value>
        /// The horizontal accuracy.
        /// </value>
        public int? HorizontalAccuracy { get; set; }

        /// <summary>
        /// Gets or sets the vertical accuracy.
        /// </summary>
        /// <value>
        /// The vertical accuracy.
        /// </value>
        public int? VerticalAccuracy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the cell id.
        /// </summary>
        /// <value>
        /// The cell id.
        /// </value>
        public string CellId { get; set; }

        /// <summary>
        /// Gets or sets the place id.
        /// </summary>
        /// <value>
        /// The place id.
        /// </value>
        public string PlaceId { get; set; }

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
