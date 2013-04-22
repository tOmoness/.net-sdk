// -----------------------------------------------------------------------
// <copyright file="GetProductTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class GetProductTests : ProductTestBase
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetProductThrowsExceptionForNullProductId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            client.GetProduct((Response<Product> result) => { }, nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetProductThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            client.GetProduct(null, "test");
        }

        [Test]
        public void EnsureGetProductReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            client.GetProduct(this.ValidateProductResponse, "test");
        }

        [Test]
        public void EnsureGetProductReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetProduct(
                (Response<Product> result) =>
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
        public void EnsureAsyncGetProductsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.single_product));
            var task = client.GetProductAsync("test");
            task.Wait();
            this.ValidateProductResponse(task.Result);
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
            IMusicClient client = new MusicClient("test", "gb", null);
            client.GetTrackSampleUri(nullId);
        }

        [Test]
        public void EnsureGetTrackSampleUriIsBuiltCorrectly()
        {
            IMusicClient client = new MusicClient("test", "gb", null);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/1234/sample/?domain=music&app_id=test", client.GetTrackSampleUri("1234").ToString());
        }
    }
}
