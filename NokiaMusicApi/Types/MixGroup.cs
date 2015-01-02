// -----------------------------------------------------------------------
// <copyright file="MixGroup.cs" company="Nokia">
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
    /// Represents a mix group
    /// </summary>
    public sealed partial class MixGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MixGroup" /> class.
        /// </summary>
        public MixGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MixGroup" /> class with
        /// parameters.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public MixGroup(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the MixGroup id.
        /// </summary>
        /// <value>
        /// The MixGroup id.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the MixGroup name.
        /// </summary>
        /// <value>
        /// The MixGroup name.
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
            MixGroup target = obj as MixGroup;
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
        /// Creates a MixGroup from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A MixGroup object
        /// </returns>
        internal static MixGroup FromJToken(JToken item, IMusicClientSettings settings)
        {
            return new MixGroup()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name")
            };
        }
    }
}
