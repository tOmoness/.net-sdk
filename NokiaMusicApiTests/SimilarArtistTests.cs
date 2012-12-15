// -----------------------------------------------------------------------
// <copyright file="SimilarArtistTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class SimilarArtistTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsThrowsExceptionForNullArtistId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtists((ListResponse<Artist> result) => { }, nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsThrowsExceptionForNullArtist()
        {
            Artist nullArtist = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtists((ListResponse<Artist> result) => { }, nullArtist);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtists(null, "test");
        }

        [Test]
        public void EnsureGetSimilarArtistsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtists(
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
                new Artist() { Id = "test" });
        }

        [Test]
        public void EnsureGetSimilarArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetSimilarArtists(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                "test");
        }

        [Test]
        public async void EnsureAsyncGetSimilarArtistsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            ListResponse<Artist> result = await client.GetSimilarArtists("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetSimilarArtists(new Artist() { Id = "test" });
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
