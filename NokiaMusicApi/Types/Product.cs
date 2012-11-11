// -----------------------------------------------------------------------
// <copyright file="Product.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Tasks;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Nokia Music Product, i.e. Album, Single or Track
    /// </summary>
    public sealed class Product : MusicItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product" /> class.
        /// </summary>
        internal Product()
        {
        }

        /// <summary>
        /// Gets the product's category.
        /// </summary>
        /// <value>
        /// The product's category.
        /// </value>
        public Category Category { get; private set; }

        /// <summary>
        /// Gets the product's genres.
        /// </summary>
        /// <value>
        /// The product's genres.
        /// </value>
        public Genre[] Genres { get; private set; }

        /// <summary>
        /// Gets the product's performers.
        /// </summary>
        /// <value>
        /// The product's performers.
        /// </value>
        public Artist[] Performers { get; private set; }

        /// <summary>
        /// Gets the product's price when available to purchase.
        /// </summary>
        /// <value>
        /// The price when available to purchase.
        /// </value>
        public Price Price { get; private set; }

        /// <summary>
        /// Gets the track count for Album or Single products.
        /// </summary>
        /// <value>
        /// The track count.
        /// </value>
        public int? TrackCount { get; private set; }

        /// <summary>
        /// Gets the Album or Single a Track is from.
        /// </summary>
        /// <value>
        /// The owning Album or Single if appropriate.
        /// </value>
        public Product TakenFrom { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Product target = obj as Product;
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
        /// Launches Nokia Music to show details about the product using the ShowProductTask
        /// </summary>
        public void Show()
        {
            ShowProductTask task = new ShowProductTask() { ProductId = this.Id };
            task.Show();
        }

        /// <summary>
        /// Creates a Product from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Product object</returns>
        internal static Product FromJToken(JToken item)
        {
            // Extract category...
            Category category = Category.Unknown;
            JToken jsonCategory = item["category"];
            if (jsonCategory != null)
            {
                category = CategoryExtensions.ParseCategory(jsonCategory.Value<string>("id"));
            }

            // Extract genres...
            Genre[] genres = null;
            JArray jsonGenres = item.Value<JArray>("genres");
            if (jsonGenres != null)
            {
                List<Genre> list = new List<Genre>();
                foreach (JToken jsonGenre in jsonGenres)
                {
                    list.Add((Genre)Genre.FromJToken(jsonGenre));
                }

                genres = list.ToArray();
            }

            // Extract takenfrom... 
            Product takenFrom = null;
            JToken jsonTakenFrom = item["takenfrom"];
            if (jsonTakenFrom != null)
            {
                takenFrom = (Product)Product.FromJToken(jsonTakenFrom);
            }

            // Extract price...
            Price price = null;
            JToken jsonPrices = item["prices"];
            if (jsonPrices != null)
            {
                JToken jsonPermDownload = jsonPrices["permanentdownload"];
                if (jsonPermDownload != null)
                {
                    price = Price.FromJToken(jsonPermDownload);
                }
            }

            // Extract Artists...
            Artist[] performers = null;

            if (item["creators"] != null)
            {
                JArray jsonArtists = item["creators"].Value<JArray>("performers");
                if (jsonArtists != null)
                {
                    List<Artist> list = new List<Artist>();
                    foreach (JToken jsonArtist in jsonArtists)
                    {
                        list.Add((Artist)Artist.FromJToken(jsonArtist));
                    }

                    performers = list.ToArray();
                }
            }

            // Extract trackcount... 
            int? trackCount = null;
            JToken jsonTrackCount = item["trackcount"];
            if (jsonTrackCount != null)
            {
                trackCount = item.Value<int>("trackcount");
            }

            // Extract thumbnails...
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square100, out square200, out square320);

            // Create the resulting Product object...
            return new Product()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
                Thumb100Uri = square100,
                Thumb200Uri = square200,
                Thumb320Uri = square320,
                Category = category,
                Genres = genres,
                TakenFrom = takenFrom,
                Price = price,
                TrackCount = trackCount,
                Performers = performers
            };
        }
    }
}
