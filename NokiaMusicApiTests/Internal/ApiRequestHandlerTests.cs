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
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Compression;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;
using Nokia.Music.Tests.Internal;
using NUnit.Framework;

namespace Nokia.Music.Tests
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
            MusicClient.RequestTimeout = 30000;
        }

        [Test]
        public void EnsureCallbackParameterChecked()
        {
            // Check ApiMethod param...
            Assert.Throws(
                typeof(ArgumentNullException),
                () => new ApiRequestHandler(new ApiUriBuilder(), new GzipHandlerWp()).SendRequestAsync<JObject>(null, null, null, null, null));
        }

        [Test]
        public void GetJsonFromLocalFileToTestSuccessfulRequestAndJsonParsing()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new LocalFileUriBuilder("country.json"), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.IsNotNull(result.Result, "Expected a result object");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(1000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void GetJsonFromLocalFileToTestSuccessfulRequestAndBadJsonHandling()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new LocalFileUriBuilder("bad-result.json"), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(1000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetJsonFromBadHttpUrlToEnsureWebExceptionCaught()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://baduritesting.co")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetJsonFromRealButBadUri()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new ApiUriBuilder(), new GzipHandlerWp());
            handler.SendRequestAsync(
                new CountryResolverCommand("test", null) { BaseApiUri = "http://music.nokia.com/gb/en/badurl" },
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode.Value, "Expected 404");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void RequestWithValidHeaderIsSuccessful()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                }),
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

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                }),
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

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void AttemptToGetStatusCodeFromRealPost()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new MockApiCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
                    gotResult = true;
                    waiter.Set();
                }));

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
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new ProductCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNull(result.StatusCode, "Expected no status code");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.AreEqual(0, MusicClient.RequestTimeout, "Expected timeout to return same value that was set");
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void TimeoutDuringEndRequestStreamGivesNoStatusCode()
        {
            bool gotResult = false;
            ManualResetEvent waiter = new ManualResetEvent(false);

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://localhost:8123/")), new GzipHandlerWp());
            handler.SendRequestAsync(
                new MockApiCommand(),
                new MockMusicClientSettings("test", null),
                null,
                new JsonResponseCallback((Response<JObject> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
                    Assert.IsNull(result.Result, "Expected no result object");
                    Assert.IsNull(result.StatusCode, "Expected no status code");
                    gotResult = true;
                    waiter.Set();
                }));

            // Wait for the response and parsing...
            waiter.WaitOne(5000);
            Assert.IsTrue(gotResult, "Expected a result flag");
        }

        [Test]
        public void EnsureNullReturnForNullStreamInStreamExtensions()
        {
            Assert.IsNull(StreamExtensions.AsString(null), "Expected a null return");
        }
    }
}
