// -----------------------------------------------------------------------
// <copyright file="CountryLookupTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Nokia.Music.Phone.Internal;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class CountryLookupTests
    {
        [Test]
        [ExpectedException(typeof(InvalidCountryCodeException))]
        public void EnsureCheckAvailabilityThrowsExceptionForNullCountryCode()
        {
            ICountryResolver client = new CountryResolver("test", "test", new SuccessfulMockApiRequestHandler());
            client.CheckAvailability(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureCheckAvailabilityThrowsExceptionForNullCallback()
        {
            ICountryResolver client = new CountryResolver("test", "test", new SuccessfulMockApiRequestHandler());
            client.CheckAvailability(null, "gb");
        }

        [Test]
        public void EnsureCheckAvailabilityWorksForValidCountry()
        {
            ICountryResolver client = new CountryResolver("test", "test", new SuccessfulMockApiRequestHandler());
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a result");
                    Assert.IsTrue(result.Result, "Expected a true result");
                    Assert.IsNull(result.Error, "Expected no error");
                },
                "gb");
        }

        [Test]
        public void EnsureCheckAvailabilityReturnsFailsForInvalidCountry()
        {
            ICountryResolver client = new CountryResolver("test", "test", new FailedMockApiRequestHandler());
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode.Value, "Expected a 404 response");
                    Assert.IsNotNull(result.Result, "Expected a result");
                    Assert.IsFalse(result.Result, "Expected a false result");
                    Assert.IsNull(result.Error, "Expected no error");
                },
                "xx");
        }

        [Test]
        public void EnsureCheckAvailabilityReturnsErrorForFailedCall()
        {
            ICountryResolver client = new CountryResolver("test", "test", new FailedMockApiRequestHandler());
            client.CheckAvailability(
                (Response<bool> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non 200 response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                "gb");
        }
    }
}
