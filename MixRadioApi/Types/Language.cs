// -----------------------------------------------------------------------
// <copyright file="Language.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Internal;
using Newtonsoft.Json.Linq;

namespace MixRadio.Types
{
    /// <summary>
    /// Represents a MixRadio Language
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        /// <value>
        /// The language id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        /// <value>
        /// The language name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Creates a Genre from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A Genre object
        /// </returns>
        internal static Language FromJToken(JToken item, IMusicClientSettings settings)
        {
            return new Language()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
            };
        }
    }
}
