// -----------------------------------------------------------------------
// <copyright file="OAuthFlowTests.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Authorization;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Internal
{
    [TestFixture]
    public class OAuthFlowTests
    {
        [Test]
        public void EnsureBasicCreationInOrder()
        {
            var flow = new OAuthUserFlow("id", "secret", MusicClientCommand.DefaultSecureBaseApiUri + MusicClientCommand.DefaultApiVersion, null);
            Assert.AreEqual(false, flow.IsBusy, "Expected a false result");
            Assert.AreEqual(false, flow.TokenCallInProgress, "Expected a false result");
            flow.TokenCallInProgress = true;
            Assert.AreEqual(true, flow.TokenCallInProgress, "Expected a true result");
            Assert.IsNull(flow.TokenResponse, "Expected null token");
        }

        [Test]
        public void EnsureAuthUriIncludesVersion()
        {
            var flow = new OAuthUserFlow("id", "secret", MusicClientCommand.DefaultSecureBaseApiUri + MusicClientCommand.DefaultApiVersion, null);
            var uri = flow.ConstructAuthorizeUri(Scope.ReadUserPlayHistory);
            Assert.AreEqual("https://sapi.mixrad.io/1.x/authorize/?response_type=code&client_id=id&scope=read_userplayhistory", uri.ToString(), "Expected a matching uri");
        }
    }
}
