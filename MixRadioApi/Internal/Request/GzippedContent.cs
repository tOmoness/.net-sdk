// -----------------------------------------------------------------------
// <copyright file="GzippedContent.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MixRadio.Internal.Request
{
    /// <summary>
    /// Provides GZip sending of data
    /// </summary>
    internal class GzippedContent : HttpContent
    {
        private readonly HttpContent _originalContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GzippedContent"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        internal GzippedContent(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this._originalContent = content;
        }

        /// <summary>
        /// Determines whether the HTTP content has a valid length in bytes.
        /// </summary>
        /// <param name="length">The length in bytes of the HTTP content.</param>
        /// <returns>
        /// Returns <see cref="T:System.Boolean" />.true if <paramref name="length" /> is a valid length; otherwise, false.
        /// </returns>
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        /// <summary>
        /// Serialize the HTTP content to a stream as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="context">Information about the transport (channel binding token, for example). This parameter may be null.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task" />.The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (Stream compressedStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                await this._originalContent.CopyToAsync(compressedStream);
            }
        }
    }
}