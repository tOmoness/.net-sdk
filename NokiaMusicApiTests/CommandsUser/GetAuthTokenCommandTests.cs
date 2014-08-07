// -----------------------------------------------------------------------
// <copyright file="GetAuthTokenCommandTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Authorization;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Commands
{
    [TestFixture]
    public class GetAuthTokenCommandTests
    {
        [Test]
        public void EnsureStockPropertiesAreCorrect()
        {
            var cmd = new GetAuthTokenCommand();

            Assert.AreEqual(MusicClientCommand.DefaultBaseApiUri, cmd.BaseApiUri, "Expected the right value");
            Assert.AreEqual(MusicClientCommand.ContentTypeFormPost, cmd.ContentType, "Expected the right value");
            Assert.AreEqual(false, cmd.RequiresCountryCode, "Expected the right value");
            Assert.AreEqual(true, cmd.RequiresEmptyQuerystring, "Expected the right value");
            Assert.AreEqual(HttpMethod.Post, cmd.HttpMethod, "Expected the right value");

            StringBuilder sb = new StringBuilder();
            cmd.AppendUriPath(sb);
            Assert.AreEqual("token/", sb.ToString(), "Expected the right value");

            // Ensure right Grant Type is returned...
            cmd.AuthorizationCode = "code";
            Assert.AreEqual(GetAuthTokenCommand.GrantTypeAuthorizationCode, cmd.GrantType, "Expected the right value");

            cmd.AuthorizationCode = null;
            cmd.RefreshToken = "token";
            Assert.AreEqual(GetAuthTokenCommand.GrantTypeRefreshToken, cmd.GrantType, "Expected the right value");
        }

        [Test]
        public void EnsurePropertiesPersist()
        {
            const string TestValue = "value";
            var cmd = new GetAuthTokenCommand();
            cmd.ClientId = TestValue;
            Assert.AreEqual(TestValue, cmd.ClientId, "Expected value to persist");

            cmd.ClientSecret = TestValue;
            Assert.AreEqual(TestValue, cmd.ClientSecret, "Expected value to persist");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureGrantTypeThrowsForNullAuthCodeAndRefreshToken()
        {
            var cmd = new GetAuthTokenCommand();
            string grantType = cmd.GrantType;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureAuthHeaderThrowsForNullClientId()
        {
            var cmd = new GetAuthTokenCommand();
            cmd.ConstructAuthHeader();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnsureAuthHeaderThrowsForNullClientSecret()
        {
            var cmd = new GetAuthTokenCommand();
            cmd.ClientId = "id";
            cmd.ConstructAuthHeader();
        }

        [Test]
        public void EnsureAuthHeaderReturnsValueForValidClientIdAndSecret()
        {
            var cmd = new GetAuthTokenCommand();
            cmd.ClientId = "id";
            cmd.ClientSecret = "secret";
            Assert.IsNotNullOrEmpty(cmd.ConstructAuthHeader(), "Expected an auth header");
        }

        [Test]
        [Ignore("Not needed while we only have one public scope")]
        public void EnsureConvertParamsToStringRespectsForHeaderParam()
        {
            var request = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", GetAuthTokenCommand.GrantTypeAuthorizationCode),
                    new KeyValuePair<string, string>("scope", Scope.ReadUserPlayHistory.AsStringParam())
                };

            var cmd = new GetAuthTokenCommand();
            var bodyParams = cmd.ConvertParamsToString(request, false);
            var headerParams = cmd.ConvertParamsToString(request, true);

            Assert.IsTrue(bodyParams.IndexOf("+") > -1, "Expected + encoded spaces");
            Assert.IsTrue(bodyParams.IndexOf("%20") == -1, "Expected + encoded spaces");

            Assert.IsTrue(headerParams.IndexOf("%20") > -1, "Expected %20 encoded spaces");
            Assert.IsTrue(headerParams.IndexOf("+") == -1, "Expected %20 encoded spaces");
        }

        [Test]
        public void EnsureKeysSortedByConvertParamsToString()
        {
            var request = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("scope", Scope.ReadUserPlayHistory.AsStringParam()),
                    new KeyValuePair<string, string>("grant_type", GetAuthTokenCommand.GrantTypeAuthorizationCode)
                };

            var cmd = new GetAuthTokenCommand();
            var stringForm = cmd.ConvertParamsToString(request, false);

            Assert.AreEqual("grant_type=authorization_code&scope=read_userplayhistory", stringForm, "Expected params to be re-sorted alphabetically");
        }

        [Test]
        public async Task EnsureResponseParsedForValidAuthCodeRequest()
        {
            var cmd = new GetAuthTokenCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Success(Resources.token_response, MusicClientCommand.ContentTypeJson)),
                OAuth2 = new OAuth2(null),
                AuthorizationCode = "code",
                ClientId = "test",
                ClientSecret = "test",
                ClientSettings = new MockMusicClientSettings("test", "gb", null)
            };

            var t = await cmd.ExecuteAsync(null);
            Assert.IsNotNull(t.Result, "Expected a result");
            Assert.IsNotNullOrEmpty(t.Result.AccessToken, "Expected an access token");
            Assert.IsTrue(t.Result.ExpiresIn > 0, "Expected expires > 0");
            Assert.IsNotNullOrEmpty(t.Result.AccessToken, "Expected a refresh token");
            Assert.IsNotNull(t.Result.UserId, "Expected a user id");
            Assert.IsNotNullOrEmpty(t.Result.Territory, "Expected territory");
        }

        [Test]
        public async Task EnsureResponseParsedForValidRefreshTokenRequest()
        {
            var cmd = new GetAuthTokenCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Success(Resources.token_response, MusicClientCommand.ContentTypeJson)),
                OAuth2 = new OAuth2(null),
                RefreshToken = "token",
                ClientId = "test",
                ClientSecret = "test",
                ClientSettings = new MockMusicClientSettings("test", "gb", null)
            };

            var t = await cmd.ExecuteAsync(null);
            Assert.IsNotNull(t.Result, "Expected a result");
            Assert.IsNotNullOrEmpty(t.Result.AccessToken, "Expected an access token");
            Assert.IsTrue(t.Result.ExpiresIn > 0, "Expected expires > 0");
            Assert.IsNotNullOrEmpty(t.Result.AccessToken, "Expected a refresh token");
            Assert.IsNotNull(t.Result.UserId, "Expected a user id");
            Assert.IsNotNullOrEmpty(t.Result.Territory, "Expected territory");
        }

        [Test]
        public async Task EnsureExceptionGivenForBadApiCredentials()
        {
            var cmd = new GetAuthTokenCommand()
            {
                RequestHandler = new MockApiRequestHandler(FakeResponse.Unauthorized()),
                OAuth2 = new OAuth2(null),
                AuthorizationCode = "code",
                ClientId = "test",
                ClientSecret = "test",
                ClientSettings = new MockMusicClientSettings(null, "test", null)
            };

            var t = await cmd.ExecuteAsync(null);
            Assert.IsNull(t.Result, "Expected no result");
            Assert.IsNotNull(t.Error, "Expected an error");
            Assert.AreNotEqual(typeof(InvalidApiCredentialsException), t.Error.GetType());
        }
    }
}
