// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Internal.Request
{
    /// <summary>
    /// Implementation of the raw API interface for making requests
    /// </summary>
#if OPEN_INTERNALS
        public
#else
    internal
#endif
 class ApiRequestHandler : IApiRequestHandler
    {
        private readonly string _userAgent;
        private readonly IHttpClientRequestProxy _requestProxy;

        private TimeSpan _serverTimeOffset = new TimeSpan(0);
        private bool _obtainedServerTime = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestHandler" /> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        /// <param name="userAgent">The UserAgent header to be sent in requests.</param>
        /// <param name="requestProxy">The proxy class responsible for making the request</param>
        public ApiRequestHandler(IApiUriBuilder uriBuilder, string userAgent = null, IHttpClientRequestProxy requestProxy = null)
        {
            this.UriBuilder = uriBuilder;
            this._userAgent = userAgent;
            this._requestProxy = requestProxy ?? new HttpClientRequestProxy();
        }

        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        public IApiUriBuilder UriBuilder { get; private set; }

        /// <summary>
        /// Gets the server UTC time.
        /// </summary>
        /// <value>
        /// The server UTC time.
        /// </value>
        public DateTime ServerTimeUtc
        {
            get
            {
                return DateTime.UtcNow.Add(this._serverTimeOffset);
            }
        }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The querystring.</param>
        /// <param name="rawDataHandler">The convertion handler for the data received.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A response for the API request.</returns>
        public async Task<Response<T>> SendRequestAsync<T>(
                                     MusicClientCommand command,
                                     IMusicClientSettings settings,
                                     List<KeyValuePair<string, string>> queryParams,
                                     Func<string, T> rawDataHandler,
                                     Dictionary<string, string> requestHeaders,
                                     CancellationToken? cancellationToken)
        {
            Uri uri = command.RawUri ?? this.UriBuilder.BuildUri(command, settings, queryParams);
            DebugLogger.Instance.WriteLog("Calling {0}", uri.ToString());

            HttpRequestMessage request = new HttpRequestMessage(command.HttpMethod, uri);
            HttpMessageHandler handler = this.CreateHandler(command.FollowHttpRedirects);

            this.AddRequestHeaders(request, requestHeaders, settings);
            BuildRequestBody(command, request);

            using (HttpClient client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromMilliseconds(MusicClient.RequestTimeout);
                HttpStatusCode? statusCode = null;
                bool? mixRadioHeaderFound = null;

                var activeCancellationToken = cancellationToken ?? CancellationToken.None;

                try
                {
                    activeCancellationToken.ThrowIfCancellationRequested();
                    using (HttpResponseMessage response = await this._requestProxy.SendRequestAsync(client, request, activeCancellationToken).ConfigureAwait(false))
                    {
                        statusCode = response.StatusCode;

                        if (command.ExpectsMixRadioHeader)
                        {
                            mixRadioHeaderFound = response.Headers.Contains("X-MixRadio");
                        }

                        var headers = new Dictionary<string, IEnumerable<string>>();

                        foreach (var header in response.Headers)
                        {
                            headers.Add(header.Key, header.Value.ToArray());
                        }

                        //// Capture Server Time offset if we haven't already...
                        this.DeriveServerTimeOffset(response.Headers.Date, response.Headers.Age);

                        command.SetAdditionalResponseInfo(new ResponseInfo(response.RequestMessage.RequestUri, headers));

                        using (var content = response.Content)
                        {
                            string contentType = content.Headers.ContentType != null ? content.Headers.ContentType.MediaType : null;
                            string responseBody = await content.ReadAsStringAsync().ConfigureAwait(false);

                            if (!response.IsSuccessStatusCode && !IsFake404(response))
                            {
                                DebugLogger.Instance.WriteException(
                                    new ApiCallFailedException(response.StatusCode),
                                    new KeyValuePair<string, string>("uri", uri.ToString()),
                                    new KeyValuePair<string, string>("errorResponseBody", responseBody),
                                    new KeyValuePair<string, string>("statusCode", statusCode.ToString()),
                                    new KeyValuePair<string, string>("mixRadioHeaderFound", mixRadioHeaderFound.HasValue ? mixRadioHeaderFound.ToString() : "unknown"));
                            }

                            T responseItem = rawDataHandler(responseBody);

                            return PrepareResponse(responseItem, statusCode, contentType, null, responseBody, command.RequestId, uri, mixRadioHeaderFound, IsFake404(response));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        if (activeCancellationToken.IsCancellationRequested)
                        {
                            DebugLogger.Instance.WriteLog("OperationCanceledException thrown due to operation being cancelled.");
                            return PrepareResponse(default(T), statusCode, null, new ApiCallCancelledException(), null, command.RequestId, uri, false);
                        }
                        else
                        {
                            DebugLogger.Instance.WriteLog("OperationCanceledException thrown without operation being cancelled.");

                            // Because Xamarin.Android fails on limited networks by cancelling the task.
                            return PrepareResponse(default(T), statusCode, null, new NetworkLimitedException(), null, command.RequestId, uri, false);
                        }
                    }

                    if (mixRadioHeaderFound.HasValue && !mixRadioHeaderFound.Value)
                    {
                        return PrepareResponse(default(T), statusCode, null, new NetworkLimitedException(), null, command.RequestId, uri, false);
                    }

                    // This is a way to check if an SSL certificate has failed.
                    // WebExceptionStatus.TrustFailure is not supported by PCL (http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx)
                    var webException = ex as WebException;
                    if (webException != null && webException.Status == WebExceptionStatus.SendFailure && !mixRadioHeaderFound.HasValue)
                    {
                        return PrepareResponse(default(T), statusCode, null, new SendFailureException(), null, command.RequestId, uri, false);
                    }

                    DebugLogger.Instance.WriteException(
                        new ApiCallFailedException(statusCode),
                        new KeyValuePair<string, string>("uri", uri.ToString()),
                        new KeyValuePair<string, string>("statusCode", statusCode.HasValue ? statusCode.ToString() : "Timeout"),
                        new KeyValuePair<string, string>("mixRadioHeaderFound", mixRadioHeaderFound.HasValue ? mixRadioHeaderFound.ToString() : "unknown"));

                    return PrepareResponse(default(T), statusCode, null, ex, null, command.RequestId, uri, mixRadioHeaderFound);
                }
            }
        }

        /// <summary>
        /// Derives the server time offset from the Date and Age headers.
        /// </summary>
        /// <param name="date">The response Date header value.</param>
        /// <param name="age">The response Age header value.</param>
#if OPEN_INTERNALS
        public
#else
        internal
#endif
        void DeriveServerTimeOffset(DateTimeOffset? date, TimeSpan? age)
        {
            if (this._obtainedServerTime)
            {
                return;
            }

            if (date.HasValue)
            {
                var serverTime = date.Value;

                if (age.HasValue)
                {
                    serverTime = serverTime.AddSeconds(age.Value.TotalSeconds);
                }

                this._serverTimeOffset = serverTime.Subtract(DateTimeOffset.Now);
                this._obtainedServerTime = true;
            }
        }

        /// <summary>
        /// Builds and gzips the request body if required
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="request">The request information</param>
        private static void BuildRequestBody(MusicClientCommand command, HttpRequestMessage request)
        {
            var requestBody = command.BuildRequestBody();

            if (requestBody != null)
            {
                var content = new StringContent(requestBody, Encoding.UTF8);
                if (command.GzipRequestBody)
                {
                    request.Content = new GzippedContent(content);
                    request.Content.Headers.Add("Content-Encoding", "gzip");
                }
                else
                {
                    request.Content = content;
                }

                if (!string.IsNullOrWhiteSpace(command.ContentType))
                {
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(command.ContentType);
                }
            }
        }

        /// <summary>
        /// Determines the difference between a 404 explicitly delivered by a service and a 404 response from WP while offline
        /// </summary>
        /// <param name="response">The web response</param>
        /// <returns>True if this 404 was provided by the WP platform without making a request</returns>
        private static bool IsFake404(HttpResponseMessage response)
        {
            return response != null && (response.RequestMessage.RequestUri == null || string.IsNullOrEmpty(response.RequestMessage.RequestUri.OriginalString) || !response.Headers.Any());
        }

        /// <summary>
        /// Logs the response and makes the callback
        /// </summary>
        /// <typeparam name="T">The type of response item</typeparam>
        /// <param name="response">The response item</param>
        /// <param name="statusCode">The response status code</param>
        /// <param name="contentType">The response content type</param>
        /// <param name="error">An error or null if successful</param>
        /// <param name="responseBody">The response body</param>
        /// <param name="requestId">The unique id of this request</param>
        /// <param name="uri">The uri requested</param>
        /// <param name="mixRadioHeaderFound">Indicates whether the X-MixRadio header was missing in the response</param>
        /// <param name="offlineNotFoundResponse">Indicates whether this is a 'fake' 404 that WP can provide while offline</param>
        /// <returns>A response</returns>
        private static Response<T> PrepareResponse<T>(
                                       T response,
                                       HttpStatusCode? statusCode,
                                       string contentType,
                                       Exception error,
                                       string responseBody,
                                       Guid requestId,
                                       Uri uri,
                                       bool? mixRadioHeaderFound,
                                       bool offlineNotFoundResponse = false)
        {
            DebugLogger.Instance.WriteLog("{0} response from {1}", statusCode.HasValue ? statusCode.ToString() : "Timeout", uri.ToString());

            if (response != null && !offlineNotFoundResponse)
            {
                return new Response<T>(statusCode, contentType, response, requestId, mixRadioHeaderFound);
            }

            error = offlineNotFoundResponse ? new NetworkUnavailableException() : error;

            if (error == null && mixRadioHeaderFound.HasValue && !mixRadioHeaderFound.Value)
            {
                error = new NetworkLimitedException();
            }

            return new Response<T>(statusCode, error, responseBody, requestId, mixRadioHeaderFound);
        }

        /// <summary>
        /// Creates an implementation of the abstract class HttpMessageHandler for use by the HttpClient.
        /// </summary>
        /// <param name="followHttpRedirects">If true, clients should automatically follow HTTP redirects; if false this will not be done.</param>
        /// <returns>An implementation of the HttpMessageHandler suitable for the current platform.</returns>
#if OPEN_INTERNALS
        protected virtual
#else
        private
#endif
 HttpMessageHandler CreateHandler(bool followHttpRedirects)
        {
            HttpClientHandler handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            handler.AllowAutoRedirect = followHttpRedirects;

            return handler;
        }

        private void AddRequestHeaders(HttpRequestMessage request, Dictionary<string, string> requestHeaders, IMusicClientSettings settings)
        {
            if (!string.IsNullOrEmpty(this._userAgent))
            {
                request.Headers.UserAgent.TryParseAdd(this._userAgent);
            }

            if (requestHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in requestHeaders)
                {
                    DebugLogger.Instance.WriteLog(" Request Header: {0} = {1}", header.Key, header.Value);
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
