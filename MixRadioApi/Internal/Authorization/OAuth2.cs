﻿// -----------------------------------------------------------------------
// <copyright file="OAuth2.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MixRadio.Internal.Authorization
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
        internal async Task<Dictionary<string, string>> CreateHeadersAsync()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("Bearer {0}", await this._authHeaderData.GetUserTokenAsync()));
            return headers;
        }

        internal Task InvalidateUserTokenAsync()
        {
            // public implementation does nothing
            return Task.FromResult(0);
        }
    }
}
