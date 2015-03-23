// -----------------------------------------------------------------------
// <copyright file="TaskBase.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
#endif

namespace MixRadio.Tasks
{
    /// <summary>
    /// Base class for MixRadio Tasks
    /// </summary>
    public class TaskBase
    {
        private string _clientId = null;

        /// <summary>
        /// Gets or sets the optional Client ID for passing through to MixRadio.
        /// </summary>
        /// <value>
        /// The client ID.
        /// </value>
        public string ClientId
        {
            get
            {
                return this._clientId;
            }

            set
            {
                this._clientId = value;
            }
        }

#pragma warning disable 1998  // Disable as we're launching the app - we don't want to wait
        /// <summary>
        /// Launches the specified app to app URI if available or shows a web equivalent.
        /// </summary>
        /// <param name="appToAppUri">The app to app URI.</param>
        /// <param name="webFallbackUri">The web fallback URI.</param>
        /// <returns>An async task to await</returns>
        [SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule", Justification = "Adding the SecurityCritical attribute was over the top")]
        protected async Task Launch(Uri appToAppUri, Uri webFallbackUri)
        {
#if WINDOWS_PHONE || NETFX_CORE
#if WINDOWS_PHONE
            bool canLaunch = string.Equals(DeviceStatus.DeviceManufacturer, "Nokia", StringComparison.OrdinalIgnoreCase) || string.Equals(DeviceStatus.DeviceManufacturer, "Microsoft", StringComparison.OrdinalIgnoreCase);
#else
            bool canLaunch = true;
#endif
            if (canLaunch)
            {
                // Append the clientId if one has been supplied...
                if (!string.IsNullOrEmpty(this.ClientId))
                {
                    if (appToAppUri.ToString().Contains("?"))
                    {
                        appToAppUri = new Uri(appToAppUri.ToString() + "&client_id=" + this.ClientId);
                    }
                    else
                    {
                        appToAppUri = new Uri(appToAppUri.ToString() + "?client_id=" + this.ClientId);
                    }
                }

                await Windows.System.Launcher.LaunchUriAsync(appToAppUri);
                return;
            }
#endif
#if WINDOWS_PHONE
            WebBrowserTask web = new WebBrowserTask();
            web.Uri = webFallbackUri;
            web.Show();
#endif
        }
#pragma warning restore 1998  // CS1998
    }
}
