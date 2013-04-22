// -----------------------------------------------------------------------
// <copyright file="ProductNewReleasesTests.cs" company="Nokia">
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
    public class ProductNewReleasesTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetNewReleasesThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleases((ListResponse<Product> result) => { }, Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleases(null, Category.Album);
        }

        [Test]
        public void EnsureGetNewReleasesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleases(this.ValidateNewReleasesResponse, Category.Album);
        }

        [Test]
        public void EnsureGetNewReleasesReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetNewReleases(
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
        public async void EnsureAsyncGetNewReleasesReturnsItems()
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
        public void EnsureGetNewReleasesForGenreThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetNewReleasesForGenre((ListResponse<Product> result) => { }, nullGenre, Category.Album);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreThrowsExceptionForNullGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            string nullGenreId = null;
            client.GetNewReleasesForGenre((ListResponse<Product> result) => { }, nullGenreId, Category.Album);
        }

        [Test]
        public void EnsureGetNewReleasesForGenreReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetNewReleasesForGenre(this.ValidateNewReleasesResponse, new Genre() { Id = "rock" }, Category.Album);
            client.GetNewReleasesForGenre(this.ValidateNewReleasesResponse, "rock", Category.Album);
        }

        [Test]
        public async void EnsureGetNewReleasesForGenreAsyncReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));

            ListResponse<Product> result = await client.GetNewReleasesForGenreAsync("rock", Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetNewReleasesForGenreAsync(new Genre() { Id = "rock" }, Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesForGenreAsyncThrowsExceptionForNullGenre()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Genre nullGenre = null;
            client.GetNewReleasesForGenreAsync(nullGenre, Category.Album);
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForAlbum()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new NewReleasesCommand
                            {
                                RequestHandler = new MockApiRequestHandler(FakeResponse.NotFound()),
                                Category = Category.Album,
                                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
                            };
            cmd.Invoke(response => { });
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
                            MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
                        };
            cmd.Invoke(response => { });
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
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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
                MusicClientSettings = new MockMusicClientSettings(string.Empty, string.Empty)
            };
            cmd.Invoke(response => { });
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