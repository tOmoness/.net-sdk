// -----------------------------------------------------------------------
// <copyright file="OAuth2.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nokia.Music.Internal.Authorization
{
    /// <summary>
    /// Represents OAuth2 request
    /// </summary>
    internal sealed class OAuth2
    {
        private IAuthHeaderDataProvider _authHeaderData;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2"/> class.
        /// </summary>
        /// <param name="authHeaderData">The auth header data.</param>
        internal OAuth2(IAuthHeaderDataProvider authHeaderData)
        {
            this._authHeaderData = authHeaderData;
        }

        /// <summary>
        /// Creates the headers.
        /// </summary>
        /// <returns>A Dictionary of headers</returns>
        internal Dictionary<string, string> CreateHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("Bearer {0}", this._authHeaderData.GetUserToken()));
            return headers;
        }
    }
}
