// -----------------------------------------------------------------------
// <copyright file="OAuthFlowTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Nokia.Music.Internal.Authorization;
using NUnit.Framework;

namespace Nokia.Music.Tests.Internal
{
    [TestFixture]
    public class OAuthFlowTests
    {
        [Test]
        public async Task EnsureBasicCreationInOrder()
        {
            var flow = new OAuthUserFlow("id", "secret", null);
            Assert.AreEqual(false, flow.IsBusy, "Expected a false result");
            Assert.AreEqual(false, flow.TokenCallInProgress, "Expected a false result");
            flow.TokenCallInProgress = true;
            Assert.AreEqual(true, flow.TokenCallInProgress, "Expected a true result");
        }
    }
}
