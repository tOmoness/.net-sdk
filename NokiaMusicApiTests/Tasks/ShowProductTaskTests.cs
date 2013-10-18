// -----------------------------------------------------------------------
// <copyright file="ShowProductTaskTests.cs" company="NOKIA">
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
    /// ShowProductTask Tests
    /// </summary>
    [TestFixture]
    public class ShowProductTaskTests
    {
        private const string TestAppId = "AppId";
        private const string TestProductId = "8061375";
        
        [Test]
        public void TestAppIdPropertyPersists()
        {
            ShowProductTask task = new ShowProductTask() { AppId = TestAppId };
            Assert.AreEqual(TestAppId, task.AppId, "Expected the same ID");
        }

        [Test]
        public void TestProductIdPropertyPersists()
        {
            ShowProductTask task = new ShowProductTask() { ProductId = TestProductId };
            Assert.AreEqual(TestProductId, task.ProductId, "Expected the same ID");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestProductIdPropertyIsRequiredForShow()
        {
            ShowProductTask task = new ShowProductTask();
            task.Show();
        }

        [Test]
        public void TestShowProductGoesAheadWhenItCan()
        {
            ShowProductTask task = new ShowProductTask() { AppId = TestAppId, ProductId = TestProductId };
            task.Show();            
            Assert.Pass();
        }
    }
}
