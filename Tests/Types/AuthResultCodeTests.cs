// -----------------------------------------------------------------------
// <copyright file="AuthResultCodeTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Nokia.Music.Internal.Authorization;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// AuthResultCode tests
    /// </summary>
    [TestFixture]
    public class AuthResultCodeTests
    {
        [Test]
        public void EnsureValidErrorCodesAreParsed()
        {
            Assert.AreEqual(AuthResultCode.AccessDenied, "access_denied".ToAuthResultReason(), "Expected enum result");
            Assert.AreEqual(AuthResultCode.UnauthorizedClient, "unauthorized_client".ToAuthResultReason(), "Expected enum result");
            Assert.AreEqual(AuthResultCode.InvalidScope, "invalid_scope".ToAuthResultReason(), "Expected enum result");
            Assert.AreEqual(AuthResultCode.ServerError, "server_error".ToAuthResultReason(), "Expected enum result");
            Assert.AreEqual(AuthResultCode.Unknown, "somethingelse".ToAuthResultReason(), "Expected enum result");
        }

        [Test]
        public void EnsureEmptyQuerystringGivesFalseResult()
        {
            string authorizationCode = null;
            AuthResultCode resultCode = AuthResultCode.Success;

            OAuthResultParser.ParseQuerystringForCompletedFlags(null, out resultCode, out authorizationCode);
            Assert.AreEqual(AuthResultCode.Unknown, resultCode, "Expected the result code reset");
            Assert.IsNullOrEmpty(authorizationCode, "Expected a null authorization code");
        }

        [Test]
        public void EnsureIrrelevantQuerystringGivesFalseResult()
        {
            string authorizationCode = null;
            AuthResultCode resultCode = AuthResultCode.Success;

            OAuthResultParser.ParseQuerystringForCompletedFlags("?expectingnullresult=true", out resultCode, out authorizationCode);
            Assert.AreEqual(AuthResultCode.Unknown, resultCode, "Expected the result code reset");
            Assert.IsNullOrEmpty(authorizationCode, "Expected a null authorization code");
        }

        [Test]
        public void EnsureSuccessQuerystringGivesTrueResult()
        {
            string authCode = Guid.NewGuid().ToString();
            string authorizationCode = null;
            AuthResultCode resultCode = AuthResultCode.Unknown;

            OAuthResultParser.ParseQuerystringForCompletedFlags("?code=" + authCode, out resultCode, out authorizationCode);
            Assert.AreEqual(AuthResultCode.Success, resultCode, "Expected the result code to be set");
            Assert.AreEqual(authCode, authorizationCode, "Expected the authorization code back");
        }

        [Test]
        public void EnsureErrorCaseQuerystringGivesTrueResult()
        {
            string authorizationCode = null;
            AuthResultCode resultCode = AuthResultCode.Unknown;

            OAuthResultParser.ParseQuerystringForCompletedFlags("?error=access_denied&error_description=denied", out resultCode, out authorizationCode);
            Assert.AreEqual(AuthResultCode.AccessDenied, resultCode, "Expected the result code to be set");
            Assert.IsNullOrEmpty(authorizationCode, "Expected a null authorization code");
        }
    }
}
