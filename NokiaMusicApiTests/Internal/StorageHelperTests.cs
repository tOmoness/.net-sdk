// -----------------------------------------------------------------------
// <copyright file="StorageHelperTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Tests.Internal;
using NUnit.Framework;

namespace Nokia.Music.Tests
{
    [TestFixture]
    public class StorageHelperTests
    {
        [Test]
        public void EnsureFileOperationsWork()
        {
            const string FileName = "file.txt";

            // In case of previous failures...
            StorageHelper.DeleteFileAsync(FileName).Wait();

            Assert.IsFalse(StorageHelper.FileExistsAsync(FileName).Result, "Expected file not to exist");

            string content = DateTime.Now.ToString();

            StorageHelper.WriteTextAsync(FileName, content).Wait();

            Assert.IsTrue(StorageHelper.FileExistsAsync(FileName).Result, "Expected file to exist");

            var t = StorageHelper.ReadTextAsync(FileName);
            t.Wait();

            Assert.AreEqual(content, t.Result, "Expected text to match");

            StorageHelper.DeleteFileAsync(FileName).Wait();

            Assert.IsFalse(StorageHelper.FileExistsAsync(FileName).Result, "Expected file not to exist");
        }

        [Test]
        public void EnsureErrorCasesAreCaught()
        {
            Assert.IsFalse(StorageHelper.FileExistsAsync(null).Result, "Expected file not to exist");
            Assert.IsNullOrEmpty(StorageHelper.ReadTextAsync(null).Result, "Expected null result");
        }
    }
}
