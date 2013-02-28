// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilderTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal.Request;
using Nokia.Music.Phone.Tests.Internal;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class ApiUriBuilderTests
    {
        private const string AppId = @"test_appid";
        private const string AppCode = @"test_appcode";
        private const string Country = @"gb";

        [Test]
        public void CheckParametersAreValidated()
        {
            IApiUriBuilder builder = new ApiUriBuilder();

            // Check ApiMethod param...
            Assert.Throws(typeof(ArgumentNullException), new TestDelegate(() => { builder.BuildUri(null, null, null); }));

            // Check settings param...
            Assert.Throws(typeof(ArgumentNullException), new TestDelegate(() => { builder.BuildUri(new SearchCommand(), null, null); }));

            // Check API Key is required...
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(new CountryResolverCommand(AppId, AppCode, null), new MockMusicClientSettings(null, null, null), null); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(new CountryResolverCommand(AppId, AppCode, null), new MockMusicClientSettings(AppId, null, null), null); }));
            
            // Check Country Code is required...
            Assert.Throws(typeof(CountryCodeRequiredException), new TestDelegate(() => { builder.BuildUri(new SearchCommand(), new MockMusicClientSettings(AppId, AppCode, null), null); }));
        }

        [Test]
        public void EnsureFullUriCanBeBuilt()
        {
            Uri expected = new Uri("http://api.ent.nokia.com/1.x/gb/?app_id=test_appid&app_code=test_appcode&domain=music&q=test&q2=test2");
            Uri result = new ApiUriBuilder().BuildUri(new SearchCommand(), new MockMusicClientSettings(AppId, AppCode, Country), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("q", "test"), new KeyValuePair<string, string>("q2", "test2") });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateOverriddenBaseUri()
        {
            Uri expected = new Uri("http://api.ent.nokia.com/2.0/gb/mixes/groups/test123/?app_id=test_appid&app_code=test_appcode&domain=music&id=testQs");
            Uri result = new ApiUriBuilder().BuildUri(new MixesCommand() { BaseApiUri = "http://api.ent.nokia.com/2.0/", MixGroupId = "test123" }, new MockMusicClientSettings(AppId, AppCode, Country), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("id", "testQs") });
            Assert.AreEqual(expected, result);
        }
    }
}
