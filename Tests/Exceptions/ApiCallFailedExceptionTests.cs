// -----------------------------------------------------------------------
// <copyright file="ApiCallFailedExceptionTests.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MixRadio;
using NUnit.Framework;

namespace MixRadio.Tests.Exceptions
{
    [TestFixture]
    public class ApiCallFailedExceptionTests
    {
        [Test]
        public void ExceptionFormatsMessageUsingStatusCode()
        {
            Assert.AreEqual(
                            "Unexpected failure, check connectivity. Result: InternalServerError",
                            new ApiCallFailedException(HttpStatusCode.InternalServerError).Message);
        }

        [Test]
        public void ExceptionFormatsInterpretsNullStatusCodeAsTimeout()
        {
            Assert.AreEqual(
                            "Unexpected failure, check connectivity. Result: timeout",
                            new ApiCallFailedException(null).Message);
        }
    }
}
