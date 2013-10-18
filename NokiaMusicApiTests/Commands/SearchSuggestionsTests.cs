// -----------------------------------------------------------------------
// <copyright file="SearchSuggestionsTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Properties;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class SearchSuggestionsTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSearchSuggestionsThrowsExceptionForNullSearchTerm()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            client.GetSearchSuggestionsAsync(null).Wait();
        }

        [Test]
        public async Task EnsureGetSearchSuggestionsReturnsValuesForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            ListResponse<string> result = await client.GetSearchSuggestionsAsync("green");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async Task EnsureGetArtistSearchSuggestionsReturnsValuesForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            ListResponse<string> result = await client.GetArtistSearchSuggestionsAsync("green");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async Task EnsureRequestIdComesBackInResponse()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            var requestId = Guid.NewGuid();
            ListResponse<string> result = await client.GetSearchSuggestionsAsync("green", requestId: requestId);

            Assert.IsNotNull(result, "Expected a result");
            Assert.AreEqual(requestId, result.RequestId);
        }

        /// <summary>
        /// The faked GetSearchSuggestions response Returns no results found
        /// </summary>
        /// <returns>An async Task</returns>
        [Test]
        public async Task EnsureGetSearchSuggestionsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.search_suggestions_noresults));
            ListResponse<string> result = await client.GetSearchSuggestionsAsync("muse");
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForSearch()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new SearchSuggestionsCommand().AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/suggestions/", uri.ToString());
        }

        [Test]
        public void EnsureUriIsBuiltCorrectlyForArtistSearch()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            var cmd = new SearchSuggestionsCommand { SuggestArtists = true };
            cmd.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/suggestions/creators/", uri.ToString());
        }
    }
}
