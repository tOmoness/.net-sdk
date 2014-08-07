// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand{TIntermediate,TResult}.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Parsing;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    /// <typeparam name="TIntermediate">The type of the intermediate object.</typeparam>
    /// <typeparam name="TResult">The type of the returned object.</typeparam>
    internal abstract class MusicClientCommand<TIntermediate, TResult> : MusicClientCommand<TIntermediate>
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
        /// Determines whether we have a valid content type
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
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
        /// Generic response handler for content lists
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="rawResult">The response</param>
        /// <param name="itemsName">The json list name</param>
        /// <param name="converter">The object creation method to use</param>
        /// <returns>
        /// A response
        /// </returns>
        internal ListResponse<T> ListItemResponseHandler<T>(Response<JObject> rawResult, string itemsName, JTokenConversionDelegate<T> converter)
        {
            // Parse the result if we got one...
            if (rawResult.Succeeded && rawResult.StatusCode.HasValue)
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
                            List<T> results = this.JsonProcessor.ParseList(rawResult.Result, itemsName, converter, this.ClientSettings);
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

                            return new ListResponse<T>(rawResult.StatusCode, results, startIndex, itemsPerPage, totalResults, RequestId);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (this.ClientSettings.CountryCodeBasedOnRegionInfo)
                        {
                            return new ListResponse<T>(rawResult.StatusCode, new ApiNotAvailableException(), rawResult.ErrorResponseBody, RequestId);
                        }

                        break;

                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        return new ListResponse<T>(rawResult.StatusCode, new InvalidApiCredentialsException(), rawResult.ErrorResponseBody, RequestId);
                }
            }

            return this.ListItemErrorResponseHandler<T>(rawResult);
        }

        /// <summary>
        /// Generic response handler for single item content
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="rawResult">The response</param>
        /// <param name="converter">The object creation method to use</param>
        /// <returns>
        /// A response
        /// </returns>
        internal Response<T> ItemResponseHandler<T>(Response<JObject> rawResult, JTokenConversionDelegate<T> converter)
        {
            // Parse the result if we got one...
            if (rawResult.Succeeded && rawResult.StatusCode.HasValue)
            {
                switch (rawResult.StatusCode.Value)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                        if (this.IsValidContentType(rawResult))
                        {
                            T result = converter(rawResult.Result, this.ClientSettings);
                            return new Response<T>(rawResult.StatusCode, result, RequestId);
                        }

                        break;

                    case HttpStatusCode.NotFound:
                        if (this.ClientSettings.CountryCodeBasedOnRegionInfo)
                        {
                            return new Response<T>(rawResult.StatusCode, new ApiNotAvailableException(), rawResult.ErrorResponseBody, RequestId);
                        }

                        break;

                    case HttpStatusCode.Forbidden:
                        return new Response<T>(rawResult.StatusCode, new InvalidApiCredentialsException(), rawResult.ErrorResponseBody, RequestId);
                }
            }

            return this.ItemErrorResponseHandler<T>(rawResult);
        }

        /// <summary>
        /// Returns a new <see cref="Response"/> with an error set.
        /// </summary>
        /// <param name="rawResult">The response</param>
        /// <returns>A Response.</returns>
        internal Response ErrorResponseHandler(Response rawResult)
        {
            if (rawResult.Error is NokiaMusicException)
            {
                return new Response(rawResult.StatusCode, rawResult.Error, rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
            }

            if (rawResult.FoundMixRadioHeader.HasValue && !rawResult.FoundMixRadioHeader.Value)
            {
                return new Response(rawResult.StatusCode, new NetworkLimitedException(), rawResult.ErrorResponseBody, rawResult.RequestId, false);
            }

            return new Response(rawResult.StatusCode, new ApiCallFailedException(rawResult.StatusCode), rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
        }

        /// <summary>
        /// Returns a new <see cref="Response{T}"/> with an error set.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="rawResult">The response</param>
        /// <returns>A Response.</returns>
        internal Response<T> ItemErrorResponseHandler<T>(Response rawResult)
        {
            if (rawResult.Error is NokiaMusicException)
            {
                return new Response<T>(rawResult.StatusCode, rawResult.Error, rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
            }

            if (rawResult.FoundMixRadioHeader.HasValue && !rawResult.FoundMixRadioHeader.Value)
            {
                return new Response<T>(rawResult.StatusCode, new NetworkLimitedException(), rawResult.ErrorResponseBody, rawResult.RequestId, false);
            }

            return new Response<T>(rawResult.StatusCode, new ApiCallFailedException(rawResult.StatusCode), rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
        }

        /// <summary>
        /// Returns a new <see cref="ListResponse{T}"/> with an error set.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="rawResult">The response</param>
        /// <returns>A Response.</returns>
        internal ListResponse<T> ListItemErrorResponseHandler<T>(Response rawResult)
        {
            if (rawResult.Error is NokiaMusicException)
            {
                return new ListResponse<T>(rawResult.StatusCode, rawResult.Error, rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
            }

            if (rawResult.FoundMixRadioHeader.HasValue && !rawResult.FoundMixRadioHeader.Value)
            {
                return new ListResponse<T>(rawResult.StatusCode, new NetworkLimitedException(), rawResult.ErrorResponseBody, rawResult.RequestId, false);
            }

            return new ListResponse<T>(rawResult.StatusCode, new ApiCallFailedException(rawResult.StatusCode), rawResult.ErrorResponseBody, rawResult.RequestId, rawResult.FoundMixRadioHeader);
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

        internal async Task<TResult> ExecuteAsync(CancellationToken? cancellationToken)
        {
            var rawResponse = await RequestHandler.SendRequestAsync(
                this,
                this.ClientSettings,
                this.BuildQueryStringParams(),
                this.HandleRawData,
                await this.BuildRequestHeadersAsync(),
                cancellationToken).ConfigureAwait(false);

            var response = this.HandleRawResponse(rawResponse);

            return response;
        }

        internal abstract TResult HandleRawResponse(Response<TIntermediate> rawResponse);
    }
}