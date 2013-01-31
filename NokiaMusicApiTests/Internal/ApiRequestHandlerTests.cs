// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandlerTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Internal;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    /// <summary>
    /// Test Request Handler with local files 
    /// </summary>
    [TestFixture]
    public class ApiRequestHandlerTests
    {
        [SetUp]
        public void ResetDefaults()
        {
            MusicClient.RequestTimeout = 60000;
            MusicClient.GzipEnabled = true;
        }

        [Test]
        public void EnsureCallbackParameterChecked()
        {
            // Check ApiMethod param...
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiRequestHandler(new ApiUriBuilder()).SendRequestAsync(null, null, null, null, null);
                }));
        }

        [Test]
        public void GetJsonFromLocalFileToTestSuccessfulRequestAndJsonParsing()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new LocalFileUriBuilder("country.json"));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"), 
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.IsNotNull(result.Result, "Expected a result object");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(1000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void GetJsonFromLocalFileToTestSuccessfulRequestAndBadJsonHandling()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new LocalFileUriBuilder("bad-result.json"));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(1000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetJsonFromBadHttpUrlToEnsureWebExceptionCaught()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://baduritesting.co")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetJsonFromRealApiWithBadKey()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new ApiUriBuilder());
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode.Value, "Expected a 401");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void RequestWithValidHeaderIsSuccessful()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                },
                new Dictionary<string, string> { { "Custom", @"test" } });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RequestWithInvalidHeaderCausesException()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                },
                new Dictionary<string, string> { { "Referer", @"test" } }); // this should cause the request to be rejected

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetStatusCodeFromRealUri()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetStatusCodeFromRealPost()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new MockApiMethod(),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void TimeoutResponseGivesNoStatusCode()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            MusicClient.RequestTimeout = 0;
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNull(result.StatusCode, "Expected no status code");
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.AreEqual(0, MusicClient.RequestTimeout, "Expected timeout to return same value that was set");
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void UseGzipOnClientSetsGzipHeadersCorrectly()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);
            Response<JObject> responseResult;
            MusicClient.GzipEnabled = true;
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));
            handler.SendRequestAsync(
                new CountryResolver("test", "test"),
                new MockMusicClientSettings("test", "test", null),
                null,
                null,
                (Response<JObject> result) =>
                {
                    responseResult = result;
                    gotResult = true;
                    waiter.Set();
                });

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.AreEqual(true, MusicClient.GzipEnabled, "Expected GzipEnabled to return same value that was set");
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void EnsureNullReturnForNullStreamInStreamExtensions()
        {
            Assert.IsNull(StreamExtensions.AsString(null), "Expected a null return");
        }
    }
}
