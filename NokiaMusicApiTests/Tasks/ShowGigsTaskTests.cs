// -----------------------------------------------------------------------
// <copyright file="ShowGigsTaskTests.cs" company="NOKIA">
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
    /// ShowGigsTask Tests
    /// </summary>
    [TestFixture]
    public class ShowGigsTaskTests
    {
        private const string TestTerm = "Test";

        [Test]
        public void TestSearchTermPropertyPersists()
        {
            ShowGigsTask task = new ShowGigsTask() { SearchTerms = TestTerm };
            Assert.AreEqual(TestTerm, task.SearchTerms, "Expected the same term");
        }

        [Test]
        public void TestShowArtistGoesAheadWhenItCan()
        {
            ShowGigsTask task1 = new ShowGigsTask() { SearchTerms = TestTerm };
            task1.Show();

            ShowGigsTask task2 = new ShowGigsTask();
            task2.Show();
            
            Assert.Pass();
        }
    }
}
