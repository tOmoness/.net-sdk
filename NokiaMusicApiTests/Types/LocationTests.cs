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
        private const double TestAltitude = 3.4567;
        private const double TestCourse = 4.5678;
        private const double TestSpeed = 5.67890;
        private const int TestHorizontalAccuracy = 6;
        private const int TestVerticalAccuracy = 7;
        private const long TestTimestampTicks = 123456;
        private const string TestCellId = "a";
        private const string TestPlaceId = "b";

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
                                    Altitude = TestAltitude,
                                    Course = TestCourse,
                                    Speed = TestSpeed,
                                    HorizontalAccuracy = TestHorizontalAccuracy,
                                    VerticalAccuracy = TestVerticalAccuracy,
                                    Timestamp = new DateTime(TestTimestampTicks),
                                    CellId = TestCellId,
                                    PlaceId = TestPlaceId,
                                };
            Assert.AreEqual(TestLatitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(TestLongitude, location.Longitude, "Expected the property to persist");
            Assert.AreEqual(TestAltitude, location.Altitude, "Expected the property to persist");
            Assert.AreEqual(TestCourse, location.Course, "Expected the property to persist");
            Assert.AreEqual(TestSpeed, location.Speed, "Expected the property to persist");
            Assert.AreEqual(TestHorizontalAccuracy, location.HorizontalAccuracy, "Expected the property to persist");
            Assert.AreEqual(TestVerticalAccuracy, location.VerticalAccuracy, "Expected the property to persist");
            Assert.AreEqual(TestTimestampTicks, location.Timestamp.GetValueOrDefault().Ticks, "Expected the property to persist");
            Assert.AreEqual(TestCellId, location.CellId, "Expected the property to persist");
            Assert.AreEqual(TestPlaceId, location.PlaceId, "Expected the property to persist");
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
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude, Altitude = TestAltitude, Course = TestCourse, Speed = TestSpeed, HorizontalAccuracy = TestHorizontalAccuracy, VerticalAccuracy = TestVerticalAccuracy };
            GeoCoordinate coord = location.ToGeoCoordinate();
            Assert.AreEqual(location.Latitude, coord.Latitude, "Expected same Latitude");
            Assert.AreEqual(location.Longitude, coord.Longitude, "Expected same Longitude");
            Assert.AreEqual(location.Altitude, coord.Altitude, "Expected same Altitude");
            Assert.AreEqual(location.Course, coord.Course, "Expected same Course");
            Assert.AreEqual(location.Speed, coord.Speed, "Expected same Speed");
            Assert.AreEqual(location.HorizontalAccuracy, coord.HorizontalAccuracy, "Expected same Horizontal Accuracy");
            Assert.AreEqual(location.VerticalAccuracy, coord.VerticalAccuracy, "Expected same Vertical Accuracy");
        }
    }
}
