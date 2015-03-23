// -----------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using MixRadio.Internal.Parsing;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Types
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
