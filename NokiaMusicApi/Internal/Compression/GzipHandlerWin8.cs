// -----------------------------------------------------------------------
// <copyright file="GzipHandlerWin8.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Nokia.Music.Internal.Compression
{
    /// <summary>
    /// Handles enabling and processing of gzip responses
    /// </summary>
    internal class GzipHandlerWin8 : IGzipHandler
    {
        /// <summary>
        /// Adds the gzip header to a web request
        /// </summary>
        /// <param name="request">The web request</param>
        public void EnableGzip(WebRequest request)
        {
            try
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            }
            catch (Exception ex)
            {
                DebugLogger.Instance.WriteLog("Failed to add gzip header {0}", ex);
            }
        }

        /// <summary>
        /// Determines whether response is gzipped and invokes platform specific decompression if necessary
        /// </summary>
        /// <param name="response">The web response</param>
        /// <returns>The response stream</returns>
        public Stream GetResponseStream(WebResponse response)
        {
            bool gzipped = false;
            if (response.Headers != null && response.Headers.Count > 0)
            {
                var headerEncoding = response.Headers["Content-Encoding"];
                gzipped = headerEncoding != null && headerEncoding.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) > -1;
            }

            return gzipped ?
                new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
                : response.GetResponseStream();
        }
    }
}