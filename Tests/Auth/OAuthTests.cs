// -----------------------------------------------------------------------
// <copyright file="OAuthTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using MixRadio.Internal.Authorization;
using MixRadio.Tests.Internal;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Auth
{
    /// <summary>
    /// OAuth general tests
    /// </summary>
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
            await oauth.InvalidateUserTokenAsync();
        }

        [Test]
        public async Task EnsurePrivateMembersReturnNulls()
        {
            var provider = new OAuthHeaderDataProvider("token", "userid");
            Assert.IsNull(provider.HashForTokenAuthentication("data"), "Expected null response for private API method");
            await provider.InvalidateUserTokenAsync();
        }
    }
}
