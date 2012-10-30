// -----------------------------------------------------------------------
// <copyright file="SearchTaskTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tasks;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
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
