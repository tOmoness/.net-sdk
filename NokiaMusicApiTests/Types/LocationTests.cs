// -----------------------------------------------------------------------
// <copyright file="LocationTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Device.Location;
using System.Globalization;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// Location tests
    /// </summary>
    [TestFixture]
    public class LocationTests
    {
        private const double TestLatitude = 1.2345;
        private const double TestLongitude = 2.3456;
        private const int TestHorizontalAccuracy = 6;
        private const long TestTimestampTicks = 123456;

        [Test]
        public void TestProperties()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            Assert.AreEqual(TestLatitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(TestLongitude, location.Longitude, "Expected the property to persist");
        }

        [Test]
        public void TestFullProperties()
        {
            Location location = new Location()
                                {
                                    Latitude = TestLatitude,
                                    Longitude = TestLongitude,
                                    HorizontalAccuracy = TestHorizontalAccuracy,
                                    Timestamp = new DateTime(TestTimestampTicks)
                                };
            Assert.AreEqual(TestLatitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(TestLongitude, location.Longitude, "Expected the property to persist");
            Assert.AreEqual(TestHorizontalAccuracy, location.HorizontalAccuracy, "Expected the property to persist");
            Assert.AreEqual(TestTimestampTicks, location.Timestamp.GetValueOrDefault().Ticks, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, Location.LocationFormat, location.Latitude, location.Longitude), location.ToString(), "Expected format to be the same");
        }

        [Test]
        public void TestConversion()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude, HorizontalAccuracy = TestHorizontalAccuracy };
            GeoCoordinate coord = location.ToGeoCoordinate();
            Assert.AreEqual(location.Latitude, coord.Latitude, "Expected same Latitude");
            Assert.AreEqual(location.Longitude, coord.Longitude, "Expected same Longitude");
            Assert.AreEqual(location.HorizontalAccuracy, coord.HorizontalAccuracy, "Expected same Horizontal Accuracy");
        }
    }
}
