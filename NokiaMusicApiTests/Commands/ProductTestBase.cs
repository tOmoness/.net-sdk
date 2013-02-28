// <copyright file="ProductTestBase.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Commands
{
    public abstract class ProductTestBase
    {
        protected void ValidateProductResponse(ListResponse<Product> result)
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
                
                if (productItem.Category == Category.Album)
                {
                    Assert.That(productItem.Tracks.Count, Is.AtLeast(1));

                    foreach (var track in productItem.Tracks)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(track.Id), "Expected trackId to be populated");
                        Assert.IsFalse(string.IsNullOrEmpty(track.Name), "Expected trackName to be populated");
                        Assert.AreNotEqual(Category.Unknown, track.Category, "Expected trackCategory to be set");
                    }
                }
            }
        }
    }
}
