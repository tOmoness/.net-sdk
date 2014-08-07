// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
#if SUPPORTS_OAUTH || UNIT_TESTS
using Nokia.Music.Internal.Authorization;
#endif
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
        internal const string ParamExclusivity = "excl";
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
        /// Signifies a method for converting a JToken into a typed object that uses client settings
        /// - e.g. to generate API URI properties such as artist images, sample clips
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="settings">The client settings.</param>
        /// <returns>
        /// A typed object
        /// </returns>
        internal delegate T JTokenConversionDelegate<T>(JToken item, IMusicClientSettings settings);

#if OPEN_INTERNALS
        /// <summary>
        /// Gets the HTTP content type for the request, exposed to private API
        /// </summary>
        public string RequestContentType
        {
            get { return this.ContentType; }
        }

        /// <summary>
        /// Gets a string form of the HTTP method
        /// </summary>
        public string Method
        {
            get { return HttpMethod.ToString().ToUpperInvariant(); }
        }

        /// <summary>
        /// Gets the body contents.
        /// </summary>
        /// <value>
        /// The body contents.
        /// </value>
        public string BodyContents
        {
            get { return this.BuildRequestBody(); }
        }
#endif

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
        /// Gets the full, raw URI to be used if the command supplies its own non-API structured URI.
        /// Primarily used in CustomCommands, but in theory could be applied to any command.
        /// </summary>
        internal virtual Uri RawUri
        {
            get { return null; }
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
        /// Gets the content type for this request
        /// </summary>
        internal virtual string ContentType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the HTTP method used for this request. GET by default
        /// </summary>
        internal virtual HttpMethod HttpMethod
        {
            get { return HttpMethod.Get; }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        internal IMusicClientSettings ClientSettings { get; set; }

        /// <summary>
        /// Gets or sets the request handler.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; set; }

        /// <summary>
        /// Gets a value indicating whether redirects should be followed by the Http client. Only needs
        /// to be overriden if you need to extract the redirect URL
        /// </summary>
        internal virtual bool FollowHttpRedirects
        {
            get { return true; }
        }

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
        /// Gets domain that this command should be using e.g. domain=music
        /// </summary>
        internal virtual string ServiceDomain
        {
            get
            {
                return "music";
            }
        }

#if SUPPORTS_OAUTH || UNIT_TESTS
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
#endif

        /// <summary>
        /// Gets a value indicating whether the X-MixRadio header is expected in this command response.
        /// </summary>
        internal virtual bool ExpectsMixRadioHeader
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request body should be gzipped
        /// </summary>
        internal virtual bool GzipRequestBody
        {
            get { return false; }
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
        /// Allows an API method to supply params to be sent in the query string of a request
        /// </summary>
        /// <returns>The query string params - Null by default, override to supply query string params for an API method</returns>
        internal virtual List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Allows an API method to supply values to be sent in the headers of a request
        /// </summary>
        /// <returns>The request headers - Null by default, override to supply request hreaders for an API method</returns>
#if SUPPORTS_OAUTH
        internal virtual async Task<Dictionary<string, string>> BuildRequestHeadersAsync()
        {
            if (this.OAuth2 != null)
            {
                return await this.OAuth2.CreateHeadersAsync();
            }

            return new Dictionary<string, string>();
        }
#else
        internal virtual Task<Dictionary<string, string>> BuildRequestHeadersAsync()
        {
            return Task.FromResult(new Dictionary<string, string>());
        }
#endif

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
    }
}