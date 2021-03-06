// -----------------------------------------------------------------------
// <copyright file="ApiKeys.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio.TestApp
{
    /// <summary>
    /// Class to hold the developer API keys
    /// </summary>
    /// <remarks>
    /// Register for your application keys at https://account.mixrad.io/developer
    /// You will receive a "Client Id" and "Client Secret" for each application, set the values below
    /// If you are using the User APIs on Windows 8, you will need to set your OAuthRedirectUri
    /// - if you have not edited the default Uri, then leave as the default below
    /// </remarks>
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
