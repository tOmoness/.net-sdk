// -----------------------------------------------------------------------
// <copyright file="ArtistTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
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
        public void TestLinkingProperties()
        {
            var itemWithId = new Artist() { Id = TestId };
            var itemWithName = new Artist() { Name = TestName };
            var itemWithNullProperties = new Artist();

            Assert.IsNotNull(itemWithId.AppToAppUri, "Expected App to App URI to be calculated");
            Assert.IsNull(itemWithNullProperties.AppToAppUri, "Expected App to App URI not to be calculated");

            Assert.IsNotNull(itemWithName.AppToAppPlayUri, "Expected App to App Play URI to be calculated");
            Assert.IsNull(itemWithNullProperties.AppToAppPlayUri, "Expected App to App Play URI not to be calculated");

            Assert.IsNotNull(itemWithId.WebUri, "Expected Web URI to be calculated");
            Assert.IsNull(itemWithNullProperties.WebUri, "Expected Web URI not to be calculated");

            Assert.IsNotNull(itemWithName.WebPlayUri, "Expected Web Play URI to be calculated");
            Assert.IsNull(itemWithNullProperties.WebPlayUri, "Expected Web Play URI not to be calculated");
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
        public void HashCodeCanBeRetrievedWhenIdIsNull()
        {
            Artist artist = new Artist();
            Assert.IsNotNull(artist.GetHashCode(), "Expected a hash code");
        }

        [Test]
        public void TestJsonParsing()
        {
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.artist_parse_tests));

            JArray items = json.Value<JArray>(MusicClientCommand.ArrayNameItems);
            
            // Test a full artist representation
            Artist fullArtist = Artist.FromJToken(items[0], null) as Artist;
            Assert.IsNotNull(fullArtist, "Expected an artist object");
            Assert.IsNotNull(fullArtist.Country, "Expected a country");
            Assert.IsNotNull(fullArtist.Genres, "Expected genres");
            Assert.IsNotNull(fullArtist.Origin, "Expected an origin location");
            Assert.IsNotNull(fullArtist.Origin.Name, "Expected an origin name");
            Assert.Greater(fullArtist.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(fullArtist.Id, "Expected an id");
            Assert.IsNotNull(fullArtist.Name, "Expected a name");
            Assert.IsNotNull(fullArtist.MusicBrainzId, "Expected a MusicBrainz id");
            Assert.IsNotNull(fullArtist.Thumb50Uri, "Expected a 50x50 thumb");
            Assert.IsNotNull(fullArtist.Thumb100Uri, "Expected a 100x100 thumb");
            Assert.IsNotNull(fullArtist.Thumb200Uri, "Expected a 200x200 thumb");
            Assert.IsNotNull(fullArtist.Thumb320Uri, "Expected a 320x320 thumb");
            Assert.IsNotNull(fullArtist.Thumb640Uri, "Expected a 640x640 thumb");

            // Test an unknown country representation
            JToken unknownCountryJson = items[1];
            Artist unknownCountryArtist = Artist.FromJToken(items[1], null) as Artist;
            Assert.IsNotNull(unknownCountryArtist, "Expected an artist object");
            Assert.IsNull(unknownCountryArtist.Country, "Expected no country");
            Assert.IsNotNull(unknownCountryArtist.Genres, "Expected genres");
            Assert.Greater(unknownCountryArtist.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(unknownCountryArtist.Id, "Expected an id");
            Assert.IsNotNull(unknownCountryArtist.Name, "Expected a name");
        }
    }
}
