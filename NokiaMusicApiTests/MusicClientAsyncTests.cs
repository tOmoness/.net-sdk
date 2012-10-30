// -----------------------------------------------------------------------
// <copyright file="MusicClientAsyncTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class MusicClientAsyncTests
    {
        [Test]
        public void CheckApiCredentialsValidated()
        {
            string nullKey = null;
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClientAsync(nullKey, nullKey); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClientAsync("test-app-id", nullKey); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClientAsync(nullKey, nullKey, "gb"); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new MusicClientAsync("test-app-id", nullKey, "gb"); }));
        }

        [Test]
        public void CheckCountryCodeValidated()
        {
            string nullCountry = null;
            Assert.DoesNotThrow(new TestDelegate(() => { new MusicClientAsync(@"test", @"test"); }));
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClientAsync(@"test", @"test", nullCountry); }));
        }

        [Test]
        public void ValidateCountryCodeEmptyCheckedAndLowerCased()
        {
            // Check InvalidCountryCodeException thrown for empty string as well as null
            string nullCountry = null;
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClientAsync("test", "test", nullCountry); }));
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClientAsync("test", "test", string.Empty); }));

            // Check InvalidCountryCodeException thrown for invalid length
            Assert.Throws(typeof(InvalidCountryCodeException), new TestDelegate(() => { new MusicClientAsync("test", "test", "GBR"); }));

            // Check InvalidCountryCodeException thrown for invalid length
            Assert.DoesNotThrow(new TestDelegate(() => { new MusicClientAsync("test", "test", "gb"); }));
        }
    }
}
