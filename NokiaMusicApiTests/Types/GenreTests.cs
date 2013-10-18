// -----------------------------------------------------------------------
// <copyright file="GenreTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// Genre tests
    /// </summary>
    [TestFixture]
    public class GenreTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            Genre genre = new Genre() { Id = TestId, Name = TestName };

            Assert.AreEqual(TestId, genre.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, genre.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Genre genre = new Genre() { Id = TestId, Name = TestName };
            Assert.IsNotNull(genre.GetHashCode(), "Expected a hash code");
            Assert.IsFalse(genre.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void HashCodeCanBeRetrievedWhenIdIsNull()
        {
            Genre genre = new Genre();
            Assert.IsNotNull(genre.GetHashCode(), "Expected a hash code");
        }

        [Test]
        public void TestJsonParsing()
        {
            Genre genre = new Genre() { Id = "Metal", Name = "Metal" };
            JObject json = JObject.Parse("{\"id\":\"Metal\",\"name\":\"Metal\"}");
            Genre genreFromJson = Genre.FromJToken(json) as Genre;

            Assert.IsNotNull(genreFromJson, "Expected a genre object");

            Assert.IsTrue(genre.Equals(genreFromJson), "Expected the same genre");
        }
    }
}
