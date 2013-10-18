// -----------------------------------------------------------------------
// <copyright file="ProductNewReleasesTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class ProductNewReleasesTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetNewReleasesThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleasesAsync(Category.Unknown).Wait();
        }

        [Test]
        public async Task EnsureGetNewReleasesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            this.ValidateNewReleasesResponse(await client.GetNewReleasesAsync(Category.Album));
        }

        [Test]
        public async Task EnsureGetNewReleasesReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Product> result = await client.GetNewReleasesAsync(Category.Album);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        public async Task EnsureAsyncGetNewReleasesReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));

            ListResponse<Product> result = await client.GetNewReleasesAsync(Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetNewReleasesAsync(Category.Album, 0, 10);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleasesForGenreAsync("rock", Category.Unknown).Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetNewReleasesForGenreAsync(nullGenre, Category.Album).Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForNullGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            string nullGenreId = null;
            client.GetNewReleasesForGenreAsync(nullGenreId, Category.Album).Wait();
        }

        [Test]
        public async Task EnsureGetNewReleasesForGenreReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            this.ValidateNewReleasesResponse(await client.GetNewReleasesForGenreAsync(new Genre() { Id = "rock" }, Category.Album));
            this.ValidateNewReleasesResponse(await client.GetNewReleasesForGenreAsync("rock", Category.Album));
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreAsyncThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetNewReleasesForGenreAsync(nullGenre, Category.Album).Wait();
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new NewReleasesCommand
                            {
                                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                                Category = Category.Album,
                                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
                            };
            cmd.AppendUriPath(uri);

            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/new/album/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForTrack()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new NewReleasesCommand
                        {
                            RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                            Category = Category.Track,
                            ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
                        };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/new/track/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForGenresAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new NewReleasesCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Album,
                GenreId = "rock",
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/genres/rock/new/album/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForGenresTrack()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new NewReleasesCommand
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                Category = Category.Track,
                GenreId = "rock",
                ClientSettings = new MockMusicClientSettings(string.Empty, string.Empty, string.Empty)
            };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/genres/rock/new/track/", uri.ToString());
        }

        private void ValidateNewReleasesResponse(ListResponse<Product> result)
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