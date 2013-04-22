// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilderTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class ApiUriBuilderTests
    {
        private const string AppId = @"test_appid";
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
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(new CountryResolverCommand(AppId, null), new MockMusicClientSettings(null, null), null); }));
            
            // Check Country Code is required...
            Assert.Throws(typeof(CountryCodeRequiredException), new TestDelegate(() => { builder.BuildUri(new SearchCommand(), new MockMusicClientSettings(AppId, null), null); }));
        }

        [Test]
        public void EnsureBlankTerritoryCommandsDoNotIncludeTerritory()
        {
            MockBlankTerritoryCommand cmd = new MockBlankTerritoryCommand();
            IMusicClientSettings settings = new MockMusicClientSettings("test", "country");

            IApiUriBuilder builder = new ApiUriBuilder();
            Uri uri = builder.BuildUri(cmd, settings, null);

            Assert.IsTrue(uri.ToString().Contains("/-/"), "Expected the country code to be '-'");
            Assert.IsFalse(uri.ToString().Contains("country"), "Expected the country code not to be included in the URI");
        }

        [Test]
        public void EnsureFullUriCanBeBuilt()
        {
            Uri expected = new Uri("http://api.ent.nokia.com/1.x/gb/?app_id=test_appid&domain=music&q=test&q2=test2");
            Uri result = new ApiUriBuilder().BuildUri(new SearchCommand(), new MockMusicClientSettings(AppId, Country), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("q", "test"), new KeyValuePair<string, string>("q2", "test2") });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateOverriddenBaseUri()
        {
            Uri expected = new Uri("http://api.ent.nokia.com/2.0/gb/mixes/groups/test123/?app_id=test_appid&domain=music&id=testQs");
            Uri result = new ApiUriBuilder().BuildUri(new MixesCommand() { BaseApiUri = "http://api.ent.nokia.com/2.0/", MixGroupId = "test123" }, new MockMusicClientSettings(AppId, Country), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("id", "testQs") });
            Assert.AreEqual(expected, result);
        }
    }
}
