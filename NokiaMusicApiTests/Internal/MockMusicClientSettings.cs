// -----------------------------------------------------------------------
// <copyright file="MockMusicClientSettings.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tests.Internal
{
    internal class MockMusicClientSettings : IMusicClientSettings
    {
        internal MockMusicClientSettings(string appId, string appCode, string countryCode)
        {
            this.AppId = appId;
            this.AppCode = appCode;
            this.CountryCode = countryCode;
        }

        public string AppId { get; set; }

        public string AppCode { get; set; }

        public string CountryCode { get; set; }

        public bool CountryCodeBasedOnRegionInfo { get; set; }
    }
}
