// -----------------------------------------------------------------------
// <copyright file="IUserIdProvider.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MixRadio.Internal.Authorization
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