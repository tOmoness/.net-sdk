// -----------------------------------------------------------------------
// <copyright file="IUserIdProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Allows a client to specify the current user id
    /// </summary>
#if OPEN_INTERNALS
    public interface IUserIdProvider
#else
    internal interface IUserIdProvider
#endif
    {
        /// <summary>
        /// Gets the UserID.
        /// </summary>
        /// <returns>The UserID</returns>
        Task<string> GetUserIdAsync();
    }
}