// -----------------------------------------------------------------------
// <copyright file="IAuthHeaderDataProvider.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MixRadio.Internal.Authorization
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
