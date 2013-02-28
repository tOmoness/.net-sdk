// -----------------------------------------------------------------------
// <copyright file="IResponseCallback.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Nokia.Music.Phone.Internal.Response
{
    internal interface IResponseCallback<T>
    {
        Action<Response<T>> Callback { get; }

        T ConvertFromRawResponse(string response);
    }
}
