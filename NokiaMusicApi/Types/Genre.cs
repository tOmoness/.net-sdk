// -----------------------------------------------------------------------
// <copyright file="Genre.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Nokia Music Genre
    /// </summary>
    public sealed class Genre
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Genre" /> class.
        /// </summary>
        internal Genre()
        {
        }

        /// <summary>
        /// Gets the genre id.
        /// </summary>
        /// <value>
        /// The genre id.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the genre name.
        /// </summary>
        /// <value>
        /// The genre name.
        /// </value>
        public string Name { get; internal set; }

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
                return string.Compare(target.Id, this.Id, StringComparison.InvariantCultureIgnoreCase) == 0;
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
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// Creates a Genre from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Genre object</returns>
        internal static Genre FromJToken(JToken item)
        {
            return new Genre()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name")
            };
        }
    }
}
