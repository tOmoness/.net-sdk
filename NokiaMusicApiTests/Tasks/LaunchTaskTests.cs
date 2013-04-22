// -----------------------------------------------------------------------
// <copyright file="LaunchTaskTests.cs" company="NOKIA">
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
    /// LaunchTask Tests
    /// </summary>
    [TestFixture]
    public class LaunchTaskTests
    {
        [Test]
        public void TestLaunchTaskGoesAhead()
        {
            LaunchTask task = new LaunchTask();
            task.Show();
        }
    }
}
