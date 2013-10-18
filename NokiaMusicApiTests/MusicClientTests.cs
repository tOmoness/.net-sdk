// -----------------------------------------------------------------------
// <copyright file="MusicClientTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Compression;
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
        public void ValidateLanguageSetIfSpecified()
        {
            MusicClient client = new MusicClient("test", "us", "en");
            Assert.AreEqual(client.Language, "en");
        }

        [Test]
        public void EnsureCreateCatalogItemBasedOnCategoryReturnsNullForUnknownCategory()
        {
            // Ensure null gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(null), "Expected a null response");

            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.category_parse_tests));
            JArray items = json.Value<JArray>(MusicClientCommand.ArrayNameItems);

            // Ensure JSON without a category gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(items[2]), "Expected a null response");

            // Ensure JSON with a category we don't handle gives a null...
            Assert.IsNull(SearchCommand.CreateCatalogItemBasedOnCategory(items[3]), "Expected a null response");
        }

        [Test]
        public void EnsureCreateCatalogItemBasedOnCategoryReturnsCatalogItemForKnownCategory()
        {
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.category_parse_tests));
            JArray items = json.Value<JArray>(MusicClientCommand.ArrayNameItems);

            Assert.AreEqual(typeof(Artist), SearchCommand.CreateCatalogItemBasedOnCategory(items[0]).GetType(), "Expected an Artist");
            Assert.AreEqual(typeof(Product), SearchCommand.CreateCatalogItemBasedOnCategory(items[1]).GetType(), "Expected a Product");
        }

        [Test]
        public async Task EnsureApiNotAvailableExceptionThrownWhenRegionInfoCtorUsedAndCountryIsInvalidForListMethods()
        {
            // First test the public constructor...
            MusicClient publicClient = new MusicClient("test");

            // Now test the handling of non-availability...

            // Our REST API will give a 404 response when the country code is not valid, so this test
            // ensures that gets translated into an ApiNotAvailableException when the RegionInfo constructor is used.
            MusicClient client = new MusicClient("test", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<MusicItem> response = await client.SearchAsync("test");
            Assert.IsNotNull(response.Error, "Expected an Error");
            Assert.AreEqual(typeof(ApiNotAvailableException), response.Error.GetType(), "Expected an ApiNotAvailableException");
        }

        [Test]
        public async Task EnsureApiNotAvailableExceptionThrownWhenRegionInfoCtorUsedAndCountryIsInvalidForItemMethods()
        {
            // First test the public constructor...
            MusicClient publicClient = new MusicClient("test");

            // Now test the handling of non-availability...

            // Our REST API will give a 404 response when the country code is not valid, so this test
            // ensures that gets translated into an ApiNotAvailableException when the RegionInfo constructor is used.
            MusicClient client = new MusicClient("test", new MockApiRequestHandler(FakeResponse.NotFound()));
            Response<Product> response = await client.GetProductAsync("test");
            Assert.IsNotNull(response.Error, "Expected an Error");
            Assert.AreEqual(typeof(ApiNotAvailableException), response.Error.GetType(), "Expected an ApiNotAvailableException");
        }

        [Test]
        public async Task EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403ForListMethods()
        {
            MusicClient client = new MusicClient("badkey", "us", new MockApiRequestHandler(FakeResponse.Forbidden()));
            ListResponse<MusicItem> response = await client.SearchAsync("test");
            Assert.IsNotNull(response.Error, "Expected an Error");
            Assert.AreEqual(typeof(InvalidApiCredentialsException), response.Error.GetType(), "Expected an InvalidApiCredentialsException");
        }

        [Test]
        public async Task EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403ForItemMethods()
        {
            MusicClient client = new MusicClient("badkey", "us", new MockApiRequestHandler(FakeResponse.Forbidden()));
            Response<Product> response = await client.GetProductAsync("test");
            Assert.IsNotNull(response.Error, "Expected an Error");
            Assert.AreEqual(typeof(InvalidApiCredentialsException), response.Error.GetType(), "Expected an InvalidApiCredentialsException");
        }

        [Test]
        public void ServerTimeGivesCurrentTimeIfNotUpdatedFromServer()
        {
            this.RunServerTimeOffsetTest(null, 0);
        }

        [Test]
        public void ServerTimeGivesCurrentTimeIfBadHeadersReceived()
        {
            this.RunServerTimeOffsetTest(new WebHeaderCollection { { "NotADate", "asdasdasdasd" } }, 0);
        }

        [Test]
        public void ServerTimeIsNullIfNotParsable()
        {
            this.RunServerTimeOffsetTest(new WebHeaderCollection { { "Date", "NotADate" } }, 0);
        }

        [Test]
        public void ServerTimeAdjustmentGivesCurrentTimeWithNoAgeOffset()
        {
            this.RunServerTimeOffsetTest(new WebHeaderCollection { { "Date", DateTime.UtcNow.ToString("R") } }, 0);
        }

        [Test]
        public void ServerTimeAdjustmentGivesCurrentTimeWithAgeOffset()
        {
            this.RunServerTimeOffsetTest(new WebHeaderCollection { { "Date", DateTime.UtcNow.ToString("R") }, { "Age", "2000" } }, 2000);
        }

        [Test]
        public void ServerTimeAdjustmentInvalidAge()
        {
            this.RunServerTimeOffsetTest(new WebHeaderCollection { { "Date", DateTime.UtcNow.ToString("R") }, { "Age", "kkkk" } }, 0);
        }

        private void RunServerTimeOffsetTest(WebHeaderCollection headers, int offset)
        {
            MusicClient client = new MusicClient("key", "us", new ApiRequestHandler(new ApiUriBuilder(), new GzipHandlerWp()));

            DateTime time = DateTime.UtcNow;
            DateTime offsetTime = time.AddSeconds(offset);

            if (headers != null)
            {
                (client.RequestHandler as ApiRequestHandler).DeriveServerTimeOffset(headers);
            }

            var oneSecBeforeStart = offsetTime.AddSeconds(-1);
            var clockTime = client.ServerTimeUtc;
            var oneSecAfterEnd = offsetTime.AddSeconds(1);

            // Assert
            Assert.GreaterOrEqual(clockTime, oneSecBeforeStart);
            Assert.GreaterOrEqual(oneSecAfterEnd, clockTime);
        }
    }
}
