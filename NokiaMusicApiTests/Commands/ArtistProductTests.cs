// -----------------------------------------------------------------------
// <copyright file="ArtistProductTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class ArtistProductTests : ProductTestBase
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistProductsThrowsExceptionForNullArtistId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProductsAsync(nullId).Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetArtistProductsThrowsExceptionForNullArtist()
        {
            Artist nullArtist = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            client.GetArtistProductsAsync(nullArtist).Wait();
        }

        [Test]
        public void EnsureGetArtistProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            var task = client.GetArtistProductsAsync(new Artist() { Id = "test" }, Category.Album);
            task.Wait();
            this.ValidateProductListResponse(task.Result);
            task = client.GetArtistProductsAsync("test");
            task.Wait();
            this.ValidateProductListResponse(task.Result);
        }

        [Test]
        public async Task EnsureGetArtistProductsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Product> result = await client.GetArtistProductsAsync("test", orderBy: OrderBy.Name, sortOrder: SortOrder.Ascend);
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
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
