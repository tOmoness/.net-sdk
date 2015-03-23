// -----------------------------------------------------------------------
// <copyright file="MixTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MixRadio;
using MixRadio.Commands;
using MixRadio.Tests.Internal;
using MixRadio.Tests.Properties;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests.Commands
{
    [TestFixture]
    public class MixTests
    {
        [Test]
        public async Task EnsureGetMixGroupsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.mixgroups));
            ListResponse<MixGroup> result = await client.GetMixGroupsAsync();
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
        }

        [Test]
        public async Task EnsureGetMixGroupsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<MixGroup> result = await client.GetMixGroupsAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetMixesThrowsExceptionForNullGroupId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.mixes));
            await client.GetMixesAsync(nullId);
        }

        [Test]
        public async Task EnsureGetMixesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.mixes));
            ListResponse<Mix> result = await client.GetMixesAsync("test");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async Task EnsureGetAllMixesReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.allmixes));
            ListResponse<Mix> result = await client.GetAllMixesAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async Task EnsureGetMixesReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Mix> result = await client.GetMixesAsync("test");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        public void EnsureExclusiveMixesRequestsSendExclusiveTag()
        {
            var exclusiveTag1 = "thisIsTheFirstExclusiveTag";
            var exclusiveTag2 = "thisIsTheSecondExclusiveTag";
            var handler = new MockApiRequestHandler(Resources.mixes);

            IMusicClient client = new MusicClient("test", "gb", handler);
            var task = client.GetMixesAsync("test", exclusiveTag1);
            Assert.Greater(task.Result.Result.Count, 0, "Expected more than 0 results");
            Assert.IsTrue(handler.LastQueryString.Contains(new KeyValuePair<string, string>(MusicClientCommand.ParamExclusive, exclusiveTag1)));

            task = client.GetMixesAsync("testId", exclusiveTag2);
            Assert.Greater(task.Result.Result.Count, 0, "Expected more than 0 results");
            Assert.IsTrue(handler.LastQueryString.Contains(new KeyValuePair<string, string>(MusicClientCommand.ParamExclusive, exclusiveTag2)));
        }

        [Test]
        public void EnsureExclusiveMixGroupRequestsSendExclusiveTag()
        {
            var exclusiveTag = "thisIsTheExclusiveTag";
            var handler = new MockApiRequestHandler(Resources.mixgroups);

            IMusicClient client = new MusicClient("test", "gb", handler);
            var groupsTask = client.GetMixGroupsAsync(exclusiveTag);
            Assert.Greater(groupsTask.Result.Result.Count, 0, "Expected more than 0 results");
            Assert.IsTrue(handler.LastQueryString.Contains(new KeyValuePair<string, string>(MusicClientCommand.ParamExclusive, exclusiveTag)));
        }

        [Test]
        public void EnsureUriIsBuiltCorrectly()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new MixesCommand { MixGroupId = "test123" }.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/mixes/groups/test123/", uri.ToString());
        }

        [Test]
        public void EnsureQueryStringIsBuiltCorrectlyWithOptionalFeaturedArtistsParameter()
        {
            var handler = new MockApiRequestHandler(Resources.mixes);

            IMusicClient client = new MusicClient("test", "gb", handler);
            var task = client.GetMixesAsync("test", true);
            Assert.Greater(task.Result.Result.Count, 0, "Expected more than 0 results");
            Assert.IsTrue(handler.LastQueryString.Contains(new KeyValuePair<string, string>("view", "topftartists")));
        }

        [Test]
        public async Task EnsureGetMixReturnsDetails()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.mix));
            Mix result = await client.GetMixAsync("35953777");
            Assert.IsNotNull(result, "Expected a result");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnsureGetMixThrowsForNullId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            Mix result = await client.GetMixAsync(null);
            Assert.IsNotNull(result, "Expected a result");
        }

        [Test]
        [ExpectedException(typeof(ApiCallFailedException))]
        public async Task EnsureGetMixThrowsForBadId()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            Mix result = await client.GetMixAsync("badid");
            Assert.IsNotNull(result, "Expected a result");
        }

        [Test]
        public void CoverExclusivityParams()
        {
            var cmd1 = new MixGroupsCommand();
            cmd1.Exclusivity = new string[] { "exclusivity" };
            cmd1.BuildQueryStringParams();

            var cmd2 = new MixesCommand();
            cmd2.Exclusivity = new string[] { "exclusivity" };
            cmd2.BuildQueryStringParams();
        }
    }
}
