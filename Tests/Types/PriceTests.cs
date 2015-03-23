// -----------------------------------------------------------------------
// <copyright file="PriceTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Types
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
