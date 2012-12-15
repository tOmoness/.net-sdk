// -----------------------------------------------------------------------
// <copyright file="ApiMethod.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Represents a Music Api Method
    /// </summary>
    public abstract class ApiMethod
    {
        /// <summary>
        /// Gets a value indicating whether the API method requires a country code to be specified.
        /// API methods require a country code by default. Override this method for calls that do not.
        /// </summary>
        internal virtual bool RequiresCountryCode
        {
            get { return true; }
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
