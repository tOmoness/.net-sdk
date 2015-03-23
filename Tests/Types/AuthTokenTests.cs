// -----------------------------------------------------------------------
// <copyright file="AuthTokenTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using MixRadio.Internal.Authorization;
using MixRadio.Tests.Auth;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Types
{
    /// <summary>
    /// AuthToken tests
    /// </summary>
    [TestFixture]
    public class AuthTokenTests
    {
        [Test]
        public void EnsureAuthTokenSerialisationAndDeserialisationWorkCorrectly()
        {
            var original = AuthTokenTests.GetTestAuthToken();
            var serialised = original.ToString();
            var dehydrated = AuthToken.FromJson(serialised);
            Assert.AreEqual(original.AccessToken, dehydrated.AccessToken, "Expected the same AccessToken");
            Assert.AreEqual(original.ExpiresUtc, dehydrated.ExpiresUtc, "Expected the same ExpiresUtc");
            Assert.AreEqual(original.RefreshToken, dehydrated.RefreshToken, "Expected the same RefreshToken");
            Assert.AreEqual(original.Territory, dehydrated.Territory, "Expected the same Territory");
            Assert.AreEqual(original.UserId, dehydrated.UserId, "Expected the same UserId");
        }

        [Test]
        public void EnsureAuthTokenConversionsWorkCorrectly()
        {
            Assert.IsNull(AuthToken.FromTokenResponse(null), "Expected null result");

            var original = AuthTokenTests.GetTestAuthToken();
            var token = original.ToTokenResponse();
            var converted = AuthToken.FromTokenResponse(token);
            Assert.AreEqual(original.AccessToken, converted.AccessToken, "Expected the same AccessToken");
            Assert.AreEqual(original.ExpiresUtc, converted.ExpiresUtc, "Expected the same ExpiresUtc");
            Assert.AreEqual(original.RefreshToken, converted.RefreshToken, "Expected the same RefreshToken");
            Assert.AreEqual(original.Territory, converted.Territory, "Expected the same Territory");
            Assert.AreEqual(original.UserId, converted.UserId, "Expected the same UserId");
        }

        [Test]
        public void EnsureAuthTokenCreatedFromXamarinAuthDictionary()
        {
            Assert.IsNull(AuthToken.FromXamarinDictionary(null), "Expected null result");

            var original = AuthTokenTests.GetTestAuthToken();
            var authProperties = new Dictionary<string, string>();
            var expiresIn = (original.ExpiresUtc - DateTime.UtcNow.AddMinutes(-1)).TotalSeconds;
            authProperties.Add("access_token", original.AccessToken);
            authProperties.Add("expires_in", expiresIn.ToString());
            authProperties.Add("refresh_token", original.RefreshToken);
            authProperties.Add("userid", original.UserId.ToString());
            authProperties.Add("territory", original.Territory);

            var converted = AuthToken.FromXamarinDictionary(authProperties);
            Assert.AreEqual(original.AccessToken, converted.AccessToken, "Expected the same AccessToken");
            Assert.AreEqual(Convert.ToInt64(new TimeSpan(original.ExpiresUtc.Ticks).TotalMinutes), Convert.ToInt64(new TimeSpan(converted.ExpiresUtc.Ticks).TotalMinutes), "Expected the same ExpiresUtc");
            Assert.AreEqual(original.RefreshToken, converted.RefreshToken, "Expected the same RefreshToken");
            Assert.AreEqual(original.Territory, converted.Territory, "Expected the same Territory");
            Assert.AreEqual(original.UserId, converted.UserId, "Expected the same UserId");
        }

        internal static AuthToken GetTestAuthToken()
        {
            return new AuthToken()
            {
                AccessToken = "token",
                ExpiresUtc = DateTime.UtcNow.AddDays(1),
                RefreshToken = "refresh",
                Territory = "in",
                UserId = Guid.NewGuid()
            };
        }
    }
}
