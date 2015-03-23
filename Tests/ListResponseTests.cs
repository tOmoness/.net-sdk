// -----------------------------------------------------------------------
// <copyright file="ListResponseTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using MixRadio;
using MixRadio.Types;
using NUnit.Framework;

namespace MixRadio.Tests
{
    [TestFixture]
    public class ListResponseTests
    {
        [Test]
        public void EnsureIListPropertiesWorkForSuccessCase()
        {
            List<Artist> artists = new List<Artist>()
            {
                new Artist() { Id = "1234", Name = "Artist" },
                new Artist() { Id = "12345", Name = "Artist" },
                new Artist() { Id = "123456", Name = "Artist" }
            };

            ListResponse<Artist> response = new ListResponse<Artist>(HttpStatusCode.OK, artists, 0, 10, 3, Guid.Empty);

            Assert.AreEqual(3, response.Count, "Expected a list");
            Assert.AreEqual(false, response.IsReadOnly, "Expected a readonly list");
            Assert.AreEqual(false, response.IsFixedSize, "Expected a non fixed size list");
            Assert.AreEqual(false, response.IsSynchronized, "Expected a non sync list");
            Assert.IsNotNull(response.SyncRoot, "Expected an object");
            Assert.AreEqual(artists[0], response[0], "Expected the same item");
            Assert.AreEqual(artists[0], (response as IList)[0], "Expected the same item");
            Assert.AreEqual(1, response.IndexOf(artists[1]), "Expected the same item");
            Assert.AreEqual(1, (response as IList).IndexOf(artists[1]), "Expected the same item");
            Assert.IsTrue(response.Contains(artists[1]), "Expected the item in there");
            Assert.IsTrue((response as IList).Contains(artists[1]), "Expected the item in there");

            // Check adding cases...
            response[2] = new Artist();
            Assert.AreEqual(3, response.Count, "Expected same items");
            (response as IList)[1] = new Artist();
            Assert.AreEqual(3, response.Count, "Expected same items");
            response.Add(new Artist());
            Assert.AreEqual(4, response.Count, "Expected more items");
            (response as IList).Add(new Artist());
            Assert.AreEqual(5, response.Count, "Expected more items");
            response.Insert(1, new Artist());
            Assert.AreEqual(6, response.Count, "Expected more items");
            (response as IList).Insert(1, new Artist());
            Assert.AreEqual(7, response.Count, "Expected more items");

            // Check CopyTo...
            Artist[] newArtists = new Artist[response.Count];
            response.CopyTo(newArtists, 0);
            Assert.AreEqual(response.Count, newArtists.Length, "Expected the same number");
            (response as IList).CopyTo(newArtists, 0);
            Assert.AreEqual(response.Count, newArtists.Length, "Expected the same number");

            // Check remove cases...
            response.Remove(artists[0]);
            Assert.AreEqual(6, response.Count, "Expected less items");
            (response as IList).Remove(artists[0]);
            Assert.AreEqual(5, response.Count, "Expected less items");
            response.RemoveAt(0);
            Assert.AreEqual(4, response.Count, "Expected less items");
            response.Clear();
            Assert.AreEqual(0, response.Count, "Expected an empty list");
        }

        [Test]
        public void EnsureIListPropertiesWorkForErrorCase()
        {
            ListResponse<Artist> response = new ListResponse<Artist>(HttpStatusCode.NotFound, new ApiCallFailedException(), null, Guid.Empty);

            Artist artist = new Artist() { Id = "1234", Name = "Artist" };

            Assert.AreEqual(0, response.Count, "Expected an empty list");
            Assert.AreEqual(false, response.IsReadOnly, "Expected a readonly list");
            Assert.IsNull(response[0], "Expected an empty list");
            Assert.AreEqual(-1, response.IndexOf(artist), "Expected an empty list");
            Assert.IsFalse(response.Contains(artist), "Expected an empty list");

            // Check adding cases...
            response[0] = artist;
            Assert.AreEqual(0, response.Count, "Expected an empty list");
            response.Add(artist);
            Assert.AreEqual(0, response.Count, "Expected an empty list");
            response.Insert(0, artist);
            Assert.AreEqual(0, response.Count, "Expected an empty list");

            // Check CopyTo does nothing...
            Artist[] artists = new Artist[0];
            response.CopyTo(artists, 0);
            Assert.AreEqual(0, artists.Length, "Expected an empty list");

            // Check remove cases...
            response.Remove(artist);
            Assert.AreEqual(0, response.Count, "Expected an empty list");
            response.RemoveAt(0);
            Assert.AreEqual(0, response.Count, "Expected an empty list");
            response.Clear();
            Assert.AreEqual(0, response.Count, "Expected an empty list");
        }
    }
}
