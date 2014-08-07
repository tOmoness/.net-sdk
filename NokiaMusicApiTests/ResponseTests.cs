// -----------------------------------------------------------------------
// <copyright file="ResponseTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Commands;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public void ValidateASuccessfulResponse()
        {
            const string SuccessfulResponse = "joy";
            Guid requestId = Guid.NewGuid();
            Response<string> response = new Response<string>(HttpStatusCode.OK, SuccessfulResponse, requestId);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected the same status code");
            Assert.AreEqual(SuccessfulResponse, response.Result, "Expected the same result");
            Assert.AreEqual(requestId, response.RequestId, "Expected the same request Id");
            Assert.IsNull(response.Error, "Expected no error");
            Assert.IsTrue(response.Succeeded, "Expected success");
            Assert.IsFalse(response.RequestTimedOut, "Expected no timeout");
        }

        [Test]
        public void ValidateASuccessfulResponseWithPaging()
        {
            const string SuccessfulResponse = "joy";
            Response<string> response = new Response<string>(HttpStatusCode.OK, SuccessfulResponse, Guid.Empty);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected the same status code");
            Assert.AreEqual(SuccessfulResponse, response.Result, "Expected the same result");
            Assert.AreEqual(Guid.Empty, response.RequestId, "Expected the same request id");
            Assert.IsNull(response.Error, "Expected no error");
            Assert.IsTrue(response.Succeeded, "Expected success");
        }

        [Test]
        public void ValidateAFailedResponse()
        {
            Exception e = new ApiCredentialsRequiredException();
            Guid requestId = Guid.NewGuid();
            Response<string> response = new Response<string>(null, e, "ThisIsTheResponseBody", requestId);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual(response.ErrorResponseBody, "ThisIsTheResponseBody");
            Assert.AreEqual(e, response.Error, "Expected the same error");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.AreEqual(requestId, response.RequestId, "Expected the same request id");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateASuccessfulListResponse()
        {
            const int ItemsPerPage = 10;
            const int TotalResults = 23;
            const int StartIndex = 11;
            Guid requestId = Guid.NewGuid();
            List<string> list = new List<string>() { "1", "2" };
            ListResponse<string> response = new ListResponse<string>(HttpStatusCode.OK, list, StartIndex, ItemsPerPage, TotalResults, requestId);
            Assert.IsNotNull(response, "Expected a new ListResponse");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected the same status code");
            Assert.AreEqual(list.Count, response.Result.Count, "Expected the same result");
            Assert.AreEqual(ItemsPerPage, response.ItemsPerPage, "Expected the same ItemsPerPage");
            Assert.AreEqual(TotalResults, response.TotalResults, "Expected the same TotalResults");
            Assert.AreEqual(StartIndex, response.StartIndex, "Expected the same StartIndex");
            Assert.AreEqual(requestId, response.RequestId, "Expected the same request id");
            Assert.IsNull(response.Error, "Expected no error");
            Assert.IsTrue(response.Succeeded, "Expected success");
        }

        [Test]
        public void ValidateAFailedListResponse()
        {
            Exception e = new ApiCredentialsRequiredException();
            Guid requestId = Guid.NewGuid();
            ListResponse<MusicItem> response = new ListResponse<MusicItem>(HttpStatusCode.OK, e, "ErrorResponseBody", requestId);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.AreEqual(e, response.Error, "Expected the same error");
            Assert.AreEqual("ErrorResponseBody", response.ErrorResponseBody, "Expected the same error");
            Assert.AreEqual(requestId, response.RequestId, "Expected the same request id");
            Assert.IsNull(response.Result, "Expected no result");
            Assert.IsFalse(response.Succeeded, "Expected failure");
        }

        [Test]
        public void EnsureNullResultGivesNoEnumerator()
        {
            ListResponse<MusicItem> response = new ListResponse<MusicItem>(HttpStatusCode.OK, new ApiCredentialsRequiredException(), null, Guid.Empty);
            Assert.IsNull((response as System.Collections.IEnumerable).GetEnumerator(), "Expected a null enumerator");
            Assert.IsNull((response as System.Collections.Generic.IEnumerable<MusicItem>).GetEnumerator(), "Expected a null enumerator");
        }

        [Test]
        public void EnsureValidResultGivesEnumerator()
        {
            ListResponse<MusicItem> response = new ListResponse<MusicItem>(HttpStatusCode.OK, new List<MusicItem>(), null, null, null, Guid.Empty);
            Assert.IsNotNull((response as System.Collections.IEnumerable).GetEnumerator(), "Expected a non-null enumerator");
            Assert.IsNotNull((response as System.Collections.Generic.IEnumerable<MusicItem>).GetEnumerator(), "Expected a non-null enumerator");
        }

        [Test]
        public void EnsureResponsePropertiesEmptyByDefault()
        {
            Response r = new Response();
            Assert.IsNull(r.Error, "Expected nothing");
            Assert.IsNull(r.ErrorResponseBody, "Expected nothing");
            Assert.AreEqual(r.RequestId, Guid.Empty, "Expected Empty");
            Assert.IsNull(r.StatusCode, "Expected nothing");
            Assert.IsTrue(r.Succeeded, "No error means success");
        }

        [Test]
        public void EnsureResponsePropertiesPersist()
        {
            Response r = new Response(HttpStatusCode.OK, new Exception(), "body", new Guid());
            Assert.IsNotNull(r.Error, "Expected something");
            Assert.IsNotNull(r.ErrorResponseBody, "Expected something");
            Assert.IsNotNull(r.RequestId, "Expected something");
            Assert.IsNotNull(r.StatusCode, "Expected something");
            Assert.AreEqual(r.StatusCode.Value, HttpStatusCode.OK, "Expected OK");
            Assert.IsFalse(r.Succeeded, "Error means failure");
        }

        [Test]
        public void ValidateToErrorResponseWithGenericException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new Exception(), "ThisIsTheResponseBody", new Guid());
            Response response = command.ErrorResponseHandler(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual(response.ErrorResponseBody, "ThisIsTheResponseBody");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NokiaMusicException>(response.Error, "Expected a NokiaMusicException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
        }

        [Test]
        public void ValidateToErrorResponseOfWithGenericException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new Exception(), "ThisIsTheResponseBody", new Guid());
            Response<object> response = command.ItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual(response.ErrorResponseBody, "ThisIsTheResponseBody");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NokiaMusicException>(response.Error, "Expected a NokiaMusicException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToListErrorResponseOfWithGenericException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new Exception(), "ThisIsTheResponseBody", new Guid());
            ListResponse<object> response = command.ListItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual(response.ErrorResponseBody, "ThisIsTheResponseBody");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NokiaMusicException>(response.Error, "Expected a NokiaMusicException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToErrorResponseWithNokiaMusicException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new NokiaMusicException(null), "ThisIsTheResponseBody", new Guid());
            Response response = command.ErrorResponseHandler(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual("ThisIsTheResponseBody", response.ErrorResponseBody, "Expected the same error response body");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.AreSame(r.Error, response.Error, "Expected the same exception");
            Assert.IsFalse(response.Succeeded, "Expected failure");
        }

        [Test]
        public void ValidateToErrorResponseOfWithNokiaMusicException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new NokiaMusicException(null), "ThisIsTheResponseBody", new Guid());
            Response<object> response = command.ItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual("ThisIsTheResponseBody", response.ErrorResponseBody, "Expected the same error response body");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.AreSame(r.Error, response.Error, "Expected the same exception");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToListErrorResponseOfWithNokiaMusicException()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(null, new NokiaMusicException(null), "ThisIsTheResponseBody", new Guid());
            ListResponse<object> response = command.ListItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.StatusCode, "Expected no status code");
            Assert.AreEqual("ThisIsTheResponseBody", response.ErrorResponseBody, "Expected the same error response body");
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.AreSame(r.Error, response.Error, "Expected the same exception");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToErrorResponseWithMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid());
            Response response = command.ErrorResponseHandler(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<ApiCallFailedException>(response.Error, "Expected a ApiCallFailedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
        }

        [Test]
        public void ValidateToErrorResponseOfWithMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid());
            Response<object> response = command.ItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<ApiCallFailedException>(response.Error, "Expected a ApiCallFailedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToListErrorResponseOfWithMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid());
            ListResponse<object> response = command.ListItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<ApiCallFailedException>(response.Error, "Expected a ApiCallFailedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToErrorResponseWithMissingMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid(), false);
            Response response = command.ErrorResponseHandler(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NetworkLimitedException>(response.Error, "Expected a NetworkLimitedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
        }

        [Test]
        public void ValidateToErrorResponseOfWithMissingMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid(), false);
            Response<object> response = command.ItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NetworkLimitedException>(response.Error, "Expected a NetworkLimitedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }

        [Test]
        public void ValidateToListErrorResponseOfWithMissingMixRadioHeader()
        {
            var command = new MockMusicClientCommand();
            Response r = new Response(HttpStatusCode.OK, new Guid(), false);
            ListResponse<object> response = command.ListItemErrorResponseHandler<object>(r);
            Assert.IsNotNull(response, "Expected a new Response");
            Assert.IsNull(response.ErrorResponseBody);
            Assert.IsNotNull(response.Error, "Expected an exception");
            Assert.IsInstanceOf<NetworkLimitedException>(response.Error, "Expected a NetworkLimitedException");
            Assert.IsFalse(response.Succeeded, "Expected failure");
            Assert.IsNull(response.Result, "Expected no result");
        }
    }
}
