// -----------------------------------------------------------------------
// <copyright file="ResponseInfo.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;

namespace Nokia.Music.Internal.Response
{
#if OPEN_INTERNALS
        public
#else
        internal
#endif
    class ResponseInfo
    {
        public ResponseInfo(Uri responseUri, WebHeaderCollection headers)
        {
            this.ResponseUri = responseUri;
            this.Headers = headers;
        }

        public Uri ResponseUri { get; private set; }

        public WebHeaderCollection Headers { get; private set; }
    }
}
