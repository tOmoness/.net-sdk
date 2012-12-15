// -----------------------------------------------------------------------
// <copyright file="GenreTests.cs" company="Nokia">
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
    public class GenreTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetGenresThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.genres));
            client.GetGenres(null);
        }

        [Test]
        public void EnsureGetGenresReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.genres));
            client.GetGenres(
                (ListResponse<Genre> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

                    foreach (Genre genre in result.Result)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(genre.Id), "Expected Id to be populated");
                        Assert.IsFalse(string.IsNullOrEmpty(genre.Name), "Expected Name to be populated");
                    }
                });
        }

        [Test]
        public void EnsureGetGenresReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.InternalServerError()));
            client.GetGenres(
                (ListResponse<Genre> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                });
        }

        [Test]
        public async void EnsureAsyncGetGenresReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.genres));

            ListResponse<Genre> result = await client.GetGenres();
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
