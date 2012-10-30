// -----------------------------------------------------------------------
// <copyright file="ProductNewReleasesTests.cs" company="Nokia">
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
    public class ProductNewReleasesTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetNewReleasesThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetNewReleases((ListResponse<Product> result) => { }, Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetNewReleasesThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetNewReleases(null, Category.Album);
        }

        [Test]
        public void EnsureGetNewReleasesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new SuccessfulMockApiRequestHandler());
            client.GetNewReleases(
                (ListResponse<Product> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                },
                Category.Album);
        }

        [Test]
        public void EnsureGetNewReleasesReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new FailedMockApiRequestHandler());
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
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new SuccessfulMockApiRequestHandler());

            ListResponse<Product> result = await client.GetNewReleases(Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetNewReleases(Category.Album, 0, 10);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
