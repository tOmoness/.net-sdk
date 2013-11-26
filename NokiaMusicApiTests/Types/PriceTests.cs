// -----------------------------------------------------------------------
// <copyright file="PriceTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Private.Tests.Types
{
    /// <summary>
    /// Price tests
    /// </summary>
    [TestFixture]
    public class PriceTests
    {
        [Test]
        public void CanCreatePriceObject()
        {
           var expectedPrice = Price.FromPriceInfo(1.50d, "GB");

            Assert.AreEqual(1.50, expectedPrice.Value);
            Assert.AreEqual("GB", expectedPrice.Currency);
        }
    }
}
