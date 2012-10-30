// -----------------------------------------------------------------------
// <copyright file="PlayMixTaskTests.cs" company="NOKIA">
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
        public void TestMixIdPropertyIsRequiredForShow()
        {
            PlayMixTask task = new PlayMixTask();
            task.Show();
        }

        [Test]
        public void TestPlayMixGoesAheadWhenItCan()
        {
            PlayMixTask task = new PlayMixTask() { MixId = TestMixId };
            task.Show();            
            Assert.Pass();
        }
    }
}
