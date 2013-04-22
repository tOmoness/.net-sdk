// -----------------------------------------------------------------------
// <copyright file="ShowMixesTaskTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
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
    /// ShowMixesTaskTests Tests
    /// </summary>
    [TestFixture]
    public class ShowMixesTaskTests
    {
        [Test]
        public void TestShowMixesTaskGoesAhead()
        {
            ShowMixesTask task = new ShowMixesTask();
            task.Show();
        }
    }
}
