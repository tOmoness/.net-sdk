// -----------------------------------------------------------------------
// <copyright file="OAuthResultParser.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Types;

namespace MixRadio
{
    /// <summary>
    /// Helper method(s) for obtaining OAuth Results
    /// <remarks>
    /// see http://tools.ietf.org/html/rfc6749#section-4.1.2.1
    /// </remarks>
    /// </summary>
    public static class OAuthResultParser
    {
        /// <summary>
        /// Parses the querystring for completed flags.
        /// </summary>
        /// <param name="querystring">The querystring.</param>
        /// <param name="resultCode">The result.</param>
        /// <param name="authorizationCode">The authorization code if one was returned.</param>
        public static void ParseQuerystringForCompletedFlags(string querystring, out AuthResultCode resultCode, out string authorizationCode)
        {
            authorizationCode = null;
            resultCode = AuthResultCode.Unknown;

            if (!string.IsNullOrEmpty(querystring))
            {
                string trimmedQuerystring = querystring;

                // Ensure we start from the "?"...
                int queryStart = querystring.IndexOf("?");
                if (queryStart > -1)
                {
                    trimmedQuerystring = querystring.Substring(queryStart + 1);
                }

                string[] queryParams = trimmedQuerystring.Split('&');

                foreach (string pair in queryParams)
                {
                    string[] keyValue = pair.Split('=');
                    if (keyValue.Length == 2 && !string.IsNullOrWhiteSpace(keyValue[1]))
                    {
                        string key = keyValue[0].ToLowerInvariant();

                        switch (key)
                        {
                            case "code":
                                authorizationCode = keyValue[1];
                                resultCode = AuthResultCode.Success;
                                break;

                            case "error":
                                authorizationCode = null;
                                resultCode = keyValue[1].ToLowerInvariant().ToAuthResultReason();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to convert an OAuth2 error code to a AuthResultCode value
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>
        /// A AuthResultCode representation of error code
        /// </returns>
        internal static AuthResultCode ToAuthResultReason(this string errorCode)
        {
            switch (errorCode)
            {
                case "access_denied":
                    return AuthResultCode.AccessDenied;

                case "unauthorized_client":
                    return AuthResultCode.UnauthorizedClient;

                case "invalid_scope":
                    return AuthResultCode.InvalidScope;

                case "server_error":
                    return AuthResultCode.ServerError;

                default:
                    return AuthResultCode.Unknown;
            }
        }
    }
}
