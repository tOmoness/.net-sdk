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
#if OPEN_INTERNALS
        public
#else
        internal
#endif
    abstract class MusicClientCommand
    {
        internal const string DefaultBaseApiUri = "http://api.ent.nokia.com/1.x/";
        internal const string DefaultSecureBaseApiUri = "https://sapi.ent.nokia.com/1.x/";
        internal const string ArrayNameItems = "items";
        
        internal const string ContentTypeFormPost = "application/x-www-form-urlencoded";
        internal const string ContentTypeApiResponseStart = "application/vnd.nokia.ent";
        internal const string ContentTypeJson = "application/json";
        
        internal const string ParamId = "id";
        internal const string ParamCategory = "category";
        internal const string ParamOrderBy = "orderby";
        internal const string ParamSortOrder = "sortorder";
        internal const string ParamExclusive = "exclusive";
        internal const string ParamGenre = "genre";
        internal const string ParamLocation = "location";
        internal const string ParamMaxDistance = "maxdistance";
        internal const string ParamSearchTerm = "q";
        internal const string ParamMaxItems = "maxitems";
        internal const string PagingStartIndex = "startindex";
        internal const string PagingItemsPerPage = "itemsperpage";
        internal const string PagingTotal = "total";

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
        /// Gets the content type for this request
        /// </summary>
        internal virtual string ContentType
        {
            get { return null; }
        }

#if OPEN_INTERNALS
        /// <summary>
        /// The HTTP content type for the request, exposed to private API
        /// </summary>
        public string RequestContentType
        {
            get { return ContentType; }
        }
#endif

        /// <summary>
        /// Gets the HTTP method used for this request. GET by default
        /// </summary>
        internal virtual HttpMethod HttpMethod
        {
            get { return HttpMethod.Get; }
        }

#if OPEN_INTERNALS
        /// <summary>
        /// A string form of the HTTP method
        /// </summary>
        public string Method
        {
            get { return HttpMethod.ToString().ToUpperInvariant(); }
        }
#endif

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        internal IMusicClientSettings ClientSettings { get; set; }

        /// <summary>
        /// Gets or sets an id representing this request.
        /// </summary>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        Guid RequestId
        {
            get { return this._requestId; }
            set { this._requestId = value; }
        }

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

        /// <summary>
        /// Gets a value indicating whether to use a blank querystring.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should use a blank querystring.
        /// </value>
        internal virtual bool RequiresEmptyQuerystring
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the command should throw upon error responses.
        /// <remarks>This will be the default behaviour in the next major version of the API, but is experimental and only used in the CountryResolver class for now</remarks>
        /// </summary>
        internal virtual bool ThrowOnError
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether to use a blank territory in the URI path.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should use a blank territory.
        /// </value>
        internal virtual bool UseBlankTerritory
        {
            get { return false; }
        }

        /// <summary>
        /// Allows an API method to supply data to be sent in the body of a request
        /// </summary>
        /// <returns>The request data - Null by default, override to supply data for an API method</returns>
        internal virtual string BuildRequestBody()
        {
            return null;
        }

#if OPEN_INTERNALS
        public string BodyContents
        {
            get { return BuildRequestBody(); }
        }
#endif

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
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        virtual void SetAdditionalResponseInfo(ResponseInfo responseInfo)
        {
            // Does nothing by default
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        protected abstract void Execute();
    }
}