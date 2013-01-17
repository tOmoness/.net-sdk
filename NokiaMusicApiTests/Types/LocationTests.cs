// -----------------------------------------------------------------------
// <copyright file="LocationTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
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

        [Test]
        public void TestProperties()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            Assert.AreEqual(TestLatitude, location.Latitude, "Expected the property to persist");
            Assert.AreEqual(TestLongitude, location.Longitude, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, Location.LocationFormat, location.Latitude, location.Longitude), location.ToString(), "Expected format to be the same");
        }
    }
}
