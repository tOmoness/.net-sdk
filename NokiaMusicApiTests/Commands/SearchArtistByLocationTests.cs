// -----------------------------------------------------------------------
// <copyright file="SearchArtistByLocationTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class SearchArtistByLocationTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureThatADefaultZeroValueIsTreatedAsNull()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            await client.GetArtistsAroundLocationAsync(0, 0);
        }

        [Test]
        public async Task EnsureGetArtistsAroundLocationAcceptsZeroLatitude()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            await client.GetArtistsAroundLocationAsync(0, -2.59239);
        }

        [Test]
        public async Task EnsureGetArtistsAroundLocationAcceptsZeroLongitude()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            await client.GetArtistsAroundLocationAsync(51.45534, 0);
        }

        [Test]
        public async Task EnsureGetArtistsAroundLocationReturnsArtistsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            ListResponse<Artist> result = await client.GetArtistsAroundLocationAsync(51.45534, -2.59239);
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
        /// The faked GetArtistsAroundLocation response Returns no results found
        /// </summary>
        /// <returns>An async Task</returns>
        [Test]
        public async Task EnsureGetArtistsAroundLocationReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_noresults));
            ListResponse<Artist> result = await client.GetArtistsAroundLocationAsync(51.45534, -2.59239);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
        }
    }
}
