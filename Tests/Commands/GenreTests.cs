// -----------------------------------------------------------------------
// <copyright file="GenreTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
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
    public class GenreTests
    {
        [Test]
        public async Task EnsureGetGenresReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.genres));
            var result = await client.GetGenresAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (Genre genre in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(genre.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(genre.Name), "Expected Name to be populated");
            }
        }

        [Test]
        public async Task EnsureGetGenresReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.InternalServerError()));
            var result = await client.GetGenresAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        public async Task EnsureGetGenresReturnsAnUnhandledErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.ConflictServerError("{ \"error\":\"true\"}")));
            var result = await client.GetGenresAsync();
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a non-OK response");
            Assert.IsNotNull(result.Error, "Expected an error");
            Assert.AreEqual(typeof(ApiCallFailedException), result.Error.GetType(), "Expected an ApiCallFailedException");
        }

        [Test]
        public void EnsureUriIsBuiltCorrectly()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new GenresCommand().AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/genres/", uri.ToString());
        }
    }
}
