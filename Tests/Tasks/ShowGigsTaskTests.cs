// -----------------------------------------------------------------------
// <copyright file="ShowGigsTaskTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Nokia.Music.Internal;
using Nokia.Music.Tasks;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
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
        public async Task TestShowArtistGoesAheadWhenItCan()
        {
            ShowGigsTask task1 = new ShowGigsTask() { SearchTerms = TestTerm };
            await task1.Show();

            ShowGigsTask task2 = new ShowGigsTask();
            await task2.Show();
            
            Assert.Pass();
        }
    }
}
