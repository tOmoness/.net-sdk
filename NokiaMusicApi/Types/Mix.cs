// -----------------------------------------------------------------------
// <copyright file="Mix.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Tasks;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Mix
    /// </summary>
    public sealed class Mix : MusicItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mix" /> class.
        /// </summary>
        internal Mix()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the mix has a parental advisory warning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if parental advisory; otherwise, <c>false</c>.
        /// </value>
        public bool ParentalAdvisory { get; internal set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Mix target = obj as Mix;
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
        /// Launches Nokia Music to start playback of the mix using the PlayMixTask
        /// </summary>
        public void Play()
        {
            PlayMixTask task = new PlayMixTask() { MixId = this.Id };
            task.Show();
        }

        /// <summary>
        /// Creates a Mix from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Mix object</returns>
        internal static Mix FromJToken(JToken item)
        {
            bool parentalAdvisory = false;
            JToken parentaladvisoryToken = item["parentaladvisory"];
            if (parentaladvisoryToken != null)
            {
                parentalAdvisory = item.Value<bool>("parentaladvisory");
            }

            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square100, out square200, out square320);

            return new Mix()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
                ParentalAdvisory = parentalAdvisory,
                Thumb100Uri = square100,
                Thumb200Uri = square200,
                Thumb320Uri = square320
            };
        }
    }
}
