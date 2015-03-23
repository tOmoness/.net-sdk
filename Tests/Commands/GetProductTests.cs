// -----------------------------------------------------------------------
// <copyright file="GetProductTests.cs" company="MixRadio">
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
using MixRadio.Internal.Request;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class GetProductTests : ProductTestBase
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetProductThrowsExceptionForNullProductId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            await client.GetProductAsync(nullId);
        }

        [Test]
        public async Task EnsureGetProductReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            this.ValidateProductResponse(await client.GetProductAsync("test"));
        }

        [Test]
        public async Task EnsureGetProductReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            Response<Product> result = await client.GetProductAsync("test");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        public async Task EnsureGetProductReturnsUnknownErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.ConflictServerError("{ \"error\":\"true\"}")));
            Response<Product> result = await client.GetProductAsync("test");
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
            new ProductCommand { ProductId = "1234" }.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/1234/", uri.ToString());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetTrackSampleUriThrowsExceptionForNullId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", (IApiRequestHandler)null);
            client.GetTrackSampleUri(nullId);
        }

        [Test]
        public void EnsureGetTrackSampleUriIsBuiltCorrectly()
        {
            IMusicClient client = new MusicClient("test", "gb");
            Assert.AreEqual("http://api.mixrad.io/1.x/gb/products/1234/sample/?domain=music&client_id=test", client.GetTrackSampleUri("1234").ToString());
        }
    }
}
