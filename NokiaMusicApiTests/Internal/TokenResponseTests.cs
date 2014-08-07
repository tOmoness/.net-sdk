// -----------------------------------------------------------------------
// <copyright file="TokenResponseTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Authorization;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class TokenResponseTests
    {
        [Test]
        public void TestPropertiesAndSerialisation()
        {
            TokenResponse token = new TokenResponse()
            {
                AccessToken = "token",
                ExpiresIn = 120,
                RefreshToken = "refresh",
                Territory = "gb",
                UserId = Guid.NewGuid()
            };

            JToken json = token.ToJToken();

            TokenResponse rehydrated = TokenResponse.FromJToken(json, null);

            Assert.AreEqual(token.AccessToken, rehydrated.AccessToken, "Expected AccessToken to match");
            Assert.AreEqual(token.ExpiresIn, rehydrated.ExpiresIn, "Expected ExpiresIn to match");
            Assert.AreEqual(token.RefreshToken, rehydrated.RefreshToken, "Expected RefreshToken to match");
            Assert.AreEqual(token.Territory, rehydrated.Territory, "Expected Territory to match");
            Assert.AreEqual(token.UserId, rehydrated.UserId, "Expected UserId to match");
        }
    }
}
