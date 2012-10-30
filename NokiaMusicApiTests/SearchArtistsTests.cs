// -----------------------------------------------------------------------
// <copyright file="SearchArtistsTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class SearchArtistsTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureSearchArtistsThrowsExceptionForNullSearchTerm()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.SearchArtists((ListResponse<Artist> result) => { }, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureSearchArtistsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.SearchArtists(null, @"lady gaga");
        }

        [Test]
        public void EnsureSearchArtistsReturnsArtistsForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.SearchArtists(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                },
                "muse");
        }

        /// <summary>
        /// The faked SearchArtists response Returns no results found
        /// </summary>
        [Test]
        public void EnsureSearchArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new FailedMockApiRequestHandler());
            client.SearchArtists(
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
                "muse");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopArtistsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtists(null);
        }

        [Test]
        public void EnsureGetTopArtistsReturnsArtists()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtists(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                });
        }

        /// <summary>
        /// The faked GetTopArtists response Returns no results found
        /// </summary>
        [Test]
        public void EnsureGetTopArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new FailedMockApiRequestHandler());
            client.GetTopArtists(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
                });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopArtistsForGenreThrowsExceptionForNullGenreId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtistsForGenre((ListResponse<Artist> result) => { }, nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopArtistsForGenreThrowsExceptionForNullGenre()
        {
            Genre nullGenre = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtistsForGenre((ListResponse<Artist> result) => { }, nullGenre);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopArtistsForGenreThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtistsForGenre(null, "rock");
        }

        [Test]
        public void EnsureGetTopArtistsForGenreReturnsArtists()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetTopArtistsForGenre(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                },
                new Genre() { Id = "rock" });
        }

        /// <summary>
        /// The faked GetTopArtistsForGenre response Returns an error condition rather than no results
        /// </summary>
        [Test]
        public void EnsureGetTopArtistsForGenreReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new FailedMockApiRequestHandler());
            client.GetTopArtistsForGenre(
                (ListResponse<Artist> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-200 response");
                    Assert.IsNull(result.Result, "Expected no results");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                "rock");
        }

        [Test]
        public async void EnsureAsyncSearchArtistsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            ListResponse<Artist> result = await client.SearchArtists("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async void EnsureAsyncGetTopArtistsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            ListResponse<Artist> result = await client.GetTopArtists();
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async void EnsureAsyncGetTopArtistsForGenreReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            ListResponse<Artist> result = await client.GetTopArtistsForGenre("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetTopArtistsForGenre(new Genre() { Id = "test" });
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
