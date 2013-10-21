// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand{TResult}.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    /// <typeparam name="TResult">The type of the returned object.</typeparam>
    internal abstract class MusicClientCommand<TResult> : MusicClientCommand
        where TResult : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientCommand{TResult}" /> class.
        /// </summary>
        internal MusicClientCommand()
        {
            this.ItemsPerPage = MusicClient.DefaultItemsPerPage;
            this.StartIndex = MusicClient.DefaultStartIndex;
        }

        /// <summary>
        /// Gets or sets the number of items to fetch.
        /// </summary>
        internal int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).
        /// </summary>
        internal int StartIndex { get; set; }

        /// <summary>
        /// Gets a json processor that can parse the response expected by this command.
        /// By default, a processor for a named item list is returned.
        /// </summary>
        internal virtual IJsonProcessor JsonProcessor
        {
            get { return new NamedItemListJsonProcessor(); }
        }

        /// <summary>
        /// Gets the callback to use allowing post-processing of data.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        internal Action<TResult> Callback
        {
            get
            {
                return (TResult result) =>
                    {
                        this.PostProcessResult(result);

                        if (this.ThrowOnError && result.Error != null)
                        {
                            throw result.Error;
                        }
                        else
                        {
                            this.ClientCallback(result);
                        }
                    };
            }
        }

        /// <summary>
        /// Gets or sets the callback for clients.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        internal Action<TResult> ClientCallback { get; set; }

        /// <summary>
        /// Determines whether we have a valid content type
        /// </summary>
        /// <param name="rawResult">The raw result.</param>
        /// <returns>
        ///   <c>true</c> if this is a valid content type; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// API will return application/json for error cases, so allow that as well as the custom type
        /// </remarks>
        internal bool IsValidContentType<TResponse>(Response<TResponse> rawResult)
        {
            return rawResult.Result != null &&
                   rawResult.ContentType != null &&
                   (rawResult.ContentType.StartsWith(MusicClientCommand.ContentTypeApiResponseStart, StringComparison.OrdinalIgnoreCase)
                   || rawResult.ContentType.StartsWith(MusicClientCommand.ContentTypeJson, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Allows simple post-processing of data retrieved
        /// </summary>
        /// <param name="result">The result object.</param>
        internal virtual void PostProcessResult(TResult result)
        {
            // default is to do nothing
        }

        /// <summary>
        /// Sets up a Task to invoke the command asynchronously.
        /// </summary>
        /// <returns>An async task</returns>
        internal Task<TResult> InvokeAsync()
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            this.Invoke((TResult result) => tcs.TrySetResult(result));
            return tcs.Task;
        }

        /// <summary>
        /// Generic response handler for content lists
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="rawResult">The response</param>
        /// <param name="itemsName">The json list name</param>
        /// <param name="converter">The object creation method to use</param>
        /// <param name="callback">The client callback</param>
        internal void ListItemResponseHandler<T>(Response<JObject> rawResult, string itemsName, JTokenConversionDelegate<T> converter, Action<ListResponse<T>> callback)
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
                        if (this.IsValidContentType(rawResult))
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
                        if (this.ClientSettings.CountryCodeBasedOnRegionInfo)
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
                var ex = new ApiCallFailedException(rawResult.StatusCode);
                response = new ListResponse<T>(rawResult.StatusCode, ex, rawResult.ErrorResponseBody, RequestId);
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
        internal void ItemResponseHandler<T>(Response<JObject> rawResult, JTokenConversionDelegate<T> converter, Action<Response<T>> callback)
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
                        if (this.IsValidContentType(rawResult))
                        {
                            T result = converter(rawResult.Result);
                            response = new Response<T>(rawResult.StatusCode, result, RequestId);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (this.ClientSettings.CountryCodeBasedOnRegionInfo)
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
                var ex = new ApiCallFailedException(rawResult.StatusCode);
                response = new Response<T>(rawResult.StatusCode, ex, rawResult.ErrorResponseBody, RequestId);
            }

            callback(response);
        }

        /// <summary>
        /// Creates an initial querystring dictionary containing paging parameters
        /// </summary>
        /// <returns>A dictionary containing standard querystring paging parameters</returns>
        internal List<KeyValuePair<string, string>> GetPagingParams()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(PagingStartIndex, this.StartIndex.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>(PagingItemsPerPage, this.ItemsPerPage.ToString(CultureInfo.InvariantCulture))
            };
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        private void Invoke(Action<TResult> callback)
        {
            this.ClientCallback = callback;
            this.Execute();
        }
    }
}