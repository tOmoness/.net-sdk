// -----------------------------------------------------------------------
// <copyright file="FakeAuthHeaderProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Internal.Authorization;

namespace Nokia.Music.Tests.Internal
{
    public class FakeAuthHeaderProvider : IAuthHeaderDataProvider
    {
        public string GetUserToken()
        {
            return "user-token";
        }
    }
}
