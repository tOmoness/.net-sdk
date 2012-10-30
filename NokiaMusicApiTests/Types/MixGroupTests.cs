// -----------------------------------------------------------------------
// <copyright file="MixGroupTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// MixGroup tests
    /// </summary>
    [TestFixture]
    public class MixGroupTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            MixGroup group = new MixGroup() { Id = TestId, Name = TestName };

            Assert.AreEqual(TestId, group.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, group.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            MixGroup group = new MixGroup() { Id = TestId, Name = TestName };
            Assert.IsNotNull(group.GetHashCode(), "Expected a hash code");
            Assert.IsFalse(group.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void TestJsonParsing()
        {
            MixGroup group = new MixGroup() { Id = "1234", Name = "Metal" };
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\"}");
            MixGroup fromJson = MixGroup.FromJToken(json) as MixGroup;

            Assert.IsNotNull(fromJson, "Expected a MixGroup object");

            Assert.IsTrue(group.Equals(fromJson), "Expected the same MixGroup");
        }
    }
}
