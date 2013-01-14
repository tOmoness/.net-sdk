// -----------------------------------------------------------------------
// <copyright file="ApiMethod.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Represents a Music Api Method
    /// </summary>
    public abstract class ApiMethod
    {
        private string _baseApiUri = @"http://api.ent.nokia.com/1.x/";
        private Guid _requestId = Guid.Empty;

        /// <summary>
        /// Gets a value indicating whether the API method requires a country code to be specified.
        /// API methods require a country code by default. Override this method for calls that do not.
        /// </summary>
        internal virtual bool RequiresCountryCode
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the base uri for Api requests
        /// </summary>
        internal virtual string BaseApiUri
        {
            get
            {
                return this._baseApiUri;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._baseApiUri = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets an id representing this request.
        /// </summary>
        internal Guid RequestId
        {
            get { return this._requestId; }
            set { this._requestId = value; }
        }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// By default, no path is added, override this to add a uri subpath for a method
        /// </summary>
        /// <param name="uri">The base uri</param>
        /// <param name="pathParams">The API method parameters</param>
        internal virtual void AppendUriPath(StringBuilder uri, Dictionary<string, string> pathParams)
        {
            // Nothing to do by default
        }
    }
}
