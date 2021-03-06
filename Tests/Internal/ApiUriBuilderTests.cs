﻿// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilderTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Internal;
using MixRadio.Internal.Request;
using MixRadio.Tests.Internal;
using NUnit.Framework;

namespace MixRadio.Tests
{
    [TestFixture]
    public class ApiUriBuilderTests
    {
        private const string ClientId = @"test_clientid";
        private const string Country = @"gb";
        private const string Language = @"en";

        [Test]
        public void CheckParametersAreValidated()
        {
            IApiUriBuilder builder = new ApiUriBuilder();

            // Check ApiMethod param...
            Assert.Throws(typeof(ArgumentNullException), new TestDelegate(() => { builder.BuildUri(null, null, null); }));

            // Check settings param...
            Assert.Throws(typeof(ArgumentNullException), new TestDelegate(() => { builder.BuildUri(new SearchCommand(), null, null); }));

            // Check API Key is required...
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(new CountryResolverCommand(ClientId, null), new MockMusicClientSettings(null, null, null), null); }));

            // Check Country Code is required...
            Assert.Throws(typeof(CountryCodeRequiredException), new TestDelegate(() => { builder.BuildUri(new SearchCommand(), new MockMusicClientSettings(ClientId, null, null), null); }));
        }

        [Test]
        public void EnsureBlankTerritoryCommandsDoNotIncludeTerritory()
        {
            MockBlankTerritoryCommand cmd = new MockBlankTerritoryCommand();
            IMusicClientSettings settings = new MockMusicClientSettings("test", "country", null);

            IApiUriBuilder builder = new ApiUriBuilder();
            Uri uri = builder.BuildUri(cmd, settings, null);

            Assert.IsTrue(uri.ToString().Contains("/-/"), "Expected the country code to be '-'");
            Assert.IsFalse(uri.ToString().Contains("country"), "Expected the country code not to be included in the URI");
        }

        [Test]
        public void EnsureBlankLanguageCommandsDoNotIncludeLanguage()
        {
            MockApiCommand cmd = new MockApiCommand();
            IMusicClientSettings settings = new MockMusicClientSettings(ClientId, Country, null);

            IApiUriBuilder builder = new ApiUriBuilder();
            Uri uri = builder.BuildUri(cmd, settings, null);

            Assert.IsFalse(uri.ToString().Contains("lang="), "Expected the language code not to be included in the URI");
        }

        [Test]
        public void EnsureSpecificApiVersionCommandsUsesCorrectVersion()
        {
            MockApiCommand cmd = new MockApiCommand() { BaseApiVersion = "2.x/" };
            IMusicClientSettings settings = new MockMusicClientSettings(ClientId, Country, null);

            IApiUriBuilder builder = new ApiUriBuilder();
            Uri uri = builder.BuildUri(cmd, settings, null);

            Assert.IsTrue(uri.ToString().Contains("2.x/"), "Expected the correct version to be included in the URI");
        }

        [Test]
        public void EnsureFullUriCanBeBuilt()
        {
            Uri expected = new Uri("http://api.mixrad.io/1.x/gb/?client_id=test_clientid&domain=music&lang=en&q=test&q2=test2");
            Uri result = new ApiUriBuilder().BuildUri(new SearchCommand(), new MockMusicClientSettings(ClientId, Country, Language), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("q", "test"), new KeyValuePair<string, string>("q2", "test2") });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateOverriddenBaseUri()
        {
            Uri expected = new Uri("http://api2.ent.nokia.com/1.x/gb/mixes/groups/test123/?client_id=test_clientid&domain=music&lang=en&id=testQs");
            Uri result = new ApiUriBuilder().BuildUri(new MixesCommand() { BaseApiUri = "http://api2.ent.nokia.com/", MixGroupId = "test123" }, new MockMusicClientSettings(ClientId, Country, Language), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("id", "testQs") });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void QueryStringCanAcceptNullValues()
        {
            Uri expected = new Uri("http://api.mixrad.io/1.x/gb/?client_id=test_clientid&domain=music&lang=en&q=&q2=");
            Uri result = new ApiUriBuilder().BuildUri(new SearchCommand(), new MockMusicClientSettings(ClientId, Country, Language), new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("q", null), new KeyValuePair<string, string>("q2", null) });
            Assert.AreEqual(expected, result);
        }
    }
}
