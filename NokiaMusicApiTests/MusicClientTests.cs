// -----------------------------------------------------------------------
// <copyright file="MusicClientTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class MusicClientTests
    {
        [Test]
        public void CheckApiCredentialsValidated()
        {
            string nullKey = null;
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClient(nullKey); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClient(nullKey, "gb"); }));
        }

        [Test]
        public void CheckCountryCodeValidated()
        {
            string nullCountry = null;
            Assert.DoesNotThrow(new TestDelegate(() => { new MusicClient("test"); }));
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", nullCountry); }));
        }

        [Test]
        public void EnsureDefaultRequestHandlerIsCreated()
        {
            MusicClient client = new MusicClient("test", "us");
            Assert.AreEqual(client.RequestHandler.GetType(), typeof(ApiRequestHandler), "Expected the default handler");
        }

        [Test]
        public void ValidateCountryCodeEmptyCheckedAndLowerCased()
        {
            // Check InvalidCountryCodeException thrown for empty string as well as null
            string nullCountry = null;
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", nullCountry); }));
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", string.Empty); }));

            // Check InvalidCountryCodeException thrown for invalid length
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClient("test", "GBR"); }));

            // Check InvalidCountryCodeException thrown for invalid length
            Assert.DoesNotThrow(new TestDelegate(() => { new MusicClient("test", "gb"); }));
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
        public void EnsureApiNotAvailableExceptionThrownWhenRegionInfoCtorUsedAndCountryIsInvalidForListMethods()
        {
            // First test the public constructor...
            MusicClient publicClient = new MusicClient("test");

            // Now test the handling of non-availability...

            // Our REST API will give a 404 response when the country code is not valid, so this test
            // ensures that gets translated into an ApiNotAvailableException when the RegionInfo constructor is used.
            MusicClient client = new MusicClient("test", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.Search(
                (ListResponse<MusicItem> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(ApiNotAvailableException), response.Error.GetType(), "Expected an ApiNotAvailableException");
                },
                "test");
        }

        [Test]
        public void EnsureApiNotAvailableExceptionThrownWhenRegionInfoCtorUsedAndCountryIsInvalidForItemMethods()
        {
            // First test the public constructor...
            MusicClient publicClient = new MusicClient("test");

            // Now test the handling of non-availability...

            // Our REST API will give a 404 response when the country code is not valid, so this test
            // ensures that gets translated into an ApiNotAvailableException when the RegionInfo constructor is used.
            MusicClient client = new MusicClient("test", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetProduct(
                (Response<Product> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(ApiNotAvailableException), response.Error.GetType(), "Expected an ApiNotAvailableException");
                },
                "test");
        }

        [Test]
        public void EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403ForListMethods()
        {
            MusicClient client = new MusicClient("badkey", "us", new MockApiRequestHandler(FakeResponse.Forbidden()));
            client.Search(
                (ListResponse<MusicItem> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(InvalidApiCredentialsException), response.Error.GetType(), "Expected an InvalidApiCredentialsException");
                },
                "test");
        }

        [Test]
        public void EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403ForItemMethods()
        {
            MusicClient client = new MusicClient("badkey", "us", new MockApiRequestHandler(FakeResponse.Forbidden()));
            client.GetProduct(
                (Response<Product> response) =>
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
