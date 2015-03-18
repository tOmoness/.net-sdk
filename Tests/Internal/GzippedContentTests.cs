// -----------------------------------------------------------------------
// <copyright file="GzippedContentTests.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music.Internal.Request;
using NUnit.Framework;

namespace Nokia.Music.Tests.Internal
{
    [TestFixture]
    public class GzippedContentTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithNoContentThrowsException()
        {
            new GzippedContent(null);
        }
        
        [Test]
        public async Task SerializeToStreamGzipsContent()
        {
            // Arrange
            var testContent = "Here is some test content";
            var content = new GzippedContentWrapper(new StringContent(testContent));
            string decompressedMessage;
            
            // Act
            using (var memoryStream = new MemoryStream())
            {
                await content.CallSerializeToStreamAsync(memoryStream);
                memoryStream.Position = 0;

                using (Stream decompressedStream = new GZipStream(memoryStream, CompressionMode.Decompress, true))
                using (TextReader reader = new StreamReader(decompressedStream, Encoding.UTF8))
                {
                    decompressedMessage = reader.ReadToEnd();
                }
            }

            // Assert
            Assert.AreEqual(testContent, decompressedMessage);
        }

        [Test]
        public void TryComputeLengthCannotCompute()
        {
            // Arrange
            var content = new GzippedContentWrapper(new StringContent("Some content"));

            // Act
            long length;
            var result = content.CallTryComputeLength(out length);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(-1, length);
        }

        private class GzippedContentWrapper : GzippedContent
        {
            public GzippedContentWrapper(HttpContent content)
                : base(content)
            {
            }

            public bool CallTryComputeLength(out long length)
            {
                return this.TryComputeLength(out length);
            }

            public async Task CallSerializeToStreamAsync(Stream stream)
            {
                await this.SerializeToStreamAsync(stream, null);
            }
        }
    }
}
