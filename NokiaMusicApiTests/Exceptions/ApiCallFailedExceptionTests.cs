// -----------------------------------------------------------------------
// <copyright file="ApiCallFailedExceptionTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Exceptions
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
