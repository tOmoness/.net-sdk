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
#else
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

namespace Nokia.Music.Internal.Request
{
    /// <summary>
    /// Time-out implementation for web requests
    /// </summary>
    internal sealed class TimedRequest : IDisposable
    {
        private static int _timeoutInMilliseconds = 30000;
        private Action _timeoutCallback;
#if NETFX_CORE
        private DispatcherTimer _timer;
#else
        private Timer _timer;
#endif

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
#if NETFX_CORE
        public async void Dispose()
#else
        public void Dispose()
#endif
        {
            if (this._timer != null)
            {
#if NETFX_CORE
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.High,
                    () =>
                        {
                            this._timer.Stop();
                        });
#else
                this._timer.Dispose();
#endif
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
#if NETFX_CORE
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds((double)_timeoutInMilliseconds);
            this._timer.Tick += this.TimeoutReached;
            this._timer.Start();
#else
            this._timer = new Timer(this.TimeoutReached, null, _timeoutInMilliseconds, Timeout.Infinite);
#endif
            this.WebRequest.BeginGetResponse(successCallback, state);
        }

        /// <summary>
        /// Aborts the web request when the timeout is reached
        /// </summary>
        /// <param name="state">The state object</param>
        private void TimeoutReached(object state)
        {
            DebugLogger.Instance.WriteLog("Request timed-out");
            this.HasTimedOut = true;
#if NETFX_CORE
            this._timer.Stop();
#else
            this._timer.Dispose();
#endif
            this._timer = null;
            this.WebRequest.Abort();
            this._timeoutCallback();
        }

#if NETFX_CORE
        /// <summary>
        /// Signals that the timeout has been reached
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TimeoutReached(object sender, object e)
        {
            this.TimeoutReached(null);
        }
#endif
    }
}
