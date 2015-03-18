// -----------------------------------------------------------------------
// <copyright file="OAuthTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
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
    public class OAuthTests
    {
        [Test]
        public async Task EnsureHeaderAndTokenUsedCorrectly()
        {
            IAuthHeaderDataProvider provider = new FakeAuthHeaderProvider();
            OAuth2 oauth = new OAuth2(provider);

            var headersProvider = await oauth.CreateHeadersAsync();
            Assert.IsNotNull(headersProvider, "Expected headers");
            Assert.AreEqual(1, headersProvider.Count, "Expected one header");
            Assert.AreEqual("Authorization", headersProvider.Keys.First(), "Expected Authorization header");
        }
    }
}
