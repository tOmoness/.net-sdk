// -----------------------------------------------------------------------
// <copyright file="FakeAuthHeaderProvider.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using MixRadio.Internal.Authorization;

namespace MixRadio.Tests.Internal
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
