// -----------------------------------------------------------------------
// <copyright file="ShowArtistTaskTests.cs" company="NOKIA">
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
    /// ShowArtistTask Tests
    /// </summary>
    [TestFixture]
    public class ShowArtistTaskTests
    {
        private const string TestArtistId = "372444";
        private const string TestArtistName = "Test";

        [Test]
        public void TestArtistIdPropertyPersists()
        {
            ShowArtistTask task = new ShowArtistTask() { ArtistId = TestArtistId };
            Assert.AreEqual(TestArtistId, task.ArtistId, "Expected the same ID");
        }

        [Test]
        public void TestArtistNamePropertyPersists()
        {
            ShowArtistTask task = new ShowArtistTask() { ArtistName = TestArtistName };
            Assert.AreEqual(TestArtistName, task.ArtistName, "Expected the same Name");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestArtistIdOrNamePropertyIsRequiredForShow()
        {
            ShowArtistTask task = new ShowArtistTask();
            task.Show();
        }

        [Test]
        public void TestShowArtistGoesAheadWhenItCan()
        {
            ShowArtistTask task1 = new ShowArtistTask() { ArtistId = TestArtistId };
            task1.Show();

            ShowArtistTask task2 = new ShowArtistTask() { ArtistName = TestArtistName };
            task2.Show();
            
            Assert.Pass();
        }

        [Test]
        public void TestPlayArtistMixGoesAheadWhenItCan()
        {
            PlayMixTask task2 = new PlayMixTask() { ArtistName = TestArtistName };
            task2.Show();

            Assert.Pass();
        }
    }
}
