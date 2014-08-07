// -----------------------------------------------------------------------
// <copyright file="ShowMixesTaskTests.cs" company="NOKIA">
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
    /// ShowMixesTaskTests Tests
    /// </summary>
    [TestFixture]
    public class ShowMixesTaskTests
    {
        [Test]
        public async Task TestShowMixesTaskGoesAhead()
        {
            ShowMixesTask task = new ShowMixesTask();
            await task.Show();
        }
    }
}
