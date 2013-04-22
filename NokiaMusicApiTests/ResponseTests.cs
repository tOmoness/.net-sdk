// -----------------------------------------------------------------------
// <copyright file="ResponseTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net;
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
    }
}
