// -----------------------------------------------------------------------
// <copyright file="TimedRequest.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
#if !NETFX_CORE
using System.Threading;
#endif
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

namespace Nokia.Music.Internal.Request
{
    /// <summary>
    /// Time-out implementation for web requests
    /// </summary>
    internal sealed class TimedRequest
    {
        private static int _timeoutInMilliseconds = 30000;
        private Action _timeoutCallback;
        private bool _requestCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedRequest" /> class.
        /// </summary>
        /// <param name="uri">The uri used in the web request</param>
        internal TimedRequest(Uri uri)
        {
            this.WebRequest = WebRequest.Create(uri);
        }

        /// <summary>
        /// Gets or sets the request timeout duration in milliseconds
        /// </summary>
        internal static int RequestTimeout
        {
            get { return _timeoutInMilliseconds; }
            set { _timeoutInMilliseconds = value; }
        }

        internal WebRequest WebRequest { get; private set; }

        internal bool HasTimedOut { get; private set; }

        /// <summary>
        /// Makes the request and starts the timer
        /// </summary>
        /// <param name="successCallback">The callback if the request is successful</param>
        /// <param name="timeoutCallback">The callback if the request times-out</param>
        /// <param name="state">The state object</param>
        internal void BeginGetResponse(AsyncCallback successCallback, Action timeoutCallback, object state)
        {
            this._timeoutCallback = timeoutCallback;
            Task.Factory.StartNew(async () => await this.BeginAsyncTimer());

            this.WebRequest.BeginGetResponse(
                ar =>
                {
                    this._requestCompleted = true;
                    successCallback(ar);
                },
                state);
        }

        private async Task BeginAsyncTimer()
        {
            await Task.Delay(_timeoutInMilliseconds);

            if (!this._requestCompleted)
            {
                this.TimeoutReached(this);
            }
        }

        /// <summary>
        /// Aborts the web request when the timeout is reached
        /// </summary>
        /// <param name="state">The state object</param>
        private void TimeoutReached(object state)
        {
            DebugLogger.Instance.WriteLog("Request timed-out");
            this.HasTimedOut = true;

            this.WebRequest.Abort();
            this._timeoutCallback();
        }
    }
}
