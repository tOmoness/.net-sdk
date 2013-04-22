// -----------------------------------------------------------------------
// <copyright file="CountryResolverTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests
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
        public void EnsureInvalidApiCredentialsExceptionThrownWhenServerGives403()
        {
            CountryResolver client = new CountryResolver("badkey", new MockApiRequestHandler(FakeResponse.Forbidden()));
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
