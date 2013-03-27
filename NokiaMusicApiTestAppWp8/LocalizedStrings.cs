// -----------------------------------------------------------------------
// <copyright file="LocalizedStrings.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.TestApp.Resources;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        /// <summary>
        /// Gets the localized string resources.
        /// </summary>
        public AppResources LocalizedResources
        {
            get
            {
                return _localizedResources;
            }
        }
    }
}