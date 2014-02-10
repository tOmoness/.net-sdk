// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MusicClientCommandTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class MusicClientCommandTests
    {
        #region Public Methods and Operators

        [Test]
        public void NetworkUnavailableIsPassedThrough()
        {
            var command = new MockMusicClientCommand();
            var unavailableResponse = new Response<JObject>(
                HttpStatusCode.NotFound, 
                new NetworkUnavailableException(), 
                string.Empty, 
                Guid.NewGuid());

            bool callbackCompleted = false;

            command.ItemResponseHandler(
                unavailableResponse, 
                item => item, 
                response =>
                    {
                        callbackCompleted = true;

                        Assert.That(!response.Succeeded);
                        Assert.That(response.Error, Is.InstanceOf<NetworkUnavailableException>());
                    });

            Assert.That(callbackCompleted);
        }

        [Test]
        public void NetworkUnavailableIsPassedThroughForLists()
        {
            var command = new MockMusicClientCommand();
            var unavailableResponse = new Response<JObject>(
                HttpStatusCode.NotFound, 
                new NetworkUnavailableException("This is a message."), 
                string.Empty, 
                Guid.NewGuid());

            bool callbackCompleted = false;

            command.ListItemResponseHandler(
                unavailableResponse, 
                "items", 
                item => item, 
                response =>
                    {
                        callbackCompleted = true;

                        Assert.That(!response.Succeeded);
                        Assert.That(response.Error, Is.InstanceOf<NetworkUnavailableException>());
                        Assert.That(response.Error.Message, Is.EqualTo("This is a message."));
                    });

            Assert.That(callbackCompleted);
        }

        [Test]
        public void WasNetworkAvailableDoesNotThrowExceptionWhenErrorIsEmpty()
        {
            var command = new MockMusicClientCommand();
            var mix = JToken.FromObject(new Mix());
            var availableResponse = new Response<JObject>(
                HttpStatusCode.OK,
                "application/vnd.nokia.ent",
                (JObject)mix,
                Guid.NewGuid());

            bool callbackCompleted = false;

            command.ListItemResponseHandler(
                availableResponse,
                "items",
                item => item,
                response =>
                {
                    callbackCompleted = true;

                    Assert.That(response.Succeeded);
                    Assert.That(response.Error, Is.Not.InstanceOf<NetworkUnavailableException>());
                });

            Assert.That(callbackCompleted);
        }

        #endregion
    }
}