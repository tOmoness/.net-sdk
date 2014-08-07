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
        private const string TestMixId = "18523926";
        private const string TestArtistName = "Muse";

        [Test]
        public void TestArtistNamePropertyPersists()
        {
            PlayMixTask task = new PlayMixTask() { ArtistName = TestArtistName };
            Assert.AreEqual(TestArtistName, task.ArtistName, "Expected the same name");
        }

        [Test]
        public void TestMixIdPropertyPersists()
        {
            PlayMixTask task = new PlayMixTask() { MixId = TestMixId };
            Assert.AreEqual(TestMixId, task.MixId, "Expected the same ID");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestMixIdPropertyIsRequiredForShow()
        {
            PlayMixTask task = new PlayMixTask();
            await task.Show();
        }

        [Test]
        public async Task TestPlayMixGoesAheadWhenItCan()
        {
            PlayMixTask task = new PlayMixTask() { MixId = TestMixId };
            await task.Show();            
            Assert.Pass();
        }
    }
}
