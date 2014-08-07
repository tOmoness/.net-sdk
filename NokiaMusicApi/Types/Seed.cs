// -----------------------------------------------------------------------
// <copyright file="Seed.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a mix data-seed
    /// </summary>
    public sealed partial class Seed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Seed"/> class.
        /// </summary>
        internal Seed()
        {
            this.Type = SeedType.Unknown;
        }

        /// <summary>
        /// Gets the id of the seed.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the seed.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the seed.
        /// </summary>
        public SeedType Type { get; private set; }

        /// <summary>
        /// Builds an seed from an artist Id
        /// </summary>
        /// <param name="artistId">
        /// The entity Id of the artist to use as a seed
        /// </param>
        /// <returns>
        /// The seed descriptor
        /// </returns>
        public static Seed FromArtistId(string artistId)
        {
            return new Seed { Type = SeedType.ArtistId, Id = artistId };
        }

        /// <summary>
        /// Builds an seed from an artist name
        /// </summary>
        /// <param name="artistName">
        /// The name of the artist to use as a seed
        /// </param>
        /// <returns>
        /// The seed descriptor
        /// </returns>
        public static Seed FromArtistName(string artistName)
        {
            return new Seed { Type = SeedType.ArtistName, Name = artistName };
        }

        /// <summary>
        /// Builds an seed from a mix Id
        /// </summary>
        /// <param name="mixId">
        /// The id of the mix to use as a seed
        /// </param>
        /// <returns>
        /// The seed descriptor
        /// </returns>
        public static Seed FromMixId(string mixId)
        {
            return new Seed { Type = SeedType.MixId, Id = mixId };
        }

        /// <summary>
        /// Builds an seed from a user Id
        /// </summary>
        /// <param name="userId">
        /// The Id of the user to use as a seed
        /// </param>
        /// <returns>
        /// The seed descriptor
        /// </returns>
        public static Seed FromUserId(string userId)
        {
            return new Seed { Type = SeedType.UserId, Id = userId };
        }

        /// <summary>
        /// Determines if two seeds are equal.
        /// </summary>
        /// <param name="seed1">The first seed/</param>
        /// <param name="seed2">The second seed.</param>
        /// <returns>True if the two seeds are equal.</returns>
        public static bool operator ==(Seed seed1, Seed seed2)
        {
            if (ReferenceEquals(seed1, null))
            {
                return ReferenceEquals(seed2, null);
            }

            return seed1.Equals(seed2);
        }

        /// <summary>
        /// Determines if two seeds are not equal.
        /// </summary>
        /// <param name="seed1">The first seed/</param>
        /// <param name="seed2">The second seed.</param>
        /// <returns>True if the two seeds are not equal.</returns>
        public static bool operator !=(Seed seed1, Seed seed2)
        {
            return !(seed1 == seed2);
        }

        /// <summary>
        /// Determines if two seeds are equal.
        /// </summary>
        /// <param name="obj">The other seed.</param>
        /// <returns>True if the two seeds are equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is Seed && this.Equals((Seed)obj);
        }

        /// <summary>
        /// Returns the hash code for this seed.
        /// </summary>
        /// <returns>A has code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int itemHash = 0;
                if (!string.IsNullOrEmpty(this.Id))
                {
                    itemHash = this.Id.GetHashCode();
                }
                else if (!string.IsNullOrEmpty(this.Name))
                {
                    itemHash = this.Name.GetHashCode();
                }

                return (itemHash * 397) ^ (int)this.Type;
            }
        }

        /// <summary>
        /// Produces a Json representation of this object
        /// </summary>
        /// <returns>The Json token</returns>
        public string ToJson()
        {
            var json = new JObject();

            switch (this.Type)
            {
                case SeedType.ArtistName:
                    json.Add("name", this.Name);
                    json.Add("type", "musicartist");
                    break;
                case SeedType.ArtistId:
                    json.Add("id", this.Id);
                    json.Add("type", "musicartist");
                    break;
                case SeedType.UserId:
                    json.Add("id", this.Id);
                    json.Add("type", "user");
                    break;
                case SeedType.MixId:
                    json.Add("id", this.Id);
                    json.Add("type", "mix");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("SeedType");
            }

            return json.ToString();
        }

        /// <summary>
        /// Given a Json representation of a seed, returns a parsed representation.
        /// </summary>
        /// <param name="json">
        /// The JToken representing the seed.
        /// </param>
        /// <returns>
        /// The seed descriptor.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the token cannot be parsed into a seed.
        /// </exception>
        internal static Seed FromJson(string json)
        {
            JObject token = JObject.Parse(json);

            var type = token.Value<string>("type");
            var id = token.Value<string>("id");
            var name = token.Value<string>("name");

            if (type != null)
            {
                switch (type)
                {
                    case "musicartist":
                        // We prefer the id version
                        if (!string.IsNullOrEmpty(id))
                        {
                            var seed = Seed.FromArtistId(token.Value<string>("id"));
                            
                            // use the name if we have one...
                            seed.Name = name;
                            return seed;
                        }
                        else if (!string.IsNullOrEmpty(token.Value<string>("name")))
                        {
                            return Seed.FromArtistName(token.Value<string>("name"));
                        }
                        else
                        {
                            throw new ArgumentException("token doesn't represent a known artist seed.");
                        }

                    case "user":
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            return Seed.FromUserId(id);
                        }

                        break;

                    case "mix":
                        // fall through to the id case
                        break;

                    default:
                        // Unknown official seed type
                        throw new ArgumentException("token doesn't represet a known seed type.");
                }
            }

            // If there is no "type", the default is a mix
            if (id != null)
            {
                return Seed.FromMixId(id);
            }

            throw new ArgumentException("Unknown seed type.");
        }

        private bool Equals(Seed other)
        {
            return !ReferenceEquals(other, null) && this.Type == other.Type &&
                ((!string.IsNullOrEmpty(this.Id) && string.Equals(this.Id, other.Id))
                || (!string.IsNullOrEmpty(this.Name) && string.Equals(this.Name, other.Name)));
        }
    }
}