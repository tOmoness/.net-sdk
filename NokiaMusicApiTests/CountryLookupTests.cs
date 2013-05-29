// -----------------------------------------------------------------------
// <copyright file="CountryLookupTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class CountryLookupTests
    {
        [Test]
        [ExpectedException(typeof(InvalidCountryCodeException))]
        public void EnsureCheckAvailabilityThrowsExceptionForNullCountryCode()
        {
            ICountryResolver client = new CountryResolver("test", new MockApiRequestHandler(Resources.country));
            client.CheckAvailability(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureCheckAvailabilityThrowsExceptionForNullCallback()
        {
            ICountryResolver client = new CountryResolver("test", new MockApiRequestHandler(Resources.country));
            client.CheckAvailability(null, "gb");
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
        public void EnsureCheckAvailabilityWorksForValidCountry()
        {
            Guid requestId = new Guid();
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(Resources.country), requestId);
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a result");
                    Assert.AreEqual(requestId, result.RequestId, "Expected a matching request Id");
                    Assert.IsTrue(result.Result, "Expected a true result");
                    Assert.IsNull(result.Error, "Expected no error");
                },
                "gb");

            var task = client.CheckAvailabilityAsync("gb");
        }

        [Test]
        public void EnsureCheckAvailabilityReturnsFailsForInvalidCountry()
        {
            Guid requestId = new Guid();
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.NotFound("{}")), requestId);
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode.Value, "Expected a 404 response");
                    Assert.IsNotNull(result.Result, "Expected a result");
                    Assert.IsFalse(result.Result, "Expected a false result");
                    Assert.AreEqual(requestId, result.RequestId, "Expected a matching request Id");
                    Assert.IsNull(result.Error, "Expected no error");
                },
                "xx");
        }

        [Test]
        public void EnsureCheckAvailabilityIsTreatedAsErrorForNetworkFailure()
        {
            Guid requestId = new Guid();
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.NotFound()), requestId);
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result.Error, "Expected an error");
                },
                "xx");
        }

        [Test]
        public void EnsureCountryResolverPassesDefaultSettings()
        {
            MockApiRequestHandler mockHandler = new MockApiRequestHandler(FakeResponse.NotFound());
            ICountryResolver client = new CountryResolver("test1", mockHandler);
            client.CheckAvailability(result => Assert.IsNotNull(result, "Expected a result"), "xx");
            
            Assert.AreEqual("test1", mockHandler.LastUsedSettings.AppId);
            Assert.AreEqual(null, mockHandler.LastUsedSettings.CountryCode);
            Assert.AreEqual(false, mockHandler.LastUsedSettings.CountryCodeBasedOnRegionInfo);
        }

        [Test]
        public void EnsureCheckAvailabilityReturnsErrorForFailedCall()
        {
            Guid requestId = new Guid();
            CountryResolver client = new CountryResolver("test", new MockApiRequestHandler(FakeResponse.GatewayTimeout()), requestId);
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non 200 response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(requestId, result.RequestId, "Expected a matching request Id");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                "gb");
        }
    }
}
