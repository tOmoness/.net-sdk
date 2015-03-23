// -----------------------------------------------------------------------
// <copyright file="LocationConverterTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Device.Location;
using MixRadio.Converters;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests
{
    [TestFixture]
    public class LocationConverterTests
    {
        private const double TestLatitude = 1.2345;
        private const double TestLongitude = 2.3456;

        [Test]
        public void EnsureLocationConverterConvertsToAGeoCoordinate()
        {
            Location location = new Location() { Latitude = TestLatitude, Longitude = TestLongitude };
            LocationConverter converter = new LocationConverter();
            GeoCoordinate coord = converter.Convert(location, typeof(GeoCoordinate), null, null) as GeoCoordinate;

            Assert.AreEqual(location.Latitude, coord.Latitude, "Expected same Latitude");
            Assert.AreEqual(location.Longitude, coord.Longitude, "Expected same Longitude");
        }

        [Test]
        public void EnsureLocationConverterReturnsNullForBadType()
        {
            LocationConverter converter = new LocationConverter();
            Assert.IsNull(converter.Convert(string.Empty, null, null, null), "Expected null result");
        }

        [Test]
        public void EnsureLocationConverterReturnsNullForConvertBack()
        {
            LocationConverter converter = new LocationConverter();
            Assert.IsNull(converter.ConvertBack(string.Empty, null, null, null), "Expected null result");
        }
    }
}
