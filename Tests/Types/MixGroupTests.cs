// -----------------------------------------------------------------------
// <copyright file="MixGroupTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
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
        public void HashCodeCanBeRetrievedWhenIdIsNull()
        {
            MixGroup mixGroup = new MixGroup();
            Assert.IsNotNull(mixGroup.GetHashCode(), "Expected a hash code");
        }

        [Test]
        public void TestJsonParsing()
        {
            MixGroup group = new MixGroup() { Id = "1234", Name = "Metal" };
            JObject json = JObject.Parse("{\"id\":\"1234\",\"name\":\"Metal\"}");
            MixGroup fromJson = MixGroup.FromJToken(json, null) as MixGroup;

            Assert.IsNotNull(fromJson, "Expected a MixGroup object");

            Assert.IsTrue(group.Equals(fromJson), "Expected the same MixGroup");
        }

        [Test]
        public void EnsureUriIsBuiltCorrectly()
        {
            StringBuilder uri = new StringBuilder("http://api.ent.nokia.com/1.x/gb/");
            new MixGroupsCommand().AppendUriPath(uri);
            Assert.AreEqual("http://api.ent.nokia.com/1.x/gb/mixes/groups/", uri.ToString());
        }

        [Test]
        public void TestParamConstructor()
        {
            MixGroup group = new MixGroup("1234", "Metal");

            Assert.AreEqual("1234", group.Id);
            Assert.AreEqual("Metal", group.Name);
        }
    }
}
