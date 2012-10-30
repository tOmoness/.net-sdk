// -----------------------------------------------------------------------
// <copyright file="MixTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// Mix tests
    /// </summary>
    [TestFixture]
    public class MixTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            Mix mix = new Mix() { Id = TestId, Name = TestName, ParentalAdvisory = true, Thumb100Uri = new Uri("http://assets.ent.nokia.com/p/d/music_image/100x100/1182.jpg"), Thumb200Uri = new Uri("http://assets.ent.nokia.com/p/d/music_image/200x200/1182.jpg") };

            Assert.AreEqual(TestId, mix.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, mix.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Mix mix = new Mix() { Id = TestId, Name = TestName };
            Assert.IsNotNull(mix.GetHashCode(), "Expected a hash code");
            Assert.IsFalse(mix.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void TestJsonParsing()
        {
            Mix mix = new Mix() { Id = "1234", Name = "Metal", ParentalAdvisory = true };
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/200x200/1182.jpg\" } }");

            Mix fromJson = Mix.FromJToken(json) as Mix;

            Assert.IsNotNull(fromJson, "Expected a Mix object");

            Assert.IsTrue(mix.Equals(fromJson), "Expected the same Mix");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMixIdPropertyIsRequiredForPlay()
        {
            Mix mix = new Mix();
            mix.Play();
        }

        [Test]
        public void TestPlayMixGoesAheadWhenItCan()
        {
            Mix mix = new Mix() { Id = "1234" };
            mix.Play();
            Assert.Pass();
        }
    }
}
