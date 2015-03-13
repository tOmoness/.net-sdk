using Nokia.Music.Types;
using System;
using System.Collections.Generic;

namespace MixRadioActivity
{
	/// <summary>
	/// Class to hold the developer API keys
	/// </summary>
	/// <remarks>
	/// Register for your application keys at http://dev.mixrad.io
	/// You will receive a "Client Id" and "Client Secret" for each application, set the values below
	/// If you are using the User APIs on Windows 8, you will need to set your OAuthRedirectUri
	/// - if you have not edited the default Uri, then leave as the default below
	/// </remarks>
	/// <seealso cref="http://dev.mixrad.io/"/>
	public static class ApiKeys
	{
		/// <summary>
		/// Your API Credentials go here!
		/// </summary>
		public const string ClientId = "226c7b9c7f10ce414836cdaee6943f4b";
		public const string ClientSecret = "LSvcc/zWIgaCUbfEZXcLzPUnS0C2ohfJr4SYUYkx1A6fLrOJ5w2nVy0TaMjlDTDH";

		public const string OAuthAuthorizeUrl = "https://sapi.mixrad.io/1.x/authorize/";
		public const string OAuthRedirectUrl = "https://account.mixrad.io/authorize/complete";
		public const string OAuthTokenUrl = "https://sapi.mixrad.io/1.x/token/";

        public static Scope OAuthScope = Scope.ReadUserPlayHistory;

		/// <summary>
		/// Generates the scope param.
		/// </summary>
		/// <param name="scope">The scope.</param>
		/// <returns>A string representation of scope flags</returns>
		private static string AsStringParam(this Scope scope)
		{
			return string.Join(" ", AsStringParams(scope));
		}

		/// <summary>
		/// Generates the scope param.
		/// </summary>
		/// <param name="scope">The scope.</param>
		/// <returns>A string representation of scope flags</returns>
		private static string[] AsStringParams(this Scope scope)
		{
			Type scopeType = typeof(Scope);
			var allScopes = (Scope[])Enum.GetValues(scopeType);

			List<string> scopes = new List<string>();

			foreach (Scope targetScope in allScopes)
			{
				if (targetScope != Scope.None)
				{
					if ((scope & targetScope) == targetScope)
					{
					scopes.Add(ConvertScopeEnum(targetScope));
					}
				}
			}

			return scopes.ToArray();
		}

		/// <summary>
		/// Converts a scope enumeration value into a string representation.
		/// </summary>
		/// <param name="scope">The scope.</param>
		/// <returns>A string value</returns>
		private static string ConvertScopeEnum(Scope scope)
		{
			return scope.ToString().ToLowerInvariant()
				.Replace("read", "read_")
				.Replace("write", "write_")
				.Replace("usage", "usage_")
				.Replace("playmix", "play_mix")
				.Replace("download", "download_")
				.Replace("receive", "receive_");
		}
	}
}

