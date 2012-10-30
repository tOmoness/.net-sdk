// -----------------------------------------------------------------------
// <copyright file="ShowMixesTaskTests.cs" company="NOKIA">
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
