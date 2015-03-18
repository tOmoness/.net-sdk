// -----------------------------------------------------------------------
// <copyright file="MixTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
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
        public void TestLinkingProperties()
        {
            var item = new Mix() { Id = TestId };
            var itemWithNullId = new Mix();

            Assert.IsNotNull(item.AppToAppUri, "Expected App to App URI to be calculated");
            Assert.IsNull(itemWithNullId.AppToAppUri, "Expected App to App URI not to be calculated");

            Assert.IsNotNull(item.WebUri, "Expected Web URI to be calculated");
            Assert.IsNull(itemWithNullId.WebUri, "Expected Web URI not to be calculated");
        }

        [Test]
        public void TestOverrides()
        {
            Mix mix = new Mix() { Id = TestId, Name = TestName };
            Assert.IsNotNull(mix.GetHashCode(), "Expected a hash code");
            Assert.IsFalse(mix.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void HashCodeCanBeRetrievedWhenIdIsNull()
        {
            Mix mix = new Mix();
            Assert.IsNotNull(mix.GetHashCode(), "Expected a hash code");
        }

        [Test]
        public void TestJsonParsing()
        {
            Assert.IsNull(Mix.FromJToken(null, null), "Expected a null return");

            Mix mix = new Mix() { Id = "1234", Name = "Metal", ParentalAdvisory = true };
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/200x200/1182.jpg\" } }");

            Mix fromJson = Mix.FromJToken(json, null) as Mix;

            Assert.IsNotNull(fromJson, "Expected a Mix object");

            Assert.IsTrue(mix.Equals(fromJson), "Expected the same Mix");
        }

        [Test]
        public void InvalidImageUriIsHandledSuccessfully()
        {
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http:////\" } }");
            Mix mixFromJson = Mix.FromJToken(json, null);

            Assert.IsNotNull(mixFromJson, "Expected a Mix object");

            Assert.AreEqual("http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg", mixFromJson.Thumb100Uri.AbsoluteUri, "Expected the thumb100 to be parsed");
            Assert.IsNull(mixFromJson.Thumb200Uri, "Expected the thumb200 invalid url to be handled correctly");
        }

        [Test]
        public void ExtraImageUriIsHandledSuccessfully()
        {
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http:////\", \"640x640\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\" } }");
            Mix mixFromJson = Mix.FromJToken(json, null);

            Assert.IsNotNull(mixFromJson, "Expected a Mix object");

            Assert.AreEqual("http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg", mixFromJson.Thumb640Uri.AbsoluteUri, "Expected the thumb100 to be parsed");
            Assert.IsNull(mixFromJson.Thumb200Uri, "Expected the thumb200 invalid url to be handled correctly");
        }

        [Test]
        public void DescriptionHandledSuccessfully()
        {
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"description\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http:////\", \"640x640\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\" } }");
            Mix mixFromJson = Mix.FromJToken(json, null);

            Assert.IsNotNull(mixFromJson, "Expected a Mix object");

            Assert.AreEqual("Metal", mixFromJson.Description, "Expected the description to be parsed");
            Assert.IsNull(mixFromJson.Thumb200Uri, "Expected the thumb200 invalid url to be handled correctly");
        }

        [Test]
        public void FeaturedArtistsHandledSuccessfully()
        {
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\",\"description\":\"Metal\",\"parentaladvisory\":true, \"thumbnails\": { \"100x100\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\", \"200x200\": \"http:////\", \"640x640\": \"http://download.ch1.vcdn.nokia.com/p/d/music_image/100x100/1182.jpg\" },\"featuredartists\":[{\"name\":\"Mark Ronson feat. Bruno Mars\",\"id\":\"72969237\"},{\"name\":\"Mark Ronson\",\"id\":\"299194\"},{\"name\":\"Bruno Mars\",\"id\":\"626716\"},{\"name\":\"Nicki Minaj\",\"id\":\"726059\"},{\"name\":\"Florence + The Machine\",\"id\":\"709885\"}] }");
            Mix mixFromJson = Mix.FromJToken(json, null);

            Assert.IsNotNull(mixFromJson, "Expected a Mix object");

            Assert.IsNotNull(mixFromJson.FeaturedArtists, "Expected FeaturedArtists list to not be null");
            Assert.AreEqual(5, mixFromJson.FeaturedArtists.Count, "Expected FeaturedArtists list to contain the expected number of artists");

            Assert.AreEqual("72969237", mixFromJson.FeaturedArtists[0].Id);
            Assert.AreEqual("Mark Ronson feat. Bruno Mars", mixFromJson.FeaturedArtists[0].Name);
        }

        [Test]
#pragma warning disable 1998  // Disable async warnings for test code
        public async Task TestSeedCollectionScenarios()
        {
            var settings = new Nokia.Music.Tests.Internal.MockMusicClientSettings("clientid", "gb", "en");

            JObject recentMixes = JObject.Parse(Encoding.UTF8.GetString(Resources.user_recent_mixes));

            foreach (JToken mixJson in recentMixes.Value<JArray>("items"))
            {
                var mix = Mix.FromJToken(mixJson, settings);
#if !PORTABLE
                try
                {
                    await mix.Play();
                }
                catch
                {
                    // on non-WP - including NUnit, PlayMeTask is not supported yet so we ignore the exception
                }
#endif
            }
        }
#pragma warning restore 1998
    }
}
