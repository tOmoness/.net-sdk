// -----------------------------------------------------------------------
// <copyright file="MockMusicClientSettings.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Commands;
using Nokia.Music.Internal;

namespace Nokia.Music.Tests.Internal
{
    internal class MockMusicClientSettings : IMusicClientSettings
    {
        internal MockMusicClientSettings()
        {
        }

        internal MockMusicClientSettings(string clientId, string countryCode, string language)
        {
            this.ClientId = clientId;
            this.CountryCode = countryCode;
            this.Language = language;
            this.ApiBaseUrl = MusicClientCommand.DefaultBaseApiUri;
            this.SecureApiBaseUrl = MusicClientCommand.DefaultSecureBaseApiUri;
        }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CountryCode { get; set; }

        public bool CountryCodeBasedOnRegionInfo { get; set; }

        public string Language { get; set; }

        public string ApiBaseUrl { get; set; }
        
        public string SecureApiBaseUrl { get; set; }
    }
}
