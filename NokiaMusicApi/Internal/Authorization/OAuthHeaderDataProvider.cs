// -----------------------------------------------------------------------
// <copyright file="OAuthHeaderDataProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Implementation of IAuthHeaderDataProvider
    /// </summary>
    internal class OAuthHeaderDataProvider : IAuthHeaderDataProvider
    {
        private readonly Task<string> _userTokenTask;
        private readonly Task<string> _userIdTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthHeaderDataProvider"/> class.
        /// </summary>
        /// <param name="userToken">The user token.</param>
        /// <param name="userId">The user id.</param>
        public OAuthHeaderDataProvider(string userToken, string userId)
        {
            this._userTokenTask = Task.FromResult(userToken);
            this._userIdTask = Task.FromResult(userId);
        }

        public Task<string> GetUserTokenAsync()
        {
            return this._userTokenTask;
        }

        public string HashForTokenAuthentication(string data)
        {
            return null;
        }

        public Task<string> GetUserIdAsync()
        {
            return this._userIdTask;
        }

        public Task InvalidateUserTokenAsync()
        {
            // public implementation does nothing
            return Task.FromResult(0);
        }
    }
}
