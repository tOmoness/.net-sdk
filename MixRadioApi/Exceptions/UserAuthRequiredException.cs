// -----------------------------------------------------------------------
// <copyright file="UserAuthRequiredException.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio
{
    /// <summary>
    /// Exception when an API method requires user authorization
    /// </summary>
    public class UserAuthRequiredException : MixRadioException
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
