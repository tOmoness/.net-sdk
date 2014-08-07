// -----------------------------------------------------------------------
// <copyright file="LocationExtensionTests.cs" company="NOKIA">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Device.Location;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// Location tests
    /// </summary>
    [TestFixture]
    public class LocationExtensionTests
    {
        [Test]
        public void TestConversion()
        {
            Location location = new Location() { Latitude = LocationTests.TestLatitude, Longitude = LocationTests.TestLongitude };
            GeoCoordinate coord = location.ToGeoCoordinate();
            Assert.AreEqual(location.Latitude, coord.Latitude, "Expected same Latitude");
            Assert.AreEqual(location.Longitude, coord.Longitude, "Expected same Longitude");
        }
    }
}
