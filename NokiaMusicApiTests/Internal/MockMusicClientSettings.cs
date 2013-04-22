// -----------------------------------------------------------------------
// <copyright file="MockMusicClientSettings.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Internal;

namespace Nokia.Music.Tests.Internal
{
    internal class MockMusicClientSettings : IMusicClientSettings
    {
        internal MockMusicClientSettings(string appId, string countryCode)
        {
            this.AppId = appId;
            this.CountryCode = countryCode;
        }

        public string AppId { get; set; }

        public string CountryCode { get; set; }

        public bool CountryCodeBasedOnRegionInfo { get; set; }
    }
}
