// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandlerFactory.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Internal.Compression;

namespace Nokia.Music.Internal.Request
{
    internal static class ApiRequestHandlerFactory
    {
        public static ApiRequestHandler Create()
        {
#if NETFX_CORE
            return new ApiRequestHandler(new ApiUriBuilder(), new GzipHandlerWin8());
#else
            return new ApiRequestHandler(new ApiUriBuilder(), new GzipHandlerWp());
#endif
        }
    }
}
