// -----------------------------------------------------------------------
// <copyright file="PlayMixTaskTests.cs" company="NOKIA">
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
    /// PlayMixTask Tests
    /// </summary>
    [TestFixture]
    public class PlayMixTaskTests
    {
        private const string TestId = "18523926";
        private const string TestArtistName = "Muse";

        [Test]
        public void TestArtistIdPropertyPersists()
        {
            PlayMixTask task = new PlayMixTask() { ArtistId = TestId };
            Assert.AreEqual(TestId, task.ArtistId, "Expected the same id");
        }

        [Test]
        public void TestArtistNamePropertyPersists()
        {
            PlayMixTask task = new PlayMixTask() { ArtistName = TestArtistName };
            Assert.AreEqual(TestArtistName, task.ArtistName, "Expected the same name");
        }

        [Test]
        public void TestMixIdPropertyPersists()
        {
            PlayMixTask task = new PlayMixTask() { MixId = TestId };
            Assert.AreEqual(TestId, task.MixId, "Expected the same ID");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestMixIdPropertyIsRequiredForShow()
        {
            PlayMixTask task = new PlayMixTask();
            await task.Show();
        }

        [Test]
        public async Task TestPlayMixGoesAheadWithMixId()
        {
            PlayMixTask task = new PlayMixTask() { MixId = TestId };
            await task.Show();            
            Assert.Pass();
        }

        [Test]
        public async Task TestPlayMixGoesAheadWithArtistId()
        {
            PlayMixTask task = new PlayMixTask() { ArtistId = TestId };
            await task.Show();
            Assert.Pass();
        }

        [Test]
        public async Task TestPlayMixGoesAheadWithArtistName()
        {
            PlayMixTask task = new PlayMixTask() { ArtistName = TestArtistName };
            await task.Show();
            Assert.Pass();
        }
    }
}
