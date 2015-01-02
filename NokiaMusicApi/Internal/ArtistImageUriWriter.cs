// -----------------------------------------------------------------------
// <copyright file="ArtistImageUriWriter.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace Nokia.Music.Internal
{
    internal class ArtistImageUriWriter
    {
        private IMusicClientSettings _settings;

        public ArtistImageUriWriter(IMusicClientSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("MusicClientSettings are required - this is usually only null in test scenarios circumventing MusicClient creation");
            }

            this._settings = settings;
        }

        /// <summary>
        /// Return an image uri based on the artist id
        /// </summary>
        /// <param name="artistId">Artist Name</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <returns>Image uri</returns>
        public Uri BuildForId(string artistId, int? width = null, int? height = null)
        {
            return this.CreateUri(string.Format("&id={0}", artistId), width, height);
        }

        /// <summary>
        /// Return an image uri based on the artist name
        /// </summary>
        /// <param name="artistName">Artist Name</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <returns>Image uri</returns>
        public Uri BuildForName(string artistName, int? width = null, int? height = null)
        {
            var encodedName = Uri.EscapeDataString(artistName);

            return this.CreateUri(string.Format("&name={0}", encodedName), width, height);
        }

        // TODO: replace with ArtistImageCommand to remove uri builder string below
        private Uri CreateUri(string selector, int? width, int? height)
        {
#if OPEN_INTERNALS
            const string ImageUri = "{0}1.x/{1}/creators/images/{2}/random/?domain=music&token={3}&lang={4}{5}";
#else
            const string ImageUri = "{0}1.x/{1}/creators/images/{2}/random/?domain=music&client_id={3}&lang={4}{5}";
#endif
            return new Uri(string.Format(ImageUri, this._settings.ApiBaseUrl, this._settings.CountryCode, this.GetSize(width, height), this._settings.ClientId, this._settings.Language, selector), UriKind.Absolute);
        }

        private string GetSize(int? width, int? height)
        {
            string size = "full";

            if (width.HasValue)
            {
                if (!height.HasValue)
                {
                    height = width;
                }

                size = string.Format(CultureInfo.InvariantCulture, "{0}x{1}", width, height);
            }

            return size;
        }
    }
}
