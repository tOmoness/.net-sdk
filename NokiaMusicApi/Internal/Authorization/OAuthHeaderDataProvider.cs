// -----------------------------------------------------------------------
// <copyright file="OAuthHeaderDataProvider.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Implementation of IAuthHeaderDataProvider
    /// </summary>
    internal class OAuthHeaderDataProvider : IAuthHeaderDataProvider
    {
        private string _userToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthHeaderDataProvider"/> class.
        /// </summary>
        /// <param name="userToken">The user token.</param>
        public OAuthHeaderDataProvider(string userToken)
        {
            this._userToken = userToken;
        }

        public string GetUserToken()
        {
            return this._userToken;
        }

        public string HashForTokenAuthentication(string data)
        {
            return null;
        }
    }
}
