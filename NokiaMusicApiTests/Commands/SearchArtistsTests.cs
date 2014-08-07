// -----------------------------------------------------------------------
// <copyright file="SearchArtistsTests.cs" company="Nokia">
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
    public class SearchArtistsTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureSearchArtistsThrowsExceptionForNullSearchTerm()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            await client.SearchArtistsAsync(null);
        }

        [Test]
        public async Task EnsureSearchArtistsReturnsArtistsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            ListResponse<Artist> result = await client.SearchArtistsAsync("muse");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (Artist artist in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(artist.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(artist.Name), "Expected Name to be populated");
                Assert.IsNotNull(artist.Genres, "Expected a genre list");
                Assert.Greater(artist.Genres.Length, 0, "Expected more than 0 genres");
            }
        }

        /// <summary>
        /// The faked SearchArtists response Returns no results found
        /// </summary>
        /// <returns>An async Task</returns>
        [Test]
        public async Task EnsureSearchArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_noresults));
            ListResponse<Artist> result = await client.SearchArtistsAsync("muse");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
        }

        [Test]
        public async Task EnsureGetArtistWithValidIdReturnsOneArtist()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.get_artist));
            Response<Artist> result = await client.GetArtistAsync("559688");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a result");
            Assert.IsNull(result.Error, "Expected no error");
        }

        [Test]
        public async Task EnsureGetArtistWithInvalidIdReturnsNullResult()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_noresults));
            Response<Artist> result = await client.GetArtistAsync("559688");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNull(result.Result, "Expected no result");
            Assert.IsNull(result.Error, "Expected no error");
        }

        [Test]
        public async Task EnsureGetArtistWithInvalidCredsGivesError()
        {
            IMusicClient client = new MusicClient("badcreds", "gb", new MockApiRequestHandler(FakeResponse.Unauthorized()));
            Response<Artist> result = await client.GetArtistAsync("559688");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode.Value, "Expected a 401 response");
            Assert.IsNull(result.Result, "Expected no result");
            Assert.IsNotNull(result.Error, "Expected an error");
        }

        [Test]
        public async Task EnsureGetTopArtistsReturnsArtists()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.top_artists));
            ListResponse<Artist> result = await client.GetTopArtistsAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        /// <summary>
        /// The faked GetTopArtists response Returns no results found
        /// </summary>
        /// <returns>An async Task</returns>
        [Test]
        public async Task EnsureGetTopArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_noresults));
            ListResponse<Artist> result = await client.GetTopArtistsAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
        }

        [Test]
        public async Task EnsureRequestIdComesBackInResponse()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            var requestId = Guid.NewGuid();
            ListResponse<Artist> result = await client.SearchArtistsAsync("muse", requestId: requestId);

            Assert.IsNotNull(result, "Expected a result");
            Assert.AreEqual(requestId, result.RequestId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetTopArtistsForGenreThrowsExceptionForNullGenreId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.top_artists_genre));
            await client.GetTopArtistsForGenreAsync(nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetTopArtistsForGenreThrowsExceptionForNullGenre()
        {
            Genre nullGenre = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.top_artists_genre));
            await client.GetTopArtistsForGenreAsync(nullGenre);
        }

        [Test]
        public async Task EnsureGetTopArtistsForGenreReturnsArtists()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.top_artists_genre));
            ListResponse<Artist> result = await client.GetTopArtistsForGenreAsync(new Genre() { Id = "rock" });
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        /// <summary>
        /// The faked GetTopArtistsForGenre response Returns an error condition rather than no results
        /// </summary>
        /// <returns>An async Task</returns>
        [Test]
        public async Task EnsureGetTopArtistsForGenreReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Artist> result = await client.GetTopArtistsForGenreAsync("rock");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-200 response");
            Assert.IsNull(result.Result, "Expected no results");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }
    }
}
