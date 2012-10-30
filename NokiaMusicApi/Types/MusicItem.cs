// -----------------------------------------------------------------------
// <copyright file="MusicItem.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Nokia Music Catalog Item
    /// </summary>
    public abstract class MusicItem
    {
        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the item name.
        /// </summary>
        /// <value>
        /// The item name.
        /// </value>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the 100x100 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 100x100 thumbnail URI.
        /// </value>
        public Uri Thumb100Uri { get; internal set; }

        /// <summary>
        /// Gets the 200x200 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 200x200 thumbnail URI.
        /// </value>
        public Uri Thumb200Uri { get; internal set; }

        /// <summary>
        /// Gets the 320x320 thumbnail URI.
        /// </summary>
        /// <value>
        /// The 320x320 thumbnail URI.
        /// </value>
        public Uri Thumb320Uri { get; internal set; }

        /// <summary>
        /// Extracts the thumbnails from JSON.
        /// </summary>
        /// <param name="thumbnailsToken">The thumbnails token.</param>
        /// <param name="square100">The square100.</param>
        /// <param name="square200">The square200.</param>
        /// <param name="square320">The square320.</param>
        protected static void ExtractThumbs(JToken thumbnailsToken, out Uri square100, out Uri square200, out Uri square320)
        {
            square100 = null;
            square200 = null;
            square320 = null;

            if (thumbnailsToken != null)
            {
                if (thumbnailsToken["100x100"] != null)
                {
                    square100 = new Uri(thumbnailsToken.Value<string>("100x100"));
                }

                if (thumbnailsToken["200x200"] != null)
                {
                    square200 = new Uri(thumbnailsToken.Value<string>("200x200"));
                }

                if (thumbnailsToken["320x320"] != null)
                {
                    square320 = new Uri(thumbnailsToken.Value<string>("320x320"));
                }
            }
        }
    }
}
