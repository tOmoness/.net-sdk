// -----------------------------------------------------------------------
// <copyright file="GzipHandlerWp.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

namespace Nokia.Music.Internal.Compression
{
    /// <summary>
    /// Handles enabling and processing of gzip responses for windows phone
    /// </summary>
    internal class GzipHandlerWp : IGzipHandler
    {
        public GzipHandlerWp()
        {
#if WINDOWS_PHONE
            WebRequest.RegisterPrefix("http://", SharpGIS.WebRequestCreator.GZip);
            WebRequest.RegisterPrefix("https://", SharpGIS.WebRequestCreator.GZip);
#endif
        }

        /// <summary>
        /// Adds the gzip header to a web request
        /// </summary>
        /// <param name="request">The web request</param>
        public void EnableGzip(WebRequest request)
        {
            // Nothing to do here - gzip is handled by the GZipWebClient 
        }

        /// <summary>
        /// Determines whether response is gzipped and invokes platform specific decompression if necessary
        /// </summary>
        /// <param name="response">The web response</param>
        /// <returns>The response stream</returns>
        public Stream GetResponseStream(WebResponse response)
        {
            // Nothing to do here - gzip is handled by the GZipWebClient
            return response.GetResponseStream();
        }
    }
}
