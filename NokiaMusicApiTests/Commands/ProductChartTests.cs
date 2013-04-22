// -----------------------------------------------------------------------
// <copyright file="ProductChartTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
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
        public void EnsureGetTopProductsThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts((ListResponse<Product> result) => { }, Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopProductsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts(null, Category.Album);
        }

        [Test]
        public void EnsureGetTopProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts(this.ValidateTopProductsResponse, Category.Album);
        }

        [Test]
        public void EnsureGetTopProductsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetTopProducts(
                (ListResponse<Product> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                Category.Album);
        }

        [Test]
        public async void EnsureAsyncGetTopProductsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            ListResponse<Product> result = await client.GetTopProductsAsync(Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleasesForGenre((ListResponse<Product> result) => { }, "rock", Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleasesForGenre(null, "pop", Category.Album);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopProductsForGenreThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetTopProductsForGenre((ListResponse<Product> result) => { }, nullGenre, Category.Album);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopProductsForGenreThrowsExceptionForNullGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            string nullGenreId = null;
            client.GetTopProductsForGenre((ListResponse<Product> result) => { }, nullGenreId, Category.Album);
        }

        [Test]
        public void EnsureGetTopProductsForGenreReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProductsForGenre(this.ValidateTopProductsResponse, new Genre() { Id = "rock" }, Category.Album);
            client.GetTopProductsForGenre(this.ValidateTopProductsResponse, "rock", Category.Album);
        }

        [Test]
        public async void EnsureGetTopProductsForGenreAsyncReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            ListResponse<Product> result = await client.GetTopProductsForGenreAsync("rock", Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetTopProductsForGenreAsync(new Genre() { Id = "rock" }, Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopProductsForGenreAsyncThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetTopProductsForGenreAsync(nullGenre, Category.Album);
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new TopProductsCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Album,
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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
