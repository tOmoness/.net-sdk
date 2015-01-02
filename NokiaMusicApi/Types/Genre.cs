// -----------------------------------------------------------------------
// <copyright file="Genre.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a MixRadio Genre
    /// </summary>
    public partial class Genre
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Genre" /> class.
        /// </summary>
        public Genre()
        {
        }

        /// <summary>
        /// Gets or sets the genre id.
        /// </summary>
        /// <value>
        /// The genre id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the genre name.
        /// </summary>
        /// <value>
        /// The genre name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the associated languages.
        /// </summary>
        /// <value>
        /// The associated languages.
        /// </value>
        public List<string> Languages { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Genre target = obj as Genre;
            if (target != null)
            {
                return string.Compare(target.Id, this.Id, StringComparison.OrdinalIgnoreCase) == 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (this.Id == null)
            {
                return base.GetHashCode();
            }

            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Creates a Genre from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A Genre object
        /// </returns>
        internal static Genre FromJToken(JToken item, IMusicClientSettings settings)
        {
            var languages = item.Value<JArray>("lang");

            return new Genre()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
                Languages = languages == null ? new List<string>() : languages.Select(x => x.Value<string>()).ToList()
            };
        }
    }
}
