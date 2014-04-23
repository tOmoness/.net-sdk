// -----------------------------------------------------------------------
// <copyright file="ApiKeys.cs" company="NOKIA">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAndMix
{
    /// <summary>
    /// Class to hold the developer API keys
    /// </summary>
    /// <remarks>
    /// Register for your application keys at http://nokia.ly/musicapireg
    /// You will receive a "Client Id" and "Client Secret" for each application, set the values below
    /// If you are using the User APIs on Windows 8, you will need to set your OAuthRedirectUri
    /// - if you have not edited the default Uri, then leave as the default below
    /// </remarks>
    /// <seealso cref="http://nokia.ly/musicapireg"/>
    internal static class ApiKeys
    {
        /// <summary>
        /// Your API Credentials go here!
        /// </summary>
        public const string ClientId = null;
        public const string ClientSecret = null;
        public const string OAuthRedirectUri = "https://account.mixrad.io/authorize/complete";
    }
}
