// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandlerTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Commands;
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
        public async Task AttemptToGetJsonFromBadHttpUrlToEnsureWebExceptionCaught()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://baduritesting.co")));

            var command = new ProductCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
        }

        [Test]
        public async Task AttemptToGetJsonFromRealButNonJsonUri()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new ApiUriBuilder());

            var command = new CountryResolverCommand("test", null) { BaseApiUri = "http://music.nokia.com/gb/en/badurl" };

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode.Value, "Expected 404");
        }

        [Test]
        public async Task AttemptToGetStringFromRealUri()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new ApiUriBuilder());

            var command = new MockMusicClientCommand() { BaseApiUri = "http://music.nokia.com/gb/en/badurl" };

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", "us", null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.IsNotNull(result.Result, "Expected a result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode.Value, "Expected 404");
        }

        [Test]
        public async Task AttemptToGetJsonFromRealUriAndClientError()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new ApiUriBuilder());

            var command = new CountryResolverCommand("test", null) { BaseApiUri = MusicClientCommand.DefaultBaseApiUri };

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.IsNotNull(result.Result, "Expected a result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode.Value, "Expected 401");
        }

        [Test]
        public async Task RequestWithValidHeaderIsSuccessful()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new ProductCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                new Dictionary<string, string> { { "Custom", @"test" } },
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
        }

        [Test]
        public async Task AttemptToGetStatusCodeFromRealUri()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new ProductCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
        }

        [Test]
        public async Task AttemptToGetStatusCodeFromRealPost()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new MockApiCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
        }

        [Test]
        public async Task TimeoutResponseGivesNoStatusCode()
        {
            MusicClient.RequestTimeout = 1;
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new ProductCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            // Wait for the response and parsing...
            Assert.AreEqual(1, MusicClient.RequestTimeout, "Expected timeout to return same value that was set");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNull(result.StatusCode, "Expected no status code");
        }

        [Test]
        public async Task TimeoutDuringEndRequestStreamGivesNoStatusCode()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://localhost:8123/")));

            var command = new MockApiCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNull(result.StatusCode, "Expected no status code");
        }

        [Test]
        public async Task NullResponseWithoutMixRadioHeaderRaisesNetworkLimitedException()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new MockApiCommand(null, HttpMethod.Head);

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
            Assert.IsInstanceOf<NetworkLimitedException>(result.Error, "Expected a NetworkLimitedException");
        }

        [Test]
        public async Task AttemptToGetStatusCodeWithCustomUserAgent()
        {
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), "MusicAPI-Tests/1.0.0.0");

            var command = new MockApiCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            // Wait for the response and parsing...
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNotNull(result.StatusCode, "Expected a status code - this test can fail when you run tests with no internet connection!");
        }

        [Test]
        public async Task CancellationDoesCancelApiCall()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            cancellationTokenSource.Cancel();

            var command = new MockApiCommand();

            var result = await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                cancellationTokenSource.Token);

            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.Error, "Expected an error, status code:" + result.StatusCode);
            Assert.IsInstanceOf<ApiCallCancelledException>(result.Error, "Expected an ApiCallCancelledException");
            Assert.IsNull(result.Result, "Expected no result object");
            Assert.IsNull(result.StatusCode, "Expected no status code");
        }

        [Test]
        [TestCase(0, true)]
        [TestCase(-1000, false)]
        public async Task ServerOffsetGotSet(int clientDateOffset, bool expectedResult)
        {
            const int EPSILON = 25; ////25 hours
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new MockApiCommand(null, HttpMethod.Head);

            await handler.SendRequestAsync(
                command,
                new MockMusicClientSettings("test", null, null),
                null,
                command.HandleRawData,
                null,
                null);

            Assert.AreEqual(expectedResult, Math.Abs(handler.ServerTimeUtc.Subtract(DateTime.Now.AddHours(clientDateOffset)).TotalHours) <= EPSILON);
        }

        [Test]
        public async Task ServerTimeOffsetIsSetForMulitpleRequests()
        {
            const int EPSILON = 25; ////25 hours
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")));

            var command = new MockApiCommand(null, HttpMethod.Head);
            await handler.SendRequestAsync(command, new MockMusicClientSettings("test", null, null), null, command.HandleRawData, null, null);
            await handler.SendRequestAsync(command, new MockMusicClientSettings("test", null, null), null, command.HandleRawData, null, null);
            
            Assert.IsTrue(Math.Abs(handler.ServerTimeUtc.Subtract(DateTime.Now).TotalHours) <= EPSILON);
        }

        [Test]
        public async Task ContentIsGzippedIfCommandSpecifies()
        {
            // Arrange
            var mockHttpClientProxy = new MockHttpClientRequestProxy();
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), null, mockHttpClientProxy);
            var rawMessage = "This is the raw body of the message";
            string actualMessage;
            var command = new MockApiCommand(rawMessage, HttpMethod.Head, true);

            // Act
            await handler.SendRequestAsync(command, new MockMusicClientSettings("test", null, null), null, command.HandleRawData, null, null);
            using (Stream decompressedStream = new GZipStream(await mockHttpClientProxy.RequestMessage.Content.ReadAsStreamAsync(), CompressionMode.Decompress, true))
            {
                using (TextReader reader = new StreamReader(decompressedStream, Encoding.UTF8))
                {
                    actualMessage = reader.ReadToEnd();
                }    
            }

            // Assert
            Assert.AreEqual(rawMessage, actualMessage);
            Assert.AreEqual("gzip", mockHttpClientProxy.RequestMessage.Content.Headers.ContentEncoding.First());
        }

        [Test]
        public async Task SslCertFailureResultsInSendFailureException()
        {
            // Arrange
            var mockHttpClientProxy = new MockHttpClientRequestProxy();
            mockHttpClientProxy.SetupException(new WebException("Message", WebExceptionStatus.SendFailure));
            IApiRequestHandler handler = new ApiRequestHandler(new TestHttpUriBuilder(new Uri("http://www.nokia.com")), null, mockHttpClientProxy);
            var rawMessage = "This is the raw body of the message";
            var command = new MockApiCommand(rawMessage, HttpMethod.Head, true);

            // Act
            var response = await handler.SendRequestAsync(command, new MockMusicClientSettings("test", null, null), null, command.HandleRawData, null, null);

            // Assert
            Assert.IsFalse(response.Succeeded);
            Assert.IsTrue(response.Error is SendFailureException);
        }
    }
}
