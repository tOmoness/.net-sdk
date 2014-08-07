// -----------------------------------------------------------------------
// <copyright file="ShowProductTaskTests.cs" company="NOKIA">
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
    /// ShowProductTask Tests
    /// </summary>
    [TestFixture]
    public class ShowProductTaskTests
    {
        private const string TestClientId = "AppId";
        private const string TestProductId = "8061375";
        
        [Test]
        public void TestAppIdPropertyPersists()
        {
            ShowProductTask task = new ShowProductTask() { ClientId = TestClientId };
            Assert.AreEqual(TestClientId, task.ClientId, "Expected the same ID");
        }

        [Test]
        public void TestProductIdPropertyPersists()
        {
            ShowProductTask task = new ShowProductTask() { ProductId = TestProductId };
            Assert.AreEqual(TestProductId, task.ProductId, "Expected the same ID");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task TestProductIdPropertyIsRequiredForShow()
        {
            ShowProductTask task = new ShowProductTask();
            await task.Show();
        }

        [Test]
        public async Task TestShowProductGoesAheadWhenItCan()
        {
            ShowProductTask task = new ShowProductTask() { ClientId = TestClientId, ProductId = TestProductId };
            await task.Show();            
            Assert.Pass();
        }
    }
}
