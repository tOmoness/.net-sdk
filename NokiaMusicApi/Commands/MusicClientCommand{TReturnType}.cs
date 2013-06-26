// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand{TReturnType}.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    /// <typeparam name="TReturnType">The type of the returned object.</typeparam>
    internal abstract class MusicClientCommand<TReturnType> : MusicClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientCommand{TReturnType}" /> class.
        /// </summary>
        internal MusicClientCommand()
        {
            this.ItemsPerPage = MusicClient.DefaultItemsPerPage;
            this.StartIndex = MusicClient.DefaultStartIndex;
        }

        /// <summary>
        /// Gets or sets the number of items to fetch.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets a json processor that can parse the response expected by this command.
        /// By default, a processor for a named item list is returned.
        /// </summary>
        protected virtual IJsonProcessor JsonProcessor
        {
            get { return new NamedItemListJsonProcessor(); }
        }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        protected Action<TReturnType> Callback { get; set; }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        public void Invoke(Action<TReturnType> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback", "A callback must be supplied");
            }

            this.Callback = callback;

            this.Execute();
        }

        protected static bool IsValidContentType(Response<JObject> rawResult)
        {
            return rawResult.Result != null &&
                   rawResult.ContentType != null &&
                   rawResult.ContentType.StartsWith("application/vnd.nokia.ent", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Generic response handler for content lists
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="rawResult">The response</param>
        /// <param name="itemsName">The json list name</param>
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The client callback</param>
        protected void ListItemResponseHandler<T>(Response<JObject> rawResult, string itemsName, JTokenConversionDelegate<T> converter, Action<ListResponse<T>> callback)
        {
            ListResponse<T> response = null;

            // Parse the result if we got one...
            if (rawResult.StatusCode.HasValue)
            {
                DebugLogger.Instance.WriteVerboseInfo("Parsing list response. Status Code: {0}", rawResult.StatusCode);
                switch (rawResult.StatusCode.Value)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                        if (IsValidContentType(rawResult))
                        {
                            DebugLogger.Instance.WriteVerboseInfo("Valid content type. Parsing...");
                            List<T> results = this.JsonProcessor.ParseList(rawResult.Result, itemsName, converter);
                            int? totalResults = null;
                            int? startIndex = null;
                            int? itemsPerPage = null;

                            JToken paging = rawResult.Result["paging"];
                            if (paging != null)
                            {
                                totalResults = paging.Value<int>(MusicClientCommand.PagingTotal);
                                startIndex = paging.Value<int>(MusicClientCommand.PagingStartIndex);
                                itemsPerPage = paging.Value<int>(MusicClientCommand.PagingItemsPerPage);
                            }

                            response = new ListResponse<T>(rawResult.StatusCode, results, startIndex, itemsPerPage, totalResults, RequestId);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (this.MusicClientSettings.CountryCodeBasedOnRegionInfo)
                        {
                            response = new ListResponse<T>(rawResult.StatusCode, new ApiNotAvailableException(), rawResult.ErrorResponseBody, RequestId);
                        }

                        break;

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        response = new ListResponse<T>(rawResult.StatusCode, new InvalidApiCredentialsException(), rawResult.ErrorResponseBody, RequestId);
                        break;
                }
            }

            if (response == null)
            {
                response = new ListResponse<T>(rawResult.StatusCode, new ApiCallFailedException(rawResult.StatusCode), rawResult.ErrorResponseBody, RequestId);
            }

            callback(response);
        }

        /// <summary>
        /// Generic response handler for single item content
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="rawResult">The response</param>
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The client callback</param>
        protected void ItemResponseHandler<T>(Response<JObject> rawResult, JTokenConversionDelegate<T> converter, Action<Response<T>> callback)
        {
            Response<T> response = null;

            // Parse the result if we got one...
            if (rawResult.StatusCode.HasValue)
            {
                switch (rawResult.StatusCode.Value)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                        if (IsValidContentType(rawResult))
                        {
                            T result = converter(rawResult.Result);
                            response = new Response<T>(rawResult.StatusCode, result, RequestId);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (this.MusicClientSettings.CountryCodeBasedOnRegionInfo)
                        {
                            response = new Response<T>(rawResult.StatusCode, new ApiNotAvailableException(), rawResult.ErrorResponseBody, RequestId);
                        }

                        break;

                    case HttpStatusCode.Forbidden:
                        response = new Response<T>(rawResult.StatusCode, new InvalidApiCredentialsException(), rawResult.ErrorResponseBody, RequestId);
                        break;
                }
            }

            if (response == null)
            {
                response = new Response<T>(rawResult.StatusCode, new ApiCallFailedException(rawResult.StatusCode), rawResult.ErrorResponseBody, RequestId);
            }

            callback(response);
        }

        /// <summary>
        /// Creates an initial querystring dictionary containing paging parameters
        /// </summary>
        /// <returns>A dictionary containing standard querystring paging parameters</returns>
        protected List<KeyValuePair<string, string>> GetPagingParams()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(PagingStartIndex, this.StartIndex.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>(PagingItemsPerPage, this.ItemsPerPage.ToString(CultureInfo.InvariantCulture))
            };
        }
    }
}