// -----------------------------------------------------------------------
// <copyright file="SeedTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MixRadio.Tests.Types
{
    [TestFixture]
    public sealed class SeedTests
    {
        private const string ArtistId = "559688";
        private const string ArtistName = "Muse";
        private const string MixId = "1234567";

        [Test]
        public void TestArtistIdSeed()
        {
            Seed seed = Seed.FromArtistId(ArtistId);
            string origJson = seed.ToJson();
            string newJson = Seed.FromJson(origJson).ToJson();

            Assert.IsNotNull(origJson);
            Assert.AreEqual(origJson, newJson);
        }

        [Test]
        public void TestArtistNameSeed()
        {
            Seed seed = Seed.FromArtistName(ArtistName);
            string origJson = seed.ToJson();
            string newJson = Seed.FromJson(origJson).ToJson();

            Assert.IsNotNull(origJson);
            Assert.AreEqual(origJson, newJson);
        }

        [Test]
        public void TestMixIdSeed()
        {
            Seed seed = Seed.FromMixId(MixId);
            string origJson = seed.ToJson();
            string newJson = Seed.FromJson(origJson).ToJson();

            Assert.IsNotNull(origJson);
            Assert.AreEqual(origJson, newJson);
        }

        [Test]
        public void TestUserIdSeed()
        {
            string userId = Guid.NewGuid().ToString();

            Seed seed = Seed.FromUserId(userId);
            string origJson = seed.ToJson();
            string newJson = Seed.FromJson(origJson).ToJson();

            Assert.IsNotNull(origJson);
            Assert.AreEqual(origJson, newJson);
        }

        [Test]
        public void TestEqualityAndHashCodes()
        {
            Seed seedMixId = Seed.FromMixId(MixId);
            Seed seedArtistId = Seed.FromArtistId(ArtistId);
            Seed seedArtistName = Seed.FromArtistName(ArtistName);
            Seed seedRehydratedArtistName = Seed.FromJson(seedArtistName.ToJson());
            Seed nullSeed1 = null;
            Seed nullSeed2 = null;

            Assert.IsTrue(nullSeed1 == nullSeed2, "Expected equality");
            Assert.IsTrue(seedArtistName == seedRehydratedArtistName, "Expected equality");
            Assert.IsTrue(seedArtistId != seedArtistName, "Expected inequality");

            Assert.IsFalse(seedArtistName.Equals(null), "Expected inequality");
            Assert.IsTrue(seedArtistName.Equals(seedArtistName), "Expected equality");
            Assert.IsTrue(seedArtistName.Equals(seedRehydratedArtistName), "Expected equality");

            Assert.AreNotEqual(seedMixId.GetHashCode(), 0, "Expected a hashcode");
            Assert.AreNotEqual(seedArtistName.GetHashCode(), 0, "Expected a hashcode");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadTypeJsonHandling()
        {
            Seed.FromJson("{\"id\":\"123456\",\"type\":\"naughty\"}");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadJsonHandling()
        {
            Seed.FromJson("{\"name\":\"123456\"}");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadUserJsonHandling()
        {
            Seed.FromJson("{\"badid\":\"123456\",\"type\":\"user\"}");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadArtistJsonHandling()
        {
            Seed.FromJson("{\"badid\":\"123456\",\"type\":\"musicartist\"}");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestUnknownTypeHandling()
        {
            new Seed().ToJson();
        }

        [Test]
        public void TestSeedCollectionInitialisation()
        {
            var nullSeeds = new SeedCollection();
            Assert.AreEqual(0, nullSeeds.Count, "Expected no seeds");
        }

        [Test]
        public void TestSeedCollectionEqualityAndHashCodes()
        {
            var seeds1 = new SeedCollection(Seed.FromMixId(MixId));
            var seeds2 = new SeedCollection(Seed.FromMixId(MixId));
            var seeds3 = new SeedCollection(Seed.FromArtistId(ArtistId));
            SeedCollection nullSeeds1 = null;
            SeedCollection nullSeeds2 = null;

            Assert.AreEqual(seeds1, seeds2, "Expected equality");

            Assert.IsTrue(nullSeeds1 == nullSeeds2, "Expected equality");
            Assert.IsTrue(seeds1 == seeds2, "Expected equality");
            Assert.IsTrue(seeds1 != seeds3, "Expected inequality");

            Assert.IsFalse(seeds1.Equals(null), "Expected inequality");
            Assert.IsTrue(seeds1.Equals(seeds1), "Expected equality");
            Assert.IsTrue(seeds1.Equals(seeds2), "Expected equality");

            Assert.AreNotEqual(seeds1.GetHashCode(), 0, "Expected a hashcode");

            Assert.IsNotNull(seeds1.GetEnumerator(), "Expected Enumerator");
            Assert.IsNotNull((seeds1 as IEnumerable).GetEnumerator(), "Expected Enumerator");
        }

        [Test]
        public void TestSeedCollectionParsing()
        {
            var seeds = SeedCollection.FromJson(System.Text.Encoding.UTF8.GetString(Resources.userevent));
            Assert.IsNotNull(seeds, "Expected seeds");
            Assert.AreEqual(1, seeds.Count, "Expected seeds");
            Assert.AreEqual(SeedType.MixId, seeds.FirstOrDefault().Type, "Expected mix seed");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadMixJsonHandling()
        {
            SeedCollection.FromJson("{\"mix\":{\"badid\":\"123456\"}}");
        }
    }
}