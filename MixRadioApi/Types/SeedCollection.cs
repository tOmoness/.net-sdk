// -----------------------------------------------------------------------
// <copyright file="SeedCollection.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Collection of seeds
    /// </summary>
    public sealed partial class SeedCollection : IEnumerable<Seed>
    {
        private readonly List<Seed> _seeds;

        /// <summary>
        /// Collection of seeds with additional functionality
        /// </summary>
        /// <param name="seeds">
        /// Seeds to construct with
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if no seeds passed in
        /// </exception>
        public SeedCollection(params Seed[] seeds)
        {
            this._seeds = new List<Seed>();
            if (seeds != null)
            {
                this._seeds.AddRange(seeds);
            }
        }

        /// <summary>
        /// Gets the seed count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get
            {
                return this._seeds.Count;
            }
        }

        /// <summary>
        ///     Determines if the two collections are equal.
        /// </summary>
        /// <param name="seed1">The first seed collection.</param>
        /// <param name="seed2">The second seed collection.</param>
        /// <returns>True if the collections are equal.</returns>
        public static bool operator ==(SeedCollection seed1, SeedCollection seed2)
        {
            return Equals(seed1, seed2);
        }

        /// <summary>
        ///     Determines if the two collections are not equal.
        /// </summary>
        /// <param name="seed1">The first seed collection.</param>
        /// <param name="seed2">The second seed collection.</param>
        /// <returns>True if the collections aren't equal.</returns>
        public static bool operator !=(SeedCollection seed1, SeedCollection seed2)
        {
            return !Equals(seed1, seed2);
        }

        /// <summary>
        /// Determines if the seed collections are equal.
        /// </summary>
        /// <param name="obj">
        /// The target for comparison.
        /// </param>
        /// <returns>
        /// True if equal, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var target = obj as SeedCollection;

            return Equals(this, target);
        }

        /// <summary>
        ///     Gets the enumerator for private collection of seeds
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<Seed> GetEnumerator()
        {
            return this._seeds.GetEnumerator();
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hashed representation.</returns>
        public override int GetHashCode()
        {
            return this._seeds != null ? this._seeds.GetHashCode() : 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._seeds.GetEnumerator();
        }

        /// <summary>
        /// Get a seed collection from a string of seed json
        /// </summary>
        /// <param name="serialised">
        /// The json containing the seeds
        /// </param>
        /// <returns>
        /// Seed collection populated
        /// </returns>
        internal static SeedCollection FromJson(string serialised)
        {
            var parsedSeeds = new List<Seed>();

            JObject json = JObject.Parse(serialised);

            JToken idJToken = json.GetValue("id");

            if (idJToken != null)
            {
                Seed mixSeed = Seed.FromMixId(idJToken.Value<string>());
                parsedSeeds.Add(mixSeed);
            }

            JToken mixJToken = json.GetValue("mix");

            if (mixJToken != null)
            {
                Seed mixSeed = Seed.FromJson(mixJToken.ToString());
                parsedSeeds.Add(mixSeed);
            }

            JToken seedJToken = json.GetValue("seeds");

            if (seedJToken != null)
            {
                foreach (JToken token in seedJToken)
                {
                    Seed seed = Seed.FromJson(token.ToString());
                    parsedSeeds.Add(seed);
                }
            }

            return new SeedCollection(parsedSeeds.ToArray());
        }

        private static bool Equals(SeedCollection seed1, SeedCollection seed2)
        {
            if (ReferenceEquals(seed1, null))
            {
                return ReferenceEquals(seed2, null);
            }

            return !ReferenceEquals(seed2, null) && seed1.Count == seed2.Count && !seed1.Except(seed2).Any();
        }
    }
}