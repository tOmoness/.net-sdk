// -----------------------------------------------------------------------
// <copyright file="GetUserRecentMixesCommandTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Internal.Authorization;
using MixRadio.Internal.Request;
using MixRadio.Tests.Auth;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using MixRadio.Tests.Types;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class GetUserRecentMixesCommandTests
    {
        [Test]
        public void EnsureStockPropertiesAreCorrect()
        {
            var cmd = new GetUserRecentMixesCommand();

            Assert.AreEqual(false, cmd.RequiresCountryCode, "Expected the right value");
            Assert.AreEqual(HttpMethod.Get, cmd.HttpMethod, "Expected the right value");
            
            StringBuilder sb = new StringBuilder();
            cmd.UserId = "USERID";
            cmd.AppendUriPath(sb);
            Assert.AreEqual("users/USERID/history/mixes/", sb.ToString(), "Expected the right value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureAppendUriPathThrowsExceptionForNoUserId()
        {
            var cmd = new GetUserRecentMixesCommand();
            StringBuilder sb = new StringBuilder();
            cmd.AppendUriPath(sb);
        }

        [Test]
        public async Task EnsureResponseParsedForValidRequest()
        {
            var cmd = new GetUserRecentMixesCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Success(Resources.user_recent_mixes)),
                OAuth2 = new OAuth2(new FakeAuthHeaderProvider()),
                ClientSettings = new MockMusicClientSettings("test", "gb", null),
                UserId = "userid"
            };

            // Initialise MusicClient.SettingsInternal by creating a MusicClient...
            new MusicClient("test", "gb");
            
            var t = await cmd.ExecuteAsync(null);
            Assert.IsNotNull(t.Result, "Expected a result");
            Assert.Greater(t.Result.Count, 0, "Expected results");
            Assert.IsNull(t.Error, "Expected no errors");
        }

        [Test]
        [ExpectedException(typeof(UserAuthRequiredException))]
        public async Task EnsureAuthRequiredExceptionThrownForUnAuthedClient()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            var result = await client.GetUserRecentMixesAsync();
        }

        [Test]
        public async Task EnsureNormalCallWorksAsExpected()
        {
            var client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.user_recent_mixes));
            client.SetAuthenticationToken(AuthTokenTests.GetTestAuthToken());
            var result = await client.GetUserRecentMixesAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
