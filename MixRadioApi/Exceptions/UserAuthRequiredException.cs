// -----------------------------------------------------------------------
// <copyright file="UserAuthRequiredException.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music
{
    /// <summary>
    /// Exception when an API method requires user authorization
    /// </summary>
    public class UserAuthRequiredException : NokiaMusicException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthRequiredException"/> class.
        /// </summary>
        public UserAuthRequiredException()
            : base("User authorization required, call AuthenticateUserAsync or GetAuthenticationTokenAsync and ensure IsUserAuthenticated is true before calling this method")
        {
        }
    }
}
