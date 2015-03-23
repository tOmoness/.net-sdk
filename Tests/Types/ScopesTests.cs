// -----------------------------------------------------------------------
// <copyright file="ScopesTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Types
{
    /// <summary>
    /// AuthScopes tests
    /// </summary>
    [TestFixture]
    public class ScopesTests
    {
        [Test]
        public void TestScopeConversion()
        {
            Scope scope = Scope.ReadUserPlayHistory;
            const string Expected = "read_userplayhistory";

            Assert.AreEqual(Expected, scope.AsStringParam(), "Expected scopes to be converted to string form correctly");
        }
    }
}
