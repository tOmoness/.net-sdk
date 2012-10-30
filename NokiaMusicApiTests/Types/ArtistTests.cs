// -----------------------------------------------------------------------
// <copyright file="ArtistTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// Artist tests
    /// </summary>
    [TestFixture]
    public class ArtistTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            Artist artist = new Artist() { Id = TestId, Name = TestName };

            Assert.AreEqual(TestId, artist.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, artist.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Artist artist = new Artist() { Id = TestId, Name = TestName };
            Assert.IsNotNull(artist.GetHashCode(), "Expected a hash code");
            Assert.IsTrue(artist.Equals(new Artist() { Id = TestId }), "Expected equality");
            Assert.IsFalse(artist.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void TestJsonParsing()
        {
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.artist_parse_tests));
            
            JArray items = json.Value<JArray>("items");
            
            // Test a full artist representation
            Artist fullArtist = Artist.FromJToken(items[0]) as Artist;
            Assert.IsNotNull(fullArtist, "Expected an artist object");
            Assert.IsNotNull(fullArtist.Country, "Expected a country");
            Assert.IsNotNull(fullArtist.Genres, "Expected genres");
            Assert.Greater(fullArtist.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(fullArtist.Id, "Expected an id");
            Assert.IsNotNull(fullArtist.Name, "Expected a name");
            Assert.IsNotNull(fullArtist.Thumb100Uri, "Expected a 100x100 thumb");
            Assert.IsNotNull(fullArtist.Thumb200Uri, "Expected a 200x200 thumb");
            Assert.IsNotNull(fullArtist.Thumb320Uri, "Expected a 320x320 thumb");

            // Test an unknown country representation
            JToken unknownCountryJson = items[1];
            Artist unknownCountryArtist = Artist.FromJToken(items[1]) as Artist;
            Assert.IsNotNull(unknownCountryArtist, "Expected an artist object");
            Assert.IsNull(unknownCountryArtist.Country, "Expected no country");
            Assert.IsNotNull(unknownCountryArtist.Genres, "Expected genres");
            Assert.Greater(unknownCountryArtist.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(unknownCountryArtist.Id, "Expected an id");
            Assert.IsNotNull(unknownCountryArtist.Name, "Expected a name");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAristNamePropertyIsRequiredForPlayMix()
        {
            Artist artist = new Artist();
            artist.PlayMix();
        }

        [Test]
        public void TestPlayMixGoesAheadWhenItCan()
        {
            Artist artist = new Artist() { Name = TestName };
            artist.PlayMix();
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAristNamePropertyIsRequiredForShow()
        {
            Artist artist = new Artist();
            artist.Show();
        }

        [Test]
        public void TestShowGoesAheadWhenItCan()
        {
            Artist artist = new Artist() { Id = TestId };
            artist.Show();
            Assert.Pass();
        }
    }
}
