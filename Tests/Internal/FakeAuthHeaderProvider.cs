// -----------------------------------------------------------------------
// <copyright file="FakeAuthHeaderProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Nokia.Music.Internal.Authorization;

namespace Nokia.Music.Tests.Internal
{
    public class FakeAuthHeaderProvider : IAuthHeaderDataProvider
    {
        public Task<string> GetUserTokenAsync()
        {
            return Task.FromResult("user-token");
        }

        public Task<string> GetUserIdAsync()
        {
            return Task.FromResult("user-id");
        }
    }
}
