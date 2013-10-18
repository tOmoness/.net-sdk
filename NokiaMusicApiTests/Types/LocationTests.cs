// -----------------------------------------------------------------------
// <copyright file="LocationTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
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
    public class LocationTests
    {
        public const double TestLatitude = 1.2345;
        public const double TestLongitude = 2.3456;
        private const string TestName = "name";

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
                                    Name = TestName
                                };
            Assert.AreEqual(TestLatitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(TestLongitude, location.Longitude, "Expected the property to persist");
            Assert.AreEqual(TestName, location.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, Location.LocationFormat, location.Latitude, location.Longitude), location.ToString(), "Expected format to be the same");
            Assert.IsNotNull(location.GetHashCode(), "Expected a hash code");
            Assert.IsFalse(location.Equals(TestLatitude), "Expected inequality");
        }

        [Test]
        public void TestConversion()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            GeoCoordinate coord = location.ToGeoCoordinate();
            Assert.AreEqual(location.Latitude, coord.Latitude, "Expected same Latitude");
            Assert.AreEqual(location.Longitude, coord.Longitude, "Expected same Longitude");
        }

        [Test]
        public void TestJsonParsing()
        {
            Assert.IsNull(Location.FromJToken(null), "Expected a null return");

            Location location = new Location()
            {
                Latitude = TestLatitude,
                Longitude = TestLongitude,
                Name = TestName
            };
            JObject json = JObject.Parse("{\"lat\":1.2345,\"lng\":2.3456,\"name\":\"name\"}");
            Location fromJson = Location.FromJToken(json);

            Assert.IsNotNull(fromJson, "Expected a event object");

            Assert.AreEqual(fromJson.Latitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(fromJson.Longitude, location.Longitude, "Expected the property to persist");
            Assert.AreEqual(fromJson.Name, location.Name, "Expected the property to persist");
        }
    }
}
