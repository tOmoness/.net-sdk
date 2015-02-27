﻿// -----------------------------------------------------------------------
// <copyright file="UserAuthTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Tests.Types;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Auth
{
    /// <summary>
    /// User Auth tests
    /// </summary>
    [TestFixture]
    public class UserAuthTests
    {
        [Test]
        public void EnsureAuthTokenSetCorrectly()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.SetAuthenticationToken(AuthTokenTests.GetTestAuthToken());
            Assert.AreEqual(true, client.IsUserAuthenticated, "Expected an authenticated user");
            Assert.AreEqual(true, client.IsUserTokenActive, "Expected an authenticated user");
        }

        [Test]
        public void EnsureGetAuthenticationUriIncludesVersion()
        {
            var client = new MusicClient("id");
            var uri = client.GetAuthenticationUri(Scope.ReadUserPlayHistory);
            Assert.AreEqual("https://sapi.mixrad.io/1.x/authorize/?response_type=code&client_id=id&scope=read_userplayhistory", uri.ToString(), "Expected a matching uri");
        }

        [Test]
        public async Task EnsureGetAuthenticationTokenAsyncReturnsExistingTokenIfValid()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            var token = AuthTokenTests.GetTestAuthToken();
            client.SetAuthenticationToken(token);
            var result = await client.GetAuthenticationTokenAsync("secret", "code");
            Assert.AreEqual(token.AccessToken, result.AccessToken, "Expected the same token");
        }

        [Test]
        public async Task EnsureGetAuthenticationTokenAsyncReturnsATokenForValidCalls()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.token_response));
            var result = await client.GetAuthenticationTokenAsync("secret", "code");
            Assert.IsNotNull(result, "Expected a result");
        }

        [Test]
        public async Task EnsureGetAuthenticationTokenAsyncReturnsNullForInvalidCalls()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.InternalServerError()));
            var result = await client.GetAuthenticationTokenAsync("secret", "code");
            Assert.IsNull(result, "Expected no result");
        }

        [Test]
        [ExpectedException(typeof(UserAuthRequiredException))]
        public async Task EnsureRefreshAuthenticationTokenAsyncThrowsWhenNoTokenSet()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            await client.RefreshAuthenticationTokenAsync("secret");
        }

        [Test]
        public async Task EnsureRefreshAuthenticationTokenAsyncReturnsExistingTokenIfValid()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            var token = AuthTokenTests.GetTestAuthToken();
            client.SetAuthenticationToken(token);
            var result = await client.RefreshAuthenticationTokenAsync("secret");
            Assert.AreEqual(token.AccessToken, result.AccessToken, "Expected the same token");
        }

        [Test]
        public async Task EnsureRefreshAuthenticationTokenAsyncReturnsTokenForValidCalls()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.token_response));
            var token = AuthTokenTests.GetTestAuthToken();
            token.ExpiresUtc = DateTime.UtcNow.AddDays(-1);
            client.SetAuthenticationToken(token);
            var result = await client.RefreshAuthenticationTokenAsync("secret");
            Assert.IsNotNull(result, "Expected no result");
        }

        [Test]
        public async Task EnsureRefreshAuthenticationTokenAsyncReturnsNullForInvalidCalls()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.InternalServerError()));
            var token = AuthTokenTests.GetTestAuthToken();
            token.ExpiresUtc = DateTime.UtcNow.AddDays(-1);
            client.SetAuthenticationToken(token);
            var result = await client.RefreshAuthenticationTokenAsync("secret");
            Assert.IsNull(result, "Expected no result");
        }
    }
}
