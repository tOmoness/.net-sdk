// -----------------------------------------------------------------------
// <copyright file="JsonResponseCallback.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Internal.Response
{
    /// <summary>
    /// Defines a callback to parse a json response
    /// </summary>
    internal sealed class JsonResponseCallback : IResponseCallback<JObject>
    {
        private readonly Action<Response<JObject>> _callback;

        public JsonResponseCallback(Action<Response<JObject>> callback)
        {
            this._callback = callback;
        }

        public Action<Response<JObject>> Callback
        {
            get { return this._callback; }
        }

        public JObject ConvertFromRawResponse(string response)
        {
            JObject json = null;
            if (!string.IsNullOrEmpty(response))
            {
                json = ParseHelper.ParseWithDate(response);
            }

            return json;
        }
    }
}
