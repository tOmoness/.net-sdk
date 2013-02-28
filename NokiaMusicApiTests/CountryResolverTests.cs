// -----------------------------------------------------------------------
// <copyright file="CountryResolverTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Request;
using Nokia.Music.Phone.Tests.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class CountryResolverTests
    {
        [Test]
        public void CheckApiCredentialsValidated()
        {
            string nullKey = null;
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new CountryResolver(nullKey, nullKey); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { new CountryResolver("test-app-id", nullKey); }));
        }

        [Test]
        public void EnsureDefaultRequestHandlerIsCreated()
        {
            CountryResolver client = new CountryResolver("test", "test");
            Assert.AreEqual(client.RequestHandler.GetType(), typeof(ApiRequestHandler), "Expected the default handler");
        }

        [Test]
        public void EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403()
        {
            CountryResolver client = new CountryResolver("badkey", "test", new MockApiRequestHandler(FakeResponse.Forbidden()));
            client.CheckAvailability(
                (Response<bool> response) =>
                {
                    Assert.IsNotNull(response.Error, "Expected an Error");
                    Assert.AreEqual(typeof(InvalidApiCredentialsException), response.Error.GetType(), "Expected an InvalidApiCredentialsException");
                },
                "gb");
        }
    }
}
