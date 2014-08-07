// -----------------------------------------------------------------------
// <copyright file="Mix.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
#if !PORTABLE
using System.Threading.Tasks;
#endif
using Newtonsoft.Json.Linq;
using Nokia.Music.Internal;
#if !PORTABLE
using Nokia.Music.Tasks;
#endif

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Mix
    /// </summary>
    public sealed partial class Mix : MusicItem
    {
#if WINDOWS_APP
        internal const string AppToAppPlayUri = "nokia-music://play/mix/?id={0}";
#else
        internal const string AppToAppPlayUri = "mixradio://play/mix/{0}";
#endif
        internal const string WebPlayUri = "http://www.mixrad.io/mixes/{0}";

        /// <summary>
        /// Gets the app-to-app uri to use to play this item in MixRadio
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
        /// Gets the web uri to use to play this item in MixRadio on the web
        /// </summary>
        public override Uri WebUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(WebPlayUri, this.Id));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mix id.
        /// </summary>
        /// <value>
        /// The Mix id.
        /// </value>
        public override string Id
        {
            get
            {
                var seeds = this.Seeds;

                if (seeds != null)
                {
                    var mixIdSeed = seeds.FirstOrDefault(x => x.Type == SeedType.MixId);

                    if (mixIdSeed != null)
                    {
                        return mixIdSeed.Id;
                    }
                }

                return null;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.Seeds = new SeedCollection(Seed.FromMixId(value));
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
        /// Gets or sets the mix seeds.
        /// </summary>
        public SeedCollection Seeds { get; set; }

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

#if !PORTABLE
        /// <summary>
        /// Launches MixRadio to start playback of the mix using the PlayMixTask
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Play()
        {
            if (!string.IsNullOrEmpty(this.Id))
            {
                PlayMixTask task = new PlayMixTask() { MixId = this.Id };
                await task.Show().ConfigureAwait(false);
                return;
            }
#if WINDOWS_PHONE
            else if (this.Seeds.Where(s => s.Type == SeedType.UserId).Count() > 0)
            {
                await new PlayMeTask().Show().ConfigureAwait(false);
                return;
            }
#endif

            if (this.Seeds != null)
            {
                var artistSeeds = this.Seeds.Where(s => (s.Type == SeedType.ArtistId || s.Type == SeedType.ArtistName));

                // for now, just take the first artist name - need to support multiple soon though
                var name = artistSeeds.Select(s => s.Name).Where(s => !string.IsNullOrEmpty(s)).FirstOrDefault();
                if (!string.IsNullOrEmpty(name))
                {
                    await new PlayMixTask() { ArtistName = name }.Show().ConfigureAwait(false);
                    return;
                }
            }

            throw new InvalidOperationException();
        }

#endif
        /// <summary>
        /// Creates a Mix from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        /// A Mix object
        /// </returns>
        internal static Mix FromJToken(JToken item, IMusicClientSettings settings)
        {
            const string PlayMeThumbUri = "http://dev.mixrad.io/assets/playme/{0}x{0}.png";

            if (item == null)
            {
                return null;
            }

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

            var seeds = item["seeds"];
            SeedCollection seedCollection = null;
            if (seeds != null)
            {
                seedCollection = SeedCollection.FromJson(item.ToString());
            }
            else
            {
                var mixId = item.Value<string>("id");

                if (!string.IsNullOrEmpty(mixId))
                {
                    seedCollection = new SeedCollection(Seed.FromMixId(mixId));
                }
            }

            var name = item.Value<string>("name");

            if (seedCollection != null && seedCollection.Count > 0)
            {
                if (seedCollection.Count(s => s.Type == SeedType.UserId) > 0)
                {
                    if (square50 == null)
                    {
                        square50 = new Uri(string.Format(PlayMeThumbUri, 50));
                    }

                    if (square100 == null)
                    {
                        square100 = new Uri(string.Format(PlayMeThumbUri, 100));
                    }

                    if (square200 == null)
                    {
                        square200 = new Uri(string.Format(PlayMeThumbUri, 200));
                    }

                    if (square320 == null)
                    {
                        square320 = new Uri(string.Format(PlayMeThumbUri, 320));
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "Play Me";
                    }
                }
                else if (seedCollection.Count(s => s.Type == SeedType.ArtistId) > 0)
                {
                    var artistSeeds = seedCollection.Where(s => (s.Type == SeedType.ArtistId)).ToArray();

                    if (string.IsNullOrEmpty(name))
                    {
                        // Derive a name
                        var names = artistSeeds.Select(s => s.Name).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        if (names.Length > 0)
                        {
                            name = string.Join(", ", names);
                        }
                        else
                        {
                            name = "Artist Mix";
                        }
                    }

                    // Derive a thumbnail image
                    var idSeed = artistSeeds.FirstOrDefault(s => !string.IsNullOrEmpty(s.Id));
                    if (idSeed != null && settings != null)
                    {
                        var builder = new ArtistImageUriWriter(settings);

                        if (square50 == null)
                        {
                            square50 = builder.BuildForId(idSeed.Id, 50);
                        }

                        if (square100 == null)
                        {
                            square100 = builder.BuildForId(idSeed.Id, 100);
                        }

                        if (square200 == null)
                        {
                            square200 = builder.BuildForId(idSeed.Id, 200);
                        }

                        if (square320 == null)
                        {
                            square320 = builder.BuildForId(idSeed.Id, 320);
                        }
                    }
                }
            }

            return new Mix()
            {
                Name = name,
                TrackCount = item.Value<int>("numbertracks"),
                ParentalAdvisory = parentalAdvisory,
                Seeds = seedCollection,
                Thumb50Uri = square50,
                Thumb100Uri = square100,
                Thumb200Uri = square200,
                Thumb320Uri = square320
            };
        }
    }
}
