// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ionic.Zlib;
using Nokia.Music.Phone.Commands;
using Nokia.Music.Phone.Internal.Response;

namespace Nokia.Music.Phone.Internal.Request
{
    /// <summary>
    /// Implementation of the raw API interface for making requests
    /// </summary>
    internal class ApiRequestHandler : IApiRequestHandler
    {
        private static bool _gzipEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestHandler" /> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        public ApiRequestHandler(IApiUriBuilder uriBuilder)
        {
            this.UriBuilder = uriBuilder;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the request should use gzip or not.
        /// </summary>
        /// <value>
        ///   <c>True</c> if the request should use gzip; otherwise, <c>false</c>.
        /// </value>
        public static bool GzipEnabled
        {
            get { return _gzipEnabled; }
            set { _gzipEnabled = value; }
        }

        /// <summary>
        /// Gets the URI builder that is being used.
        /// </summary>
        /// <value>
        /// The URI builder.
        /// </value>
        public IApiUriBuilder UriBuilder { get; private set; }

        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <typeparam name="T">The type of response item</typeparam>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="queryParams">The querystring.</param>
        /// <param name="callback">The callback to hit when done.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        /// <exception cref="System.ArgumentNullException">Thrown when no callback is specified</exception>
        public void SendRequestAsync<T>(
                                     MusicClientCommand command,
                                     IMusicClientSettings settings,
                                     List<KeyValuePair<string, string>> queryParams,
                                     IResponseCallback<T> callback,
                                     Dictionary<string, string> requestHeaders = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            Uri uri = this.UriBuilder.BuildUri(command, settings, queryParams);
            DebugLogger.Instance.WriteLog("Calling {0}", uri.ToString());

            TimedRequest request = new TimedRequest(uri);
            this.AddRequestHeaders(request.WebRequest, requestHeaders);
            this.TryBuildRequestBody(
                (IAsyncResult ar) =>
                {
                    if (request.HasTimedOut)
                    {
                        return;
                    }

                    request.Dispose();
                    WebResponse response = null;
                    HttpWebResponse webResponse = null;
                    T responseItem = default(T);
                    HttpStatusCode? statusCode = null;
                    Exception error = null;
                    string responseBody = null;

                    try
                    {
                        response = request.WebRequest.EndGetResponse(ar);
                        webResponse = response as HttpWebResponse;
                        if (webResponse != null)
                        {
                            command.SetAdditionalResponseInfo(new ResponseInfo(webResponse.ResponseUri, webResponse.Headers));
                            statusCode = webResponse.StatusCode;
                        }
                    }
                    catch (WebException ex)
                    {
                        error = ex;
                        if (ex.Response != null)
                        {
                            response = ex.Response;
                            webResponse = (HttpWebResponse)ex.Response;
                            statusCode = webResponse.StatusCode;
                        }
                    }

                    string contentType = null;

                    if (response != null)
                    {
                        contentType = response.ContentType;
                        try
                        {
                            using (Stream responseStream = GetResponseStream(response))
                            {
                                responseBody = responseStream.AsString();
                                if (error == null)
                                {
                                    responseItem = callback.ConvertFromRawResponse(responseBody);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            responseItem = default(T);
                        }
                    }

                    DoCallback(callback.Callback, responseItem, statusCode, contentType, error, responseBody, command.RequestId, uri);
                },
                () => DoCallback(callback.Callback, default(T), null, null, new ApiCallFailedException(), null, command.RequestId, uri),
                request,
                command);
        }

        /// <summary>
        /// Logs the response and makes the callback
        /// </summary>
        /// <typeparam name="T">The type of response item</typeparam>
        /// <param name="callback">The callback command</param>
        /// <param name="response">The response item</param>
        /// <param name="statusCode">The response status code</param>
        /// <param name="contentType">The response content type</param>
        /// <param name="error">An error or null if successful</param>
        /// <param name="responseBody">The response body</param>
        /// <param name="requestId">The unique id of this request</param>
        /// <param name="uri">The uri requested</param>
        private static void DoCallback<T>(
                                       Action<Response<T>> callback,
                                       T response,
                                       HttpStatusCode? statusCode,
                                       string contentType,
                                       Exception error,
                                       string responseBody,
                                       Guid requestId,
                                       Uri uri)
        {
            DebugLogger.Instance.WriteLog("{0} response from {1}", statusCode.HasValue ? statusCode.ToString() : "Timeout", uri.ToString());
            if (response != null)
            {
                callback(new Response<T>(statusCode, contentType, response, requestId));
            }
            else
            {
                DebugLogger.Instance.WriteLog("Error:{0}", error);
                callback(new Response<T>(statusCode, error, responseBody, requestId));
            }
        }

        private void TryBuildRequestBody(AsyncCallback requestSuccessCallback, Action requestTimeoutCallback, TimedRequest request, MusicClientCommand apiMethod)
        {
            var requestBody = apiMethod.BuildRequestBody();
            request.WebRequest.Method = apiMethod.HttpMethod.ToString().ToUpperInvariant();
            if (requestBody != null)
            {
                var requestState = new RequestState(request, requestBody, requestSuccessCallback, requestTimeoutCallback);
                request.WebRequest.ContentType = apiMethod.ContentType;
                request.WebRequest.BeginGetRequestStream(this.RequestStreamCallback, requestState);
            }
            else
            {
                // No request body, just make the request immediately
                request.BeginGetResponse(requestSuccessCallback, requestTimeoutCallback, request);
            }
        }

        /// <summary>
        /// Writes request data to the request stream
        /// </summary>
        /// <param name="ar">The async response</param>
        private void RequestStreamCallback(IAsyncResult ar)
        {
            var requestState = (RequestState)ar.AsyncState;
            try
            {
                Stream streamResponse = requestState.TimedRequest.WebRequest.EndGetRequestStream(ar);
                byte[] byteArray = Encoding.UTF8.GetBytes(requestState.RequestBody);
                streamResponse.Write(byteArray, 0, requestState.RequestBody.Length);
                streamResponse.Dispose();

                TimedRequest request = requestState.TimedRequest;
                request.BeginGetResponse(requestState.SuccessCallback, requestState.TimeoutCallback, request);
            }
            catch (WebException ex)
            {
                DebugLogger.Instance.WriteLog("WebException in RequestStreamCallback: {0}", ex);
                requestState.TimeoutCallback();
            }
        }

        /// <summary>
        /// Determines whether response is gzipped and decodes if necessary
        /// </summary>
        /// <param name="response">The web response</param>
        /// <returns>The response stream</returns>
        private Stream GetResponseStream(WebResponse response)
        {
            bool gzipped = false;
            if (response.ContentLength > 0 && response.Headers != null && response.Headers.Count > 0)
            {
                var headerEncoding = response.Headers["Content-Encoding"];
                gzipped = headerEncoding != null && headerEncoding.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) > -1;
            }

            return gzipped
                    ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
                    : response.GetResponseStream();
        }

        private void AddRequestHeaders(WebRequest request, Dictionary<string, string> requestHeaders)
        {
            if (requestHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in requestHeaders)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }

            try
            {
                if (GzipEnabled)
                {
                    request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance.WriteLog("Failed to add gzip header {0}", ex);
            }
        }

        private class RequestState
        {
            internal RequestState(TimedRequest request, string requestBody, AsyncCallback successCallback, Action timeoutCallback)
            {
                this.TimedRequest = request;
                this.RequestBody = requestBody;
                this.SuccessCallback = successCallback;
                this.TimeoutCallback = timeoutCallback;
            }

            internal TimedRequest TimedRequest { get; private set; }

            internal string RequestBody { get; private set; }

            internal AsyncCallback SuccessCallback { get; private set; }

            internal Action TimeoutCallback { get; private set; }
        }
    }
}
