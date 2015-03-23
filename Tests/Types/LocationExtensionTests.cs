// -----------------------------------------------------------------------
// <copyright file="LocationExtensionTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Device.Location;
using System.Globalization;
using MixRadio.Types;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MixRadio.Tests.Types
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
