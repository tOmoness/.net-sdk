// -----------------------------------------------------------------------
// <copyright file="CountryResolverTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Internal.Request;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MixRadio.Tests
{
    [TestFixture]
    public class CountryResolverTests
    {
        [Test]
        public void CheckApiCredentialsValidated()
        {
            string nullKey = null;
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new CountryResolver(nullKey); }));
        }

        [Test]
        public void EnsureDefaultRequestHandlerIsCreated()
        {
            CountryResolver client = new CountryResolver("test");
            Assert.AreEqual(client.RequestHandler.GetType(), typeof(ApiRequestHandler), "Expected the default handler");
        }

        [Test]
        [ExpectedException(typeof(InvalidApiCredentialsException))]
        public async Task EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403()
        {
            CountryResolver client = new CountryResolver("badkey", new MockApiRequestHandler(FakeResponse.Forbidden()));
            await client.CheckAvailabilityAsync("gb");
        }

        [Test]
        [ExpectedException(typeof(InvalidCountryCodeException))]
        public async Task EnsureCheckAvailabilityThrowsExceptionForNullCountryCode()
        {
            ICountryResolver client = new CountryResolver("test", new MockApiRequestHandler(Resources.country));
            await client.CheckAvailabilityAsync(null);
        }

        [Test]
        public void ApiMethodsDefaultToGetHttpMethod()
        {
            var resolver = new CountryResolverCommand("test", new MockApiRequestHandler(Resources.country));
            Assert.AreEqual(HttpMethod.Get, resolver.HttpMethod);
        }

        [Test]
        public void ApiMethodsDefaultToNullContentType()
        {
            var resolver = new CountryResolverCommand("test", new MockApiRequestHandler(Resources.country));
            Assert.IsNull(resolver.ContentType);
        }

        [Test]
        public async Task EnsureCheckAvailabilityWorksForValidCountry()
        {
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(Resources.country));
            bool result = await client.CheckAvailabilityAsync("gb");
            Assert.IsTrue(result, "Expected a true result");
        }

        [Test]
        public async Task EnsureCheckAvailabilityReturnsFailsForInvalidCountry()
        {
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.NotFound("{}")));
            bool result = await client.CheckAvailabilityAsync("xx");
            Assert.IsFalse(result, "Expected a false result");
        }

        [Test]
        [ExpectedException(typeof(ApiCallFailedException))]
        public async Task EnsureCheckAvailabilityIsTreatedAsErrorForNetworkFailure()
        {
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.NotFound()));
            bool result = await client.CheckAvailabilityAsync("xx");
        }

        [Test]
        public async Task EnsureCountryResolverPassesDefaultSettings()
        {
            MockApiRequestHandler mockHandler = new MockApiRequestHandler(Resources.country);
            ICountryResolver client = new CountryResolver("test1", mockHandler);
            bool result = await client.CheckAvailabilityAsync("xx");
            Assert.AreEqual("test1", mockHandler.LastUsedSettings.ClientId);
            Assert.AreEqual(null, mockHandler.LastUsedSettings.CountryCode);
            Assert.AreEqual(false, mockHandler.LastUsedSettings.CountryCodeBasedOnRegionInfo);
            Assert.AreEqual(MusicClientCommand.DefaultBaseApiUri, mockHandler.LastUsedSettings.ApiBaseUrl);
            Assert.AreEqual(MusicClientCommand.DefaultSecureBaseApiUri, mockHandler.LastUsedSettings.SecureApiBaseUrl);
        }

        [Test]
        [ExpectedException(typeof(ApiCallFailedException))]
        public async Task EnsureCountryWithoutItemsRaisesApiCallFailedException()
        {
            MockApiRequestHandler mockHandler = new MockApiRequestHandler(System.Text.Encoding.UTF8.GetBytes("{ \"items\": [] }"));
            ICountryResolver client = new CountryResolver("test1", mockHandler);
            bool result = await client.CheckAvailabilityAsync("xx");
        }

        [Test]
        [ExpectedException(typeof(ApiCallFailedException))]
        public async Task EnsureCountryResolverWithInvalidContentTypeRaisesApiCallFailedException()
        {
            ICountryResolver client = new CountryResolver("test1", new MockApiRequestHandler(FakeResponse.Success(null, null)));
            bool result = await client.CheckAvailabilityAsync("xx");
        }

        [Test]
        [ExpectedException(typeof(ApiCallFailedException))]
        public async Task EnsureCheckAvailabilityReturnsErrorForFailedCall()
        {
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.GatewayTimeout()));
            bool result = await client.CheckAvailabilityAsync("gb");
        }
    }
}
