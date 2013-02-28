// -----------------------------------------------------------------------
// <copyright file="SearchArtistByLocationTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Commands
{
    [TestFixture]
    public class SearchArtistByLocationTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistsAroundLocationThrowsExceptionForZeroLatitude()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistsAroundLocation((ListResponse<Artist> result) => { }, 0, -2.59239);
        }

        public void EnsureGetArtistsAroundLocationThrowsExceptionForZeroLongitude()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistsAroundLocation((ListResponse<Artist> result) => { }, 51.45534, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistsAroundLocationThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistsAroundLocation(null, 51.45534, -2.59239);
        }

        [Test]
        public void EnsureGetArtistsAroundLocationReturnsArtistsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistsAroundLocation(
                (ListResponse<Artist> result) =>
                {
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
                },
                51.45534,
                -2.59239);
        }

        /// <summary>
        /// The faked GetArtistsAroundLocation response Returns no results found
        /// </summary>
        [Test]
        public void EnsureGetArtistsAroundLocationReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_noresults));
            client.GetArtistsAroundLocation(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
                },
                51.45534,
                -2.59239);
        }

        [Test]
        public async void EnsureAsyncGetArtistsAroundLocationReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            ListResponse<Artist> result = await client.GetArtistsAroundLocation(51.45534, -2.59239);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
