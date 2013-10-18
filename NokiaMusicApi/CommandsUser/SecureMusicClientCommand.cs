// -----------------------------------------------------------------------
// <copyright file="SecureMusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Commands;
using Nokia.Music.Internal.Authorization;

namespace Nokia.Music.Commands
{
    internal abstract class SecureMusicClientCommand<T> : MusicClientCommand<T>
        where T : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureMusicClientCommand{T}"/> class.
        /// </summary>
        internal SecureMusicClientCommand()
        {
            this.SecureApiBaseUri = MusicClientCommand.DefaultSecureBaseApiUri;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        internal string UserId { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 Implementation
        /// </summary>
        internal OAuth2 OAuth2 { get; set; }

        /// <summary>
        /// Gets or sets the base API uri for secure requests
        /// </summary>
        internal string SecureApiBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the base uri for Api requests
        /// </summary>
        internal override string BaseApiUri
        {
            get
            {
                return this.SecureApiBaseUri;
            }
        }
    }
}
