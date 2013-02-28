// -----------------------------------------------------------------------
// <copyright file="SimilarProductsTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Tests.Properties;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Commands
{
    [TestFixture]
    public class SimilarProductsTests : ProductTestBase
    {
        [Test]
        public void EnsureGetSimilarProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            client.GetSimilarProducts(this.ValidateProductResponse, "test");
        }

        [Test]
        public void EnsureAsyncGetSimilarProductsReturnsItems()
        {
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            var task = client.GetSimilarProducts("test");
            this.ValidateProductResponse(task.Result);
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
