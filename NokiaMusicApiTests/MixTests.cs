// -----------------------------------------------------------------------
// <copyright file="MixTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tests.Internal;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class MixTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetMixGroupsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixgroups));
            client.GetMixGroups(null);
        }

        [Test]
        public void EnsureGetMixGroupsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixgroups));
            client.GetMixGroups(
                (ListResponse<MixGroup> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

                    foreach (MixGroup mixGroup in result.Result)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(mixGroup.Id), "Expected Id to be populated");
                        Assert.IsFalse(string.IsNullOrEmpty(mixGroup.Name), "Expected Name to be populated");
                    }
                });
        }

        [Test]
        public void EnsureGetMixGroupsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetMixGroups(
                (ListResponse<MixGroup> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetMixesThrowsExceptionForNullGroupId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixes));
            client.GetMixes((ListResponse<Mix> result) => { }, nullId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetMixesThrowsExceptionForNullGroup()
        {
            MixGroup nullGroup = null;
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixes));
            client.GetMixes((ListResponse<Mix> result) => { }, nullGroup);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetMixesThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixes));
            client.GetMixes(null, "test");
        }

        [Test]
        public void EnsureGetMixesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.mixes));
            client.GetMixes(
                (ListResponse<Mix> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                },
                new MixGroup() { Id = "test" });
        }

        [Test]
        public void EnsureGetMixesReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            client.GetMixes(
                (ListResponse<Mix> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
                    Assert.IsNotNull(result.Error, "Expected an error");
                    Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
                },
                "test");
        }

        [Test]
        public async void EnsureAsyncGetMixGroupsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.mixgroups));

            ListResponse<MixGroup> result = await client.GetMixGroups();
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async void EnsureAsyncGetMixesReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.mixes));

            ListResponse<Mix> result = await client.GetMixes("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            result = await client.GetMixes(new MixGroup() { Id = "test" });
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
