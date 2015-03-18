// -----------------------------------------------------------------------
// <copyright file="ProductChartTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class ProductChartTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task EnsureGetTopProductsThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            await client.GetTopProductsAsync(Category.Unknown);
        }

        [Test]
        public async Task EnsureGetTopProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            this.ValidateTopProductsResponse(await client.GetTopProductsAsync(Category.Album));
        }

        [Test]
        public async Task EnsureGetTopProductsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Product> result = await client.GetTopProductsAsync(Category.Album);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task EnsureGetNewReleasesForGenreThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            await client.GetNewReleasesForGenreAsync("rock", Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetTopProductsForGenreThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
#pragma warning disable 0618  // Disable this for tests
            await client.GetTopProductsForGenreAsync(nullGenre, Category.Album);
#pragma warning restore 0618
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetTopProductsForGenreThrowsExceptionForNullGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            string nullGenreId = null;
            await client.GetTopProductsForGenreAsync(nullGenreId, Category.Album);
        }

        [Test]
        public async Task EnsureGetTopProductsForGenreReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
#pragma warning disable 0618  // Disable this for tests
            this.ValidateTopProductsResponse(await client.GetTopProductsForGenreAsync(new Genre() { Id = "rock" }, Category.Album));
#pragma warning restore 0618
            this.ValidateTopProductsResponse(await client.GetTopProductsForGenreAsync("rock", Category.Album));
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new TopProductsCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Album,
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/charts/album/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForTrack()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new TopProductsCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Track,
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/charts/track/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForGenreAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new TopProductsCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Album,
                GenreId = "pop",
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/genres/pop/charts/album/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForGenreTrack()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new TopProductsCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Track,
                GenreId = "pop",
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/genres/pop/charts/track/", uri.ToString());
        }

        private void ValidateTopProductsResponse(ListResponse<Product> result)
        {
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (Product productItem in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(productItem.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(productItem.Name), "Expected Name to be populated");
                Assert.AreNotEqual(Category.Unknown, productItem.Category, "Expected Category to be set");
            }
        }
    }
}
