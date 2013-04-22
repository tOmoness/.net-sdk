// -----------------------------------------------------------------------
// <copyright file="SimilarProductsTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class SimilarProductsTests : ProductTestBase
    {
        [Test]
        public void EnsureGetSimilarProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetSimilarProducts(this.ValidateProductListResponse, "test");
            client.GetSimilarProducts(this.ValidateProductListResponse, new Product() { Id = "test" });

            var task = client.GetSimilarProductsAsync("test");
            this.ValidateProductListResponse(task.Result);

            var productTask = client.GetSimilarProductsAsync(new Product() { Id = "test" });
            this.ValidateProductListResponse(productTask.Result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureExceptionIsThrownIfNullProductId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetSimilarProducts(null, string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureExceptionIsThrownIfNullProduct()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Product nullProduct = null;
            client.GetSimilarProducts(null, nullProduct);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureExceptionIsThrownIfNullProductAsync()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            Product nullProduct = null;
            var t = client.GetSimilarProductsAsync(nullProduct);
            t.Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureExceptionIsThrownIfNullProductIdAsync()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            var t = client.GetSimilarProductsAsync(string.Empty);
            t.Wait();
        }

        [Test]
        public void EnsureUriIsBuiltCorrectly()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new SimilarProductsCommand { ProductId = "1234" }.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/products/1234/similar", uri.ToString());
        }
    }
}
