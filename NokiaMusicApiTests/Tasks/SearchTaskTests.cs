// -----------------------------------------------------------------------
// <copyright file="SearchTaskTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Nokia.Music.Internal;
using Nokia.Music.Tasks;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// MusicSearchTask Tests
    /// </summary>
    [TestFixture]
    public class SearchTaskTests
    {
        private const string TestTerm = "Test";

        [Test]
        public void TestSearchTermPropertyPersists()
        {
            MusicSearchTask task = new MusicSearchTask() { SearchTerms = TestTerm };
            Assert.AreEqual(TestTerm, task.SearchTerms, "Expected the same term");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSearchTermPropertyIsRequired()
        {
            MusicSearchTask task = new MusicSearchTask();
            task.Show();
        }

        [Test]
        public void TestSearchGoesAheadWhenItCan()
        {
            MusicSearchTask task = new MusicSearchTask() { SearchTerms = TestTerm };
            task.Show();
        }
    }
}
