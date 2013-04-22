// -----------------------------------------------------------------------
// <copyright file="Mix.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nokia.Music.Tasks;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Mix
    /// </summary>
    public sealed partial class Mix : MusicItem
    {
        internal const string AppToAppPlayUri = "nokia-music://play/mix/?id={0}";

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in Nokia Music
        /// </summary>
        public override Uri AppToAppUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(AppToAppPlayUri, this.Id));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mix has a parental advisory warning.
        /// </summary>
        /// <value>
        ///   <c>true</c> if parental advisory; otherwise, <c>false</c>.
        /// </value>
        public bool ParentalAdvisory { get; set; }

        /// <summary>
        /// Gets or sets the track count.
        /// </summary>
        /// <value>
        /// The track count.
        /// </value>
        public int TrackCount { get; set; }

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

            Uri square50 = null;
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square50, out square100, out square200, out square320);

            return new Mix()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
                TrackCount = item.Value<int>("numbertracks"),
                ParentalAdvisory = parentalAdvisory,
                Thumb50Uri = square50,
                Thumb100Uri = square100,
                Thumb200Uri = square200,
                Thumb320Uri = square320
            };
        }
    }
}
