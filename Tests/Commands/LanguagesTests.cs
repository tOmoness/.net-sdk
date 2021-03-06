﻿// -----------------------------------------------------------------------
// <copyright file="LanguagesTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class LanguagesTests
    {
        [Test]
        public async Task EnsureGetLanguagesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.languages));
            var result = await client.GetLanguagesAsync();

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (Language language in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(language.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(language.Name), "Expected Name to be populated");
            }
        }
    }
}
