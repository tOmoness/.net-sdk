// -----------------------------------------------------------------------
// <copyright file="TokenResponseTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MixRadio.Internal.Authorization;
using MixRadio.Types;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MixRadio.Tests
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

        [Test]
        public void ScopeJsonConverterCanConvertScope()
        {
            var converter = new TokenResponse.ScopeJsonConverter();

            Assert.IsTrue(converter.CanConvert(typeof(Scope)));
        }
    }
}
