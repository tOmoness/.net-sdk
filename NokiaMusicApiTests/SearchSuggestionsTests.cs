// -----------------------------------------------------------------------
// <copyright file="SearchSuggestionsTests.cs" company="Nokia">
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
    public class SearchSuggestionsTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSearchSuggestionsThrowsExceptionForNullSearchTerm()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            client.GetSearchSuggestions((ListResponse<string> result) => { }, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSearchSuggestionsThrowsExceptionForNullCallback()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            client.GetArtistSearchSuggestions(null, @"lady gaga");
        }

        [Test]
        public void EnsureGetSearchSuggestionsReturnsValuesForValidSearch()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            client.GetSearchSuggestions(
                (ListResponse<string> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
                },
                "green");
        }

        /// <summary>
        /// The faked GetSearchSuggestions response Returns no results found
        /// </summary>
        [Test]
        public void EnsureGetSearchSuggestionsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions_noresults));
            client.GetSearchSuggestions(
                (ListResponse<string> result) =>
                {
                    Assert.IsNotNull(result, "Expected a result");
                    Assert.IsNotNull(result.StatusCode, "Expected a status code");
                    Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
                    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
                    Assert.IsNotNull(result.Result, "Expected a list of results");
                    Assert.IsNull(result.Error, "Expected no error");
                    Assert.AreEqual(result.Result.Count, 0, "Expected 0 results");
                },
                "muse");
        }

        [Test]
        public async void EnsureAsyncGetArtistSearchSuggestionsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            ListResponse<string> result = await client.GetArtistSearchSuggestions("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }

        [Test]
        public async void EnsureAsyncGetSearchSuggestionsReturnsItems()
        {
            // Only test happy path, as the MusicClient tests cover the unhappy path
            IMusicClientAsync client = new MusicClientAsync("test", "test", "gb", new MockApiRequestHandler(Resources.search_suggestions));
            ListResponse<string> result = await client.GetSearchSuggestions("test");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");
        }
    }
}
