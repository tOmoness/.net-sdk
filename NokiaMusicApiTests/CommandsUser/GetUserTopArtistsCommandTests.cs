// -----------------------------------------------------------------------
// <copyright file="GetUserTopArtistsCommandTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
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
    public class GetUserTopArtistsCommandTests
    {
        [Test]
        public void EnsureStockPropertiesAreCorrect()
        {
            var cmd = new GetUserTopArtistsCommand();

            Assert.AreEqual(false, cmd.RequiresCountryCode, "Expected the right value");
            Assert.AreEqual(HttpMethod.Get, cmd.HttpMethod, "Expected the right value");
            
            StringBuilder sb = new StringBuilder();
            cmd.UserId = "USERID";
            cmd.AppendUriPath(sb);
            Assert.AreEqual("users/USERID/history/creators/", sb.ToString(), "Expected the right value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureAppendUriPathThrowsExceptionForNoUserId()
        {
            var cmd = new GetUserTopArtistsCommand();
            StringBuilder sb = new StringBuilder();
            cmd.AppendUriPath(sb);
        }

        [Test]
        public void EnsurePropertiesPersist()
        {
            var cmd = new GetUserTopArtistsCommand();
            DateTime date = DateTime.Now;
            cmd.StartDate = date;
            cmd.EndDate = date;
            Assert.AreEqual(date, cmd.StartDate, "Expected the right value");
            Assert.AreEqual(date, cmd.EndDate, "Expected the right value");
        }

        [Test]
        public void EnsureResponseParsedForValidRequest()
        {
            var cmd = new GetUserTopArtistsCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Success(Resources.usereventlist)),
                OAuth2 = new OAuth2(new FakeAuthHeaderProvider()),
                ClientSettings = new MockMusicClientSettings("test", "gb", null),
                UserId = "userid"
            };

            var task = cmd.InvokeAsync();
            task.Wait();
            var t = task.Result;
            Assert.IsNotNull(t.Result, "Expected a result");
            Assert.Greater(t.Result.Count, 0, "Expected results");
            Assert.IsNull(t.Error, "Expected no errors");
        }
    }
}
