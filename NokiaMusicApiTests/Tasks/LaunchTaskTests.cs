﻿// -----------------------------------------------------------------------
// <copyright file="LaunchTaskTests.cs" company="NOKIA">
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
    /// LaunchTask Tests
    /// </summary>
    [TestFixture]
    public class LaunchTaskTests
    {
        [Test]
        public async Task TestLaunchTaskGoesAhead()
        {
            LaunchTask task = new LaunchTask();
            await task.Show();
        }
    }
}
