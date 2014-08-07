// -----------------------------------------------------------------------
// <copyright file="UserEventTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// UserEvent tests
    /// </summary>
    [TestFixture]
    public class UserEventTests
    {
        private const string TestClient = "NokiaMusic";
        private const string TestClientVersion = "3.10";
        private const string TestProductId = "6020029";
        private const string TestMixId = "1234";
        private const int TestOffset = 250;
        
        private readonly DateTime when = Convert.ToDateTime("2013-01-01 09:00:00.000+0000");
        private readonly Location where = new Location() { Latitude = LocationTests.TestLatitude, Longitude = LocationTests.TestLongitude };

        public UserEvent MakeUserEvent()
        {
            UserEvent e = new UserEvent()
            {
                Action = UserEventAction.Complete,
                ClientVersion = TestClientVersion,
                DateTime = this.when,
                Location = this.where,
                Mix = new Mix() { Id = TestMixId },
                Offset = TestOffset,
                Product = new Product() { Id = TestProductId },
                Target = UserEventTarget.Track
            };
            return e;
        }

        [Test]
        public void TestProperties()
        {
            UserEvent e = this.MakeUserEvent();

            Assert.AreEqual(UserEventAction.Complete, e.Action, "Expected the property to persist");
            Assert.AreEqual(TestClientVersion, e.ClientVersion, "Expected the property to persist");
            Assert.AreEqual(this.when, e.DateTime, "Expected the property to persist");
            Assert.AreEqual(this.where, e.Location, "Expected the property to persist");
            Assert.AreEqual(TestMixId, e.Mix.Id, "Expected the property to persist");
            Assert.AreEqual(TestOffset, e.Offset, "Expected the property to persist");
            Assert.AreEqual(TestProductId, e.Product.Id, "Expected the property to persist");
            Assert.AreEqual(UserEventTarget.Track, e.Target, "Expected the property to persist");
        }

        [Test]
        public void TestJsonParsing()
        {
            UserEvent e = this.MakeUserEvent();
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.userevent));
            UserEvent eventFromJson = UserEvent.FromJToken(json, null) as UserEvent;

            Assert.IsNotNull(eventFromJson, "Expected a event object");

            Assert.AreEqual(eventFromJson.Action, e.Action, "Expected the property to persist");
            Assert.AreEqual(eventFromJson.ClientVersion, e.ClientVersion, "Expected the property to persist");
            Assert.AreEqual(eventFromJson.DateTime, e.DateTime, "Expected the property to persist");
            Assert.IsTrue(eventFromJson.Location.Equals(e.Location), "Expected the property to persist");
            Assert.IsTrue(eventFromJson.Mix.Equals(e.Mix), "Expected the property to persist");
            Assert.AreEqual(eventFromJson.Offset, e.Offset, "Expected the property to persist");
            Assert.IsTrue(eventFromJson.Product.Equals(e.Product), "Expected the property to persist");
            Assert.AreEqual(eventFromJson.Target, e.Target, "Expected the property to persist");
        }
    }
}
