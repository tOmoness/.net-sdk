// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MusicClientCommandTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Authorization;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class MusicClientCommandTests
    {
        [Test]
        public void EnsureGzipRequestBodyFalseBYDefault()
        {
            var command = new MockMusicClientCommand();
            Assert.IsFalse(command.GzipRequestBody, "Expected GzipRequestBody false by default");
        }

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

            var response = command.ItemResponseHandler(
                unavailableResponse,
                (item, settings) => item);

            Assert.That(!response.Succeeded);
            Assert.That(response.Error, Is.InstanceOf<NetworkUnavailableException>());
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

            var response = command.ListItemResponseHandler(
                unavailableResponse, 
                "items",
                (item, settings) => item);

            Assert.That(!response.Succeeded);
            Assert.That(response.Error, Is.InstanceOf<NetworkUnavailableException>());
            Assert.That(response.Error.Message, Is.EqualTo("This is a message."));
        }

        [Test]
        public void NetworkLimitedIsPassedThroughForLists()
        {
            var command = new MockMusicClientCommand();
            var unavailableResponse = new Response<JObject>(
                HttpStatusCode.NotFound,
                new NetworkLimitedException("This is a message."),
                string.Empty,
                Guid.NewGuid());

            var response = command.ListItemResponseHandler(
                unavailableResponse,
                "items",
                (item, settings) => item);

            Assert.That(!response.Succeeded);
            Assert.That(response.Error, Is.InstanceOf<NetworkLimitedException>());
            Assert.That(response.Error.Message, Is.EqualTo("This is a message."));
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

            var response = command.ListItemResponseHandler(
                availableResponse,
                "items",
                (item, settings) => item);

            Assert.That(response.Succeeded);
            Assert.That(response.Error, Is.Not.InstanceOf<NetworkUnavailableException>());
        }

        [Test]
        public async System.Threading.Tasks.Task UnauthorizedResponseDoesNothingInDefaultImplementation()
        {
            var command = new MockMusicClientCommand
            {
                OAuth2 = new OAuth2(new FakeAuthHeaderProvider()),
                RequestHandler = new MockApiRequestHandler(FakeResponse.RawUnauthorized()),
            };

            await command.ExecuteAsync(null);
        }

        #endregion
    }
}