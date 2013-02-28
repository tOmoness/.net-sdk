// -----------------------------------------------------------------------
// <copyright file="ArtistProductTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Tests.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Commands
{
    [TestFixture]
    public class ArtistProductTests : ProductTestBase
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
            client.GetArtistProducts(this.ValidateProductResponse, new Artist() { Id = "test" }, Category.Album);
            client.GetArtistProducts(this.ValidateProductResponse, "test");
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
        public void EnsureAsyncGetArtistProductsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.artist_products));
            var artistProductByIdTask = client.GetArtistProducts("test");
            var artistProductByArtistTask = client.GetArtistProducts(new Artist() { Id = "test" });
            artistProductByIdTask.Wait();
            artistProductByArtistTask.Wait();
            this.ValidateProductResponse(artistProductByIdTask.Result);
            this.ValidateProductResponse(artistProductByArtistTask.Result);
        }

        [Test]
        public void EnsureUriIsBuiltCorrectly()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new ArtistProductsCommand() { ArtistId = "123456" }.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/creators/123456/products/", uri.ToString());
        }        
    }
}
