﻿// -----------------------------------------------------------------------
// <copyright file="Mix.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MixRadio.Internal;
using Newtonsoft.Json.Linq;

namespace MixRadio.Types
{
    /// <summary>
    /// Represents a Mix
    /// </summary>
    public sealed partial class Mix : MusicItem
    {
        internal const string AppToAppPlayUri = "mixradio://play/mix/{0}";
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
        /// Gets or sets the description text for the mix.
        /// </summary>
        /// <value>
        /// A description of the mix, if present.
        /// </value>
        public string Description { get; set; }

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
        /// Gets or sets the featured artists
        /// </summary>
        /// <value>
        /// The featured artists
        /// </value>
        public List<Artist> FeaturedArtists { get; set; }

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

            JToken featuredArtists = item["featuredartists"];
            var featuredArtistsList = new List<Artist>();
            if (featuredArtists != null)
            {
                foreach (JToken token in featuredArtists)
                {
                    Artist artist = Artist.FromJToken(token, settings);
                    featuredArtistsList.Add(artist);
                }
            }

            Uri square50 = null;
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;
            Uri square640 = null;

            var thumbnailsToken = item["thumbnails"];

            MusicItem.ExtractThumbs(thumbnailsToken, out square50, out square100, out square200, out square320);

            if (thumbnailsToken != null)
            {
                square640 = Mix.ExtractThumb(thumbnailsToken, "640x640");
            }

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

            var description = item.Value<string>("description");

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

                    if (square640 == null)
                    {
                        square640 = new Uri(string.Format(PlayMeThumbUri, 640));
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

                        name = names.Length > 0
                                ? string.Join(", ", names)
                                : "Artist Mix";
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

                        if (square640 == null)
                        {
                            square640 = builder.BuildForId(idSeed.Id, 640);
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
                Thumb320Uri = square320,
                Thumb640Uri = square640,
                Description = description,
                FeaturedArtists = featuredArtistsList
            };
        }
    }
}
