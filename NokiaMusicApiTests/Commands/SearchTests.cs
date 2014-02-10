// -----------------------------------------------------------------------
// <copyright file="SearchTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class SearchTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureSearchThrowsExceptionForNullSearchTermAndGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_all));
            client.SearchAsync().Wait();
        }

        [Test]
        public async Task EnsureSearchReturnsItemsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_all));
            ListResponse<MusicItem> result = await client.SearchAsync("lady gaga");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (MusicItem item in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(item.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(item.Name), "Expected Name to be populated");
                Assert.IsNotNull(item.Thumb100Uri, "Expected a thumbnail uri");
            }
        }

        [Test]
        public async Task EnsureSearchBpmReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            ListResponse<MusicItem> result = await client.SearchBpmAsync(120, 130);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async Task EnsureSearchWithMultipleCategoriesReturnsItemsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_album_and_single));
            ListResponse<MusicItem> result = await client.SearchAsync("lady gaga", Category.Album | Category.Single);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (MusicItem item in result.Result)
            {
                Assert.IsInstanceOf<Product>(item);
                Assert.IsFalse(string.IsNullOrEmpty(item.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(item.Name), "Expected Name to be populated");
                Assert.IsNotNull(item.Thumb100Uri, "Expected a thumbnail uri");
            }
        }

        [Test]
        public async Task EnsureSearchForRadioStationReturnsItemsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_radiostation));
            ListResponse<MusicItem> result = await client.SearchAsync("lady gaga", Category.RadioStation);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (MusicItem item in result.Result)
            {
                Assert.IsInstanceOf<Mix>(item);
                Assert.IsFalse(string.IsNullOrEmpty(item.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(item.Name), "Expected Name to be populated");
                Assert.IsNotNull(item.Thumb100Uri, "Expected a thumbnail uri");
            }
        }

        [Test]
        public async Task EnsureSearchForGenreTracksReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_all));
            ListResponse<MusicItem> result = await client.SearchAsync(genreId: "Rock", category: Category.Track, orderBy: OrderBy.ReleaseDate, sortOrder: SortOrder.Ascend);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (MusicItem item in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(item.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(item.Name), "Expected Name to be populated");
                Assert.IsNotNull(item.Thumb100Uri, "Expected a thumbnail uri");
            }
        }

        [Test]
        public async Task EnsureSearchReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<MusicItem> result = await client.SearchAsync("green day");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }
    }
}
