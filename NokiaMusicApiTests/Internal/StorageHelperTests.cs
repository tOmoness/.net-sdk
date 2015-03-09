// -----------------------------------------------------------------------
// <copyright file="StorageHelperTests.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MixRadio.AuthHelpers;
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
        public async Task EnsureFileOperationsWork()
        {
            const string FileName = "file.txt";

            // In case of previous failures...
            await StorageHelper.DeleteFileAsync(FileName);

            Assert.IsFalse(await StorageHelper.FileExistsAsync(FileName), "Expected file not to exist");

            string content = DateTime.Now.ToString();

            await StorageHelper.WriteTextAsync(FileName, content);

            Assert.IsTrue(await StorageHelper.FileExistsAsync(FileName), "Expected file to exist");

            var t = await StorageHelper.ReadTextAsync(FileName);
            
            Assert.AreEqual(content, t, "Expected text to match");

            await StorageHelper.DeleteFileAsync(FileName);

            Assert.IsFalse(await StorageHelper.FileExistsAsync(FileName), "Expected file not to exist");
        }

        [Test]
        public async Task EnsureErrorCasesAreCaught()
        {
            Assert.IsFalse(await StorageHelper.FileExistsAsync(null), "Expected file not to exist");
            Assert.IsNullOrEmpty(await StorageHelper.ReadTextAsync(null), "Expected null result");
        }
    }
}
