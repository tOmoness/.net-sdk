// -----------------------------------------------------------------------
// <copyright file="RawResponseCallback.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music;

namespace Nokia.Music.Internal.Response
{
    internal sealed class RawResponseCallback : IResponseCallback<string>
    {
        private Action<Response<string>> _callback;

        public RawResponseCallback(Action<Response<string>> callback)
        {
            this._callback = callback;
        }

        public Action<Response<string>> Callback
        {
            get { return this._callback; }
        }

        public string ConvertFromRawResponse(string response)
        {
            return response;
        }
    }
}
