// -----------------------------------------------------------------------
// <copyright file="ArtistProductTests.cs" company="Nokia">
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
    public class ArtistProductTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistProductsThrowsExceptionForNullArtistId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProducts((ListResponse<Product> result) => { }, nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistProductsThrowsExceptionForNullArtist()
        {
            Artist nullArtist = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProducts((ListResponse<Product> result) => { }, nullArtist);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistProductsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProducts(null, "test");
        }

        [Test]
        public void EnsureGetArtistProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProducts(this.ProductResponse, new Artist() { Id = "test" }, Category.Album);
            client.GetArtistProducts(this.ProductResponse, "test");
        }

        [Test]
        public void EnsureGetArtistProductsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetArtistProducts(
                (ListResponse<Product> result) =>
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
        public async void EnsureAsyncGetArtistProductsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.artist_products));
            this.ProductResponse(await client.GetArtistProducts("test"));
            this.ProductResponse(await client.GetArtistProducts(new Artist() { Id = "test" }));
        }

        private void ProductResponse(ListResponse<Product> result)
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
