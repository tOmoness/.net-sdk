// -----------------------------------------------------------------------
// <copyright file="GetUserPlayHistoryCommandTests.cs" company="Nokia">
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
    public class GetUserPlayHistoryCommandTests
    {
        [Test]
        public void EnsureStockPropertiesAreCorrect()
        {
            var cmd = new GetUserPlayHistoryCommand();

            Assert.AreEqual(false, cmd.RequiresCountryCode, "Expected the right value");
            Assert.AreEqual(HttpMethod.Get, cmd.HttpMethod, "Expected the right value");
            Assert.AreEqual(UserEventTarget.Track, cmd.Target, "Expected the right value");
            
            StringBuilder sb = new StringBuilder();
            cmd.UserId = "USERID";
            cmd.AppendUriPath(sb);
            Assert.AreEqual("users/USERID/history/", sb.ToString(), "Expected the right value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureAppendUriPathThrowsExceptionForNoUserId()
        {
            var cmd = new GetUserPlayHistoryCommand();
            StringBuilder sb = new StringBuilder();
            cmd.AppendUriPath(sb);
        }

        [Test]
        public void EnsurePropertiesPersist()
        {
            var cmd = new GetUserPlayHistoryCommand();
            cmd.Action = UserEventAction.Complete;
            Assert.AreEqual(UserEventAction.Complete, cmd.Action, "Expected the right value");
        }

        [Test]
        public void EnsureBuildParamsDoesNotSendUnknownFilters()
        {
            var cmd = new GetUserPlayHistoryCommand();

            Assert.AreEqual(3, cmd.BuildQueryStringParams().Count, "Expected only startindex / itemsperpage at default values and target=track");

            cmd.Action = UserEventAction.Complete;

            Assert.AreEqual(4, cmd.BuildQueryStringParams().Count, "Expected all 4 params set");
        }

        [Test]
        public async Task EnsureResponseParsedForValidRequest()
        {
            var cmd = new GetUserPlayHistoryCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Success(Resources.usereventlist)),
                OAuth2 = new OAuth2(new FakeAuthHeaderProvider()),
                ClientSettings = new MockMusicClientSettings("test", "gb", null),
                UserId = "userid"
            };

            var t = await cmd.ExecuteAsync(null);
            Assert.IsNotNull(t.Result, "Expected a result");
            Assert.Greater(t.Result.Count, 0, "Expected results");
            Assert.IsNull(t.Error, "Expected no errors");
        }
    }
}
