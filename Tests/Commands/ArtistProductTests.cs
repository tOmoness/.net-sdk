// -----------------------------------------------------------------------
// <copyright file="ArtistProductTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class ArtistProductTests : ProductTestBase
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetArtistProductsThrowsExceptionForNullArtistId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            await client.GetArtistProductsAsync(nullId);
        }

        [Test]
        public async Task EnsureGetArtistProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_artists));
            var t = await client.GetArtistProductsAsync("test", Category.Album);
            this.ValidateProductListResponse(t);
            t = await client.GetArtistProductsAsync("test");
            this.ValidateProductListResponse(t);
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
