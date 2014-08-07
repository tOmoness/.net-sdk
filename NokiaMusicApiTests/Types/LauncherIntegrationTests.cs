// -----------------------------------------------------------------------
// <copyright file="LauncherIntegrationTests.cs" company="NOKIA">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// Artist tests
    /// </summary>
    [TestFixture]
    public class LauncherIntegrationTests
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestAristNamePropertyIsRequiredForPlayMix()
        {
            Artist artist = new Artist();
            await artist.PlayMix();
        }

        [Test]
        public async Task TestPlayArtistMixGoesAheadWhenItCan()
        {
            Artist artist = new Artist() { Name = "Name" };
            await artist.PlayMix();
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestAristNamePropertyIsRequiredForShow()
        {
            Artist artist = new Artist();
            await artist.Show();
        }

        [Test]
        public async Task TestArtistShowGoesAheadWhenItCan()
        {
            Artist artist = new Artist() { Id = "Id" };
            await artist.Show();
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestMixIdPropertyIsRequiredForPlay()
        {
            Mix mix = new Mix();
            await mix.Play();
        }

        [Test]
        public async Task TestPlayMixGoesAheadWhenItCan()
        {
            Mix mix = new Mix() { Id = "1234" };
            await mix.Play();
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestIdPropertyIsRequiredForShow()
        {
            Product product = new Product();
            await product.Show();
        }

        [Test]
        public async Task TestProductShowGoesAheadWhenItCan()
        {
            Product product = new Product() { Id = "Id" };
            await product.Show();
            Assert.Pass();
        }
    }
}
