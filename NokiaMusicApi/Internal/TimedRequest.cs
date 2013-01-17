// -----------------------------------------------------------------------
// <copyright file="TimedRequest.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading;

namespace Nokia.Music.Phone.Internal
{
    /// <summary>
    /// Time-out implementation for web requests
    /// </summary>
    internal sealed class TimedRequest : IDisposable
    {
        private static int _timeoutInMilliseconds = 60000;

        private Action _timeoutCallback;
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedRequest" /> class.
        /// </summary>
        /// <param name="uri">The uri used in the web request</param>
        internal TimedRequest(Uri uri)
        {
            WebRequest = WebRequest.Create(uri);
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
        /// Stops the timer
        /// </summary>
        public void Dispose()
        {
            if (this._timer != null)
            {
                this._timer.Dispose();
            }
        }

        /// <summary>
        /// Makes the request and starts the timer
        /// </summary>
        /// <param name="successCallback">The callback if the request is successful</param>
        /// <param name="timeoutCallback">The callback if the request times-out</param>
        /// <param name="state">The state object</param>
        internal void BeginGetResponse(AsyncCallback successCallback, Action timeoutCallback, object state)
        {
            this._timeoutCallback = timeoutCallback;
            this._timer = new Timer(this.TimeoutReached, null, _timeoutInMilliseconds, Timeout.Infinite);
            this.WebRequest.BeginGetResponse(successCallback, state);
        }

        /// <summary>
        /// Aborts the web request when the timeout is reached
        /// </summary>
        /// <param name="state">The state object</param>
        private void TimeoutReached(object state)
        {
            this.HasTimedOut = true;
            this._timer.Dispose();
            this.WebRequest.Abort();
            this._timeoutCallback();
        }
    }
}
