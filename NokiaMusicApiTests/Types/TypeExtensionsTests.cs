// -----------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Nokia.Music.Internal.Parsing;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// TypeExtensions tests
    /// </summary>
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void EnsureValidCategoriesAreParsed()
        {
            Assert.AreEqual(Category.Artist, ParseHelper.ParseEnumOrDefault<Category>("Artist"), "Expected Artist result");
        }

        [Test]
        public void EnsureInValidCategoriesReturnUnknown()
        {
            Assert.AreEqual(Category.Unknown, ParseHelper.ParseEnumOrDefault<Category>("BadCategory"), "Expected Unknown result");
        }
    }
}
