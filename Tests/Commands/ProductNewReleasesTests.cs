// -----------------------------------------------------------------------
// <copyright file="ProductNewReleasesTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Internal.Request;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class ProductNewReleasesTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task EnsureGetNewReleasesThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            await client.GetNewReleasesAsync(Category.Unknown);
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
        public async Task EnsureGetNewReleasesForGenreThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            await client.GetNewReleasesForGenreAsync("rock", Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetNewReleasesForGenreThrowsExceptionForNullGenreId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            string nullGenreId = null;
            await client.GetNewReleasesForGenreAsync(nullGenreId, Category.Album);
        }

        [Test]
        public async Task EnsureGetNewReleasesForGenreReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            this.ValidateNewReleasesResponse(await client.GetNewReleasesForGenreAsync("rock", Category.Album));
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