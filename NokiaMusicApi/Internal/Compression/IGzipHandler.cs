// -----------------------------------------------------------------------
// <copyright file="IGzipHandler.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Net;

namespace Nokia.Music.Internal.Compression
{
#if OPEN_INTERNALS
        public
#else
        internal
#endif
    interface IGzipHandler
    {
        /// <summary>
        /// EnablesGzip for the supplied request
        /// </summary>
        /// <param name="request">The request</param>
        void EnableGzip(WebRequest request);
        
        /// <summary>
        /// Determines whether response is gzipped and invokes platform specific decompression if necessary
        /// </summary>
        /// <param name="response">The web response</param>
        /// <returns>The response stream</returns>
        Stream GetResponseStream(WebResponse response);
    }
}
