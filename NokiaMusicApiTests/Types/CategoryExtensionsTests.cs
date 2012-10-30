// -----------------------------------------------------------------------
// <copyright file="CategoryExtensionsTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// CategoryExtensions tests
    /// </summary>
    [TestFixture]
    public class CategoryExtensionsTests
    {
        [Test]
        public void EnsureValidCategoriesAreParsed()
        {
            Assert.AreEqual(Category.Artist, CategoryExtensions.ParseCategory("Artist"), "Expected Artist result");
        }

        [Test]
        public void EnsureInValidCategoriesReturnUnknown()
        {
            Assert.AreEqual(Category.Unknown, CategoryExtensions.ParseCategory("BadCategory"), "Expected Unknown result");
        }
    }
}
