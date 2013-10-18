// -----------------------------------------------------------------------
// <copyright file="TaskBase.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
#endif
using Nokia.Music.Internal;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Base class for Nokia Music Tasks
    /// </summary>
    public class TaskBase
    {
        /// <summary>
        /// Launches the specified app to app URI if available or shows a web equivalent.
        /// </summary>
        /// <param name="appToAppUri">The app to app URI.</param>
        /// <param name="webFallbackUri">The web fallback URI.</param>
        [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule", Justification = "Adding the SecurityCritical attribute was over the top")]
        protected void Launch(Uri appToAppUri, Uri webFallbackUri)
        {
#if WINDOWS_PHONE || NETFX_CORE
#if WINDOWS_PHONE
            bool canLaunch = string.Equals(DeviceStatus.DeviceManufacturer, "NOKIA", StringComparison.OrdinalIgnoreCase);
#else
            bool canLaunch = true;
#endif
            if (canLaunch)
            {
                DebugLogger.Instance.WriteLog("Launching Nokia Music App with " + appToAppUri.ToString());
#pragma warning disable 4014  // Disable as we're launching the app - we don't want to wait
                Windows.System.Launcher.LaunchUriAsync(appToAppUri);
#pragma warning restore 4014  // CS4014
                return;
            }
#endif
#if WINDOWS_PHONE
            WebBrowserTask web = new WebBrowserTask();
            DebugLogger.Instance.WriteLog("Launching Nokia Music Web with " + webFallbackUri.ToString());
            web.Uri = webFallbackUri;
            web.Show();
#endif
        }
    }
}
