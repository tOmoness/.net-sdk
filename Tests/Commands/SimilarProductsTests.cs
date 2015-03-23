﻿// -----------------------------------------------------------------------
// <copyright file="SimilarProductsTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class SimilarProductsTests : ProductTestBase
    {
        [Test]
        public async Task EnsureGetSimilarProductsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            this.ValidateProductListResponse(await client.GetSimilarProductsAsync("test"));
            this.ValidateProductListResponse(await client.GetSimilarProductsAsync("test"));

            var task = client.GetSimilarProductsAsync("test");
            this.ValidateProductListResponse(task.Result);

            var productTask = client.GetSimilarProductsAsync("test");
            this.ValidateProductListResponse(productTask.Result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureExceptionIsThrownIfNullProductId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.product_parse_tests));
            await client.GetSimilarProductsAsync(string.Empty);
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
