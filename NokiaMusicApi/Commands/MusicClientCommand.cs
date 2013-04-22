// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Request;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    internal abstract class MusicClientCommand
    {
        internal const string ArrayNameItems = "items";
        internal const string ParamId = "id";
        internal const string ParamCategory = "category";
        internal const string ParamExclusive = "exclusive";
        internal const string ParamGenre = "genre";
        internal const string ParamLocation = "location";
        internal const string ParamMaxDistance = "maxdistance";
        internal const string ParamSearchTerm = "q";
        internal const string PagingStartIndex = "startindex";
        internal const string PagingItemsPerPage = "itemsperpage";
        internal const string PagingTotal = "total";
        internal const string DefaultBaseApiUri = "http://api.ent.nokia.com/1.x/";

        private string _baseApiUri = DefaultBaseApiUri;
        private Guid _requestId = Guid.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientCommand" /> class.
        /// </summary>
        internal MusicClientCommand()
        {
        }

        /// <summary>
        /// Signifies a method for converting a JToken into a typed object
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>A typed object</returns>
        internal delegate T JTokenConversionDelegate<T>(JToken item);

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        internal IMusicClientSettings MusicClientSettings { get; set; }

        /// <summary>
        /// Gets or sets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; set; }

        /// <summary>
        /// Gets a value indicating whether the API method requires a country code to be specified.
        /// API methods require a country code by default. Override this method for calls that do not.
        /// </summary>
        internal virtual bool RequiresCountryCode
        {
            get { return true; }
        }

        internal virtual bool UseBlankTerritory
        {
            get { return false; }
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
        /// Gets the HTTP method used for this request. GET by default
        /// </summary>
        internal virtual HttpMethod HttpMethod
        {
            get { return HttpMethod.Get; }
        }

        /// <summary>
        /// Gets the content type for this request
        /// </summary>
        internal virtual string ContentType
        {
            get { return null; }
        }

        /// <summary>
        /// Allows an API method to supply data to be sent in the body of a request
        /// </summary>
        /// <returns>The request data - Null by default, override to supply data for an API method</returns>
        internal virtual string BuildRequestBody()
        {
            return null;
        }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// By default, no path is added, override this to add a uri subpath for a method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal virtual void AppendUriPath(StringBuilder uri)
        {
            // Nothing to do by default
        }

        /// <summary>
        /// In special cases, allows a command to use any additional information about the response
        /// </summary>
        /// <param name="responseInfo">The web response info</param>
        internal virtual void SetAdditionalResponseInfo(ResponseInfo responseInfo)
        {
            // Does nothing by default
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected abstract void Execute();
    }
}