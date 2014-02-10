// -----------------------------------------------------------------------
// <copyright file="AuthScopesTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// AuthScopes tests
    /// </summary>
    [TestFixture]
    public class AuthScopesTests
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
