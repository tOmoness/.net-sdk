// -----------------------------------------------------------------------
// <copyright file="ProductChartTests.cs" company="Nokia">
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
    public class ProductChartTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EnsureGetTopProductsThrowsExceptionForUnsupportedCategory()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts((ListResponse<Product> result) => { }, Category.Unknown);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTopProductsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts(null, Category.Album);
        }

        [Test]
        public void EnsureGetTopProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetTopProducts(
                (ListResponse<Product> result) =>
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
                },
                Category.Album);
        }

        [Test]
        public void EnsureGetTopProductsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetTopProducts(
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
        public async void EnsureAsyncGetTopProductsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            ListResponse<Product> result = await client.GetTopProducts(Category.Album);
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
