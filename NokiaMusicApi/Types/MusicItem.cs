// -----------------------------------------------------------------------
// <copyright file="MusicItem.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Nokia MixRadio Catalog Item
    /// </summary>
    public abstract class MusicItem
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in Nokia MixRadio
        /// </summary>
        public abstract Uri AppToAppUri { get; }

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in Nokia MixRadio
        /// </summary>
        public abstract Uri WebUri { get; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        /// <value>
        /// The item name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the 50x50 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 50x50 thumbnail URI.
        /// </value>
        public Uri Thumb50Uri { get; set; }

        /// <summary>
        /// Gets or sets the 100x100 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 100x100 thumbnail URI.
        /// </value>
        public Uri Thumb100Uri { get; set; }

        /// <summary>
        /// Gets or sets the 200x200 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 200x200 thumbnail URI.
        /// </value>
        public Uri Thumb200Uri { get; set; }

        /// <summary>
        /// Gets or sets the 320x320 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 320x320 thumbnail URI.
        /// </value>
        public Uri Thumb320Uri { get; set; }

        /// <summary>
        /// Extracts the thumbnails from JSON.
        /// </summary>
        /// <param name="thumbnailsToken">The thumbnails token.</param>
        /// <param name="square50">The square50 uri.</param>
        /// <param name="square100">The square100 uri.</param>
        /// <param name="square200">The square200 uri.</param>
        /// <param name="square320">The square320 uri.</param>
        internal static void ExtractThumbs(JToken thumbnailsToken, out Uri square50, out Uri square100, out Uri square200, out Uri square320)
        {
            square50 = null;
            square100 = null;
            square200 = null;
            square320 = null;

            if (thumbnailsToken != null)
            {
                square50 = ExtractThumb(thumbnailsToken, "50x50");
                square100 = ExtractThumb(thumbnailsToken, "100x100");
                square200 = ExtractThumb(thumbnailsToken, "200x200");
                square320 = ExtractThumb(thumbnailsToken, "320x320");
            }
        }

        private static Uri ExtractThumb(JToken thumbnailsToken, string size)
        {
            Uri thumb = null;
            if (thumbnailsToken[size] != null)
            {
                var thumbAsString = thumbnailsToken.Value<string>(size);
                try
                {
                    thumb = new Uri(thumbAsString, UriKind.RelativeOrAbsolute);
                }
                catch (FormatException ex)
                {
                    DebugLogger.Instance.WriteLog("Thumbnail {0}: {1} could not be used as Uri. {2}", size, thumbAsString, ex);
                }
            }

            return thumb;
        }
    }
}
