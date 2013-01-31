// -----------------------------------------------------------------------
// <copyright file="MusicClientTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class MusicClientTests
    {
        [Test]
        public void CheckApiCredentialsValidated()
        {
            string nullKey = null;
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClient(nullKey, nullKey, "gb"); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClient("test-app-id", nullKey, "gb"); }));
        }

        [Test]
        public void CheckCountryCodeValidated()
        {
            string nullCountry = null;
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient(@"test", @"test", nullCountry); }));
        }

        [Test]
        public void EnsureDefaultRequestHandlerIsCreated()
        {
            MusicClient client = new MusicClient("test", "test", "us");
            Assert.AreEqual(client.RequestHandler.GetType(), typeof(ApiRequestHandler), "Expected the default handler");
        }

        [Test]
        public void ValidateCountryCodeEmptyCheckedAndLowerCased()
        {
            // Check InvalidCountryCodeException thrown for empty string as well as null
            string nullCountry = null;
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", "test", nullCountry); }));
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", "test", string.Empty); }));

            // Check InvalidCountryCodeException thrown for invalid length
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", "test", "GBR"); }));
        }

        [Test]
        public void EnsureCreateCatalogItemBasedOnCategoryReturnsNullForUnknownCategory()
        {
            // Ensure null gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(null), "Expected a null response");

            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.category_parse_tests));
            JArray items = json.Value<JArray>("items");

            // Ensure JSON without a category gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(items[2]), "Expected a null response");

            // Ensure JSON with a category we don't handle gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(items[3]), "Expected a null response");
        }

        [Test]
        public void EnsureCreateCatalogItemBasedOnCategoryReturnsCatalogItemForKnownCategory()
        {
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.category_parse_tests));
            JArray items = json.Value<JArray>("items");

            Assert.AreEqual(typeof(Artist), SearchCommand.CreateCatalogItemBasedOnCategory(items[0]).GetType(), "Expected an Artist");
            Assert.AreEqual(typeof(Product), SearchCommand.CreateCatalogItemBasedOnCategory(items[1]).GetType(), "Expected a Product");
        }

        [Test]
        public void EnsureApiNotAvailableExceptionThrownWhenRegionInfoCtorUsedAndCountryIsInvalid()
        {
            // First test the public constructor...
            MusicClient publicClient = new MusicClient("test", "test");

            // Now test the handling of non-availability...

            // Our REST API will give a 404 response when the country code is not valid, so this test
            // ensures that gets translated into an ApiNotAvailableException when the RegionInfo constructor is used.
            MusicClient client = new MusicClient("test", "test", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.Search(
                (ListResponse<MusicItem> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(ApiNotAvailableException), response.Error.GetType(), "Expected an ApiNotAvailableException");
                },
                "test");
        }

        [Test]
        public void EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403()
        {
            MusicClient client = new MusicClient("badkey", "test", "us", new MockApiRequestHandler(FakeResponse.Forbidden()));
            client.Search(
                (ListResponse<MusicItem> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(InvalidApiCredentialsException), response.Error.GetType(), "Expected an InvalidApiCredentialsException");
                },
                "test");
        }

        [Test]
        public void ValidateDefaultSettings()
        {
            Assert.AreEqual(60000, MusicClient.RequestTimeout);
            Assert.IsTrue(MusicClient.GzipEnabled);
        }
    }
}
