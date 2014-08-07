// -----------------------------------------------------------------------
// <copyright file="GetUserRecentMixesCommandTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
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
using Nokia.Music.Commands;
using Nokia.Music.Internal.Authorization;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
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
    }
}
