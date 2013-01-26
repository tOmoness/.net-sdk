// -----------------------------------------------------------------------
// <copyright file="ApiRequestHandler.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ionic.Zlib;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Implementation of the raw API interface for making requests
    /// </summary>
    internal class ApiRequestHandler : IApiRequestHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestHandler" /> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        public ApiRequestHandler(IApiUriBuilder uriBuilder)
        {
            this.UriBuilder = uriBuilder;
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
        /// <param name="method">The method to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="pathParams">The path params.</param>
        /// <param name="querystringParams">The querystring params.</param>
        /// <param name="callback">The callback to hit when done.</param>
        /// <param name="requestHeaders">HTTP headers to add to the request</param>
        /// <exception cref="System.ArgumentNullException">Thrown when no callback is specified</exception>
        public void SendRequestAsync(
                                     ApiMethod method,
                                     IMusicClientSettings settings,
                                     Dictionary<string, string> pathParams,
                                     Dictionary<string, string> querystringParams,
                                     Action<Response<JObject>> callback,
                                     Dictionary<string, string> requestHeaders = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            Uri uri = this.UriBuilder.BuildUri(method, settings, pathParams, querystringParams);

            DebugLogger.Instance.WriteLog("Calling {0}", uri.ToString());

            TimedRequest request = new TimedRequest(uri);
            this.AddRequestHeaders(request.WebRequest, requestHeaders);
            request.BeginGetResponse(
                (IAsyncResult ar) =>
                    {
                    if (request.HasTimedOut)
                    {
                        return;
                    }

                    request.Dispose();
                    WebResponse response = null;
                    HttpWebResponse webResponse = null;
                    JObject json = null;
                    HttpStatusCode? statusCode = null;
                    Exception error = null;

                    try
                    {
                        response = request.WebRequest.EndGetResponse(ar);
                        webResponse = response as HttpWebResponse;
                        if (webResponse != null)
                        {
                            statusCode = webResponse.StatusCode;
                        }
                    }
                    catch (WebException ex)
                    {
                        error = ex;
                        if (ex.Response != null)
                        {
                            webResponse = (HttpWebResponse)ex.Response;
                            statusCode = webResponse.StatusCode;
                        }
                    }

                    string contentType = null;
                    string result = null;

                    if (response != null)
                    {
                        contentType = response.ContentType;
                        try
                        {
                            using (Stream responseStream = GetResponseStream(response))
                            {
                                result = responseStream.AsString();
                                if (!string.IsNullOrEmpty(result))
                                {
                                    json = JObject.Parse(result);            
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            error = ex;
                            json = null;
                        }
                    }

                    DoCallback(callback, json, statusCode, contentType, error, method.RequestId, uri);
                },
                () => DoCallback(callback, null, null, null, new ApiCallFailedException(), method.RequestId, uri),
                request);
        }

        /// <summary>
        /// Logs the response and makes the callback
        /// </summary>
        /// <param name="callback">The callback method</param>
        /// <param name="json">The json response</param>
        /// <param name="statusCode">The response status code</param>
        /// <param name="contentType">The response content type</param>
        /// <param name="error">An error or null if successful</param>
        /// <param name="requestId">The unique id of this request</param>
        /// <param name="uri">The uri requested</param>
        private static void DoCallback(
                                       Action<Response<JObject>> callback,
                                       JObject json,
                                       HttpStatusCode? statusCode,
                                       string contentType,
                                       Exception error,
                                       Guid requestId,
                                       Uri uri)
        {            
            DebugLogger.Instance.WriteLog("{0} response from {1}", statusCode.HasValue ? statusCode.ToString() : "Timeout", uri.ToString());
            if (json != null)
            {
                callback(new Response<JObject>(statusCode, contentType, json, requestId));
            }
            else
            {
                DebugLogger.Instance.WriteLog("Error:{0}", error);
                callback(new Response<JObject>(statusCode, error, requestId));
            }
        }

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
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            }
            catch (Exception ex)
            {
                DebugLogger.Instance.WriteLog("Failed to add gzip header {0}", ex);
            }
        }
    }
}
