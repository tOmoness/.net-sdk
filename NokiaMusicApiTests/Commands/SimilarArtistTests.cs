// -----------------------------------------------------------------------
// <copyright file="SimilarArtistTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class SimilarArtistTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsThrowsExceptionForNullArtistId()
        {
            string nullId = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtistsAsync(nullId).Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsThrowsExceptionForNullArtist()
        {
            Artist nullArtist = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtistsAsync(nullArtist).Wait();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGetSimilarArtistsAsyncThrowsExceptionForNullArtist()
        {
            Artist nullArtist = null;
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            client.GetSimilarArtistsAsync(nullArtist).Wait();
        }

        [Test]
        public async Task EnsureGetSimilarArtistsReturnsItems()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(Resources.artist_similar));
            ListResponse<Artist> result = await client.GetSimilarArtistsAsync(new Artist() { Id = "test" });
            Assert.IsNotNull(result, "Expected a result");
            Assert.IsNotNull(result.StatusCode, "Expected a status code");
            Assert.IsTrue(result.StatusCode.HasValue, "Expected a status code");
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode.Value, "Expected a 200 response");
            Assert.IsNotNull(result.Result, "Expected a list of results");
            Assert.IsNull(result.Error, "Expected no error");
            Assert.Greater(result.Result.Count, 0, "Expected more than 0 results");

            foreach (Artist artist in result.Result)
            {
                Assert.IsFalse(string.IsNullOrEmpty(artist.Id), "Expected Id to be populated");
                Assert.IsFalse(string.IsNullOrEmpty(artist.Name), "Expected Name to be populated");
                Assert.IsNotNull(artist.Genres, "Expected a genre list");
                Assert.Greater(artist.Genres.Length, 0, "Expected more than 0 genres");
            }
        }

        [Test]
        public async Task EnsureGetSimilarArtistsReturnsErrorForFailedCall()
        {
            IMusicClient client = new MusicClient("test", "gb", new MockApiRequestHandler(FakeResponse.NotFound()));
            ListResponse<Artist> result = await client.GetSimilarArtistsAsync("test");
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
            new SimilarArtistsCommand { ArtistId = "348877" }.AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/creators/348877/similar/", uri.ToString());
        }
    }
}
