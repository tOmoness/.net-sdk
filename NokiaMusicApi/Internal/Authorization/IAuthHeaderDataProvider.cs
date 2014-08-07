﻿// -----------------------------------------------------------------------
// <copyright file="IAuthHeaderDataProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Allows a client to specify settings required to build an OAuth2 request
    /// </summary>
    internal interface IAuthHeaderDataProvider : IUserIdProvider
    {
        /// <summary>
        /// Gets the user token.
        /// </summary>
        /// <returns>The user token for authenticating against the REST API</returns>
        Task<string> GetUserTokenAsync();
    }
}
