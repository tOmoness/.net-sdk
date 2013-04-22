// -----------------------------------------------------------------------
// <copyright file="JsonResponseCallbackTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Response;
using NUnit.Framework;

namespace Nokia.Music.Tests.Internal
{
    [TestFixture]
    public class JsonResponseCallbackTests
    {
        [Test]
        public static void ConvertFromRawResponseDoesNotChangeDateTimes()
        {
            var jsonResponseCallback = new JsonResponseCallback(r => Assert.Pass());
            string json = "{ \"time\" : \"2013-08-11T19:00:00.000-05:00\" }";
            JObject jobject = jsonResponseCallback.ConvertFromRawResponse(json);

            Assert.That(jobject, Is.Not.Null, "Items was null");
            Assert.That(jobject.Value<string>("time"), Is.EqualTo("2013-08-11T19:00:00.000-05:00"));
        }
    }
}
