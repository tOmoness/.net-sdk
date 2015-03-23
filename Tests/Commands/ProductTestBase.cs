// <copyright file="ProductTestBase.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MixRadio;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    public abstract class ProductTestBase
    {
        protected void ValidateProductListResponse(ListResponse<Product> result)
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

        protected void ValidateProductResponse(Response<Product> result)
        {
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a result");
            Assert.IsNull(result.Error, "Expected no error");

            Assert.IsFalse(string.IsNullOrEmpty(result.Result.Id), "Expected Id to be populated");
            Assert.IsFalse(string.IsNullOrEmpty(result.Result.Name), "Expected Name to be populated");
            Assert.AreNotEqual(Category.Unknown, result.Result.Category, "Expected Category to be set");

            if (result.Result.Category == Category.Album)
            {
                Assert.That(result.Result.Tracks.Count, Is.AtLeast(1));

                foreach (var track in result.Result.Tracks)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(track.Id), "Expected trackId to be populated");
                    Assert.IsFalse(string.IsNullOrEmpty(track.Name), "Expected trackName to be populated");
                    Assert.AreNotEqual(Category.Unknown, track.Category, "Expected trackCategory to be set");
                }
            }
        }
    }
}
