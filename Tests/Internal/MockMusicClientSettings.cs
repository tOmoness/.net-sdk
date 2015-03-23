// -----------------------------------------------------------------------
// <copyright file="MockMusicClientSettings.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Commands;
using MixRadio.Internal;

namespace MixRadio.Tests.Internal
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
