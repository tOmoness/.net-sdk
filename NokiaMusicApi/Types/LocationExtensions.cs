// -----------------------------------------------------------------------
// <copyright file="LocationExtensions.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Device.Location;

namespace Nokia.Music.Phone.Types
{
    internal static class LocationExtensions
    {
        /// <summary>
        /// Turns a Location into a GeoCoordinate
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>
        /// A GeoCoordinate representation
        /// </returns>
        /// <remarks>
        /// Done as an extension to keep the api as portable as possible
        /// </remarks>
        internal static GeoCoordinate ToGeoCoordinate(this Location location)
        {
            return new GeoCoordinate(
                location.Latitude,
                location.Longitude,
                location.Altitude.GetValueOrDefault(),
                location.HorizontalAccuracy.GetValueOrDefault(),
                location.VerticalAccuracy.GetValueOrDefault(),
                location.Speed.GetValueOrDefault(),
                location.Course.GetValueOrDefault());
        }
    }
}
