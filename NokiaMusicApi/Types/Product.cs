// -----------------------------------------------------------------------
// <copyright file="Product.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Internal.Parsing;
#if !PORTABLE
using Nokia.Music.Tasks;
#endif

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Nokia MixRadio Product, i.e. Album, Single or Track
    /// </summary>
    public sealed partial class Product : MusicItem
    {
        internal const string AppToAppShowUri = "nokia-music://show/product/?id={0}";
        internal const string WebShowUri = "http://www.mixrad.io/product/{0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Product" /> class.
        /// </summary>
        public Product()
        {
            this.Tracks = new List<Product>();
        }

        /// <summary>
        /// Gets the app-to-app uri to use to show this item in Nokia MixRadio
        /// </summary>
        public override Uri AppToAppUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(AppToAppShowUri, this.Id));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the web uri to use to show this item in Nokia MixRadio on the web
        /// </summary>
        public override Uri WebUri
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Id))
                {
                    return new Uri(string.Format(WebShowUri, this.Id));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the product's category.
        /// </summary>
        /// <value>
        /// The product's category.
        /// </value>
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the product's genres.
        /// </summary>
        /// <value>
        /// The product's genres.
        /// </value>
        public Genre[] Genres { get; set; }

        /// <summary>
        /// Gets or sets the product's performers.
        /// </summary>
        /// <value>
        /// The product's performers.
        /// </value>
        public Artist[] Performers { get; set; }

        /// <summary>
        /// Gets or sets the product's price when available to purchase.
        /// </summary>
        /// <value>
        /// The price when available to purchase.
        /// </value>
        public Price Price { get; set; }

        /// <summary>
        /// Gets or sets the track count for Album or Single products.
        /// </summary>
        /// <value>
        /// The track count.
        /// </value>
        public int? TrackCount { get; set; }

        /// <summary>
        /// Gets or sets the Album or Single a Track is from.
        /// </summary>
        /// <value>
        /// The owning Album or Single if appropriate.
        /// </value>
        public Product TakenFrom { get; set; }

        /// <summary>
        /// Gets or sets the tracks on the album.
        /// </summary>
        /// <value>
        /// The tracks.
        /// </value>
        public List<Product> Tracks { get; set; }

        /// <summary>
        /// Gets or sets the product's duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int? Duration { get; set; }

        /// <summary>
        /// Gets or sets the tracknumber of a local track if available.
        /// </summary>
        /// <value>
        /// The tracknumber.
        /// </value>
        public int? Sequence { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are various artists on this product (album/track).
        /// </summary>
        /// <value>
        ///   <c>True</c> if there are various artists on this product (album/track); otherwise, <c>false</c>.
        /// </value>
        public bool VariousArtists { get; set; }

        /// <summary>
        /// Gets or sets the product's record label.
        /// </summary>
        /// <value>
        /// The name of the record label.
        /// </value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the street release date of the product.
        /// </summary>
        /// <value>
        /// The street release date.
        /// </value>
        public DateTime StreetReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the Seller Statement for the product
        /// </summary>
        /// <value>
        /// The state from the seller, if available. Sony provide this for example.
        /// </value>
        public string SellerStatement { get; set; }

        /// <summary>
        /// Gets or sets the actor names
        /// Only available in India
        /// </summary>
        public List<string> ActorNames { get; set; }

        /// <summary>
        /// Gets or sets the Lyricist name
        /// Only available in India
        /// </summary>
        public List<string> LyricistsNames { get; set; }

        /// <summary>
        /// Gets or sets the Singer names
        /// Only available in India
        /// </summary>
        public List<string> SingerNames { get; set; }

        /// <summary>
        /// Gets or sets the movie director names
        /// Only available in India
        /// </summary>
        public List<string> MovieDirectorNames { get; set; }

        /// <summary>
        /// Gets or sets the movie producer names
        /// Only available in India
        /// </summary>
        public List<string> MovieProducerNames { get; set; }

        /// <summary>
        /// Gets or sets the music director names
        /// Only available in India
        /// </summary>
        public List<string> MusicDirectorNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item comes with parental advisory.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [parental advisory]; otherwise, <c>false</c>.
        /// </value>
        public bool ParentalAdvisory { get; set; }
    
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
        /// Launches Nokia MixRadio to show details about the product using the ShowProductTask
        /// </summary>
        public void Show()
        {
            ShowProductTask task = new ShowProductTask() { ProductId = this.Id };
            task.Show();
        }

#endif
        /// <summary>
        /// Creates a Product from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Product object</returns>
        internal static Product FromJToken(JToken item)
        {
            if (item == null)
            {
                return null;
            }

            // Extract category...
            Category category = Category.Unknown;
            JToken jsonCategory = item["category"];
            if (jsonCategory != null)
            {
                category = ParseHelper.ParseEnumOrDefault<Category>(jsonCategory.Value<string>("id"));
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
                takenFrom = (Product)FromJToken(jsonTakenFrom);
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
                JArray jsonArtists = item["creators"].Value<JArray>("performers")
                                     ?? item["creators"].Value<JArray>("composers");

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
            Uri square50 = null;
            Uri square100 = null;
            Uri square200 = null;
            Uri square320 = null;

            MusicItem.ExtractThumbs(item["thumbnails"], out square50, out square100, out square200, out square320);

            // Extract Bollywood information
            var actorNames = new List<string>();

            if (item["actornames"] != null)
            {
                ParseArray(item, actorNames, "actornames");
            }

            var lyricistNames = new List<string>();
            if (item["lyricistnames"] != null)
            {
                ParseArray(item, lyricistNames, "lyricistnames");
            }

            var singerNames = new List<string>();
            if (item["singernames"] != null)
            {
                ParseArray(item, singerNames, "singernames");
            }

            var movieDirectorNames = new List<string>();
            if (item["moviedirectornames"] != null)
            {
                ParseArray(item, movieDirectorNames, "moviedirectornames");
            }

            var movieProducerNames = new List<string>();
            if (item["movieproducernames"] != null)
            {
                ParseArray(item, movieProducerNames, "movieproducernames");
            }

            var musicDirectorNames = new List<string>();
            if (item["moviedirectornames"] != null)
            {
                ParseArray(item, musicDirectorNames, "musicdirectornames");
            }

            var parentalAdvisory = item.Value<bool>("parentaladvisory");

            // Create the resulting Product object...
            var product = new Product()
            {
                Id = item.Value<string>("id"),
                Name = item.Value<string>("name"),
                Thumb50Uri = square50,
                Thumb100Uri = square100,
                Thumb200Uri = square200,
                Thumb320Uri = square320,
                Category = category,
                Genres = genres,
                TakenFrom = takenFrom,
                Price = price,
                TrackCount = trackCount,
                Tracks = ExtractTracks(item["tracks"]),
                Performers = performers,
                Duration = item.Value<int?>("duration"),
                VariousArtists = item.Value<bool>("variousartists"),
                StreetReleaseDate = item.Value<DateTime>("streetreleasedate"),
                SellerStatement = item.Value<string>("sellerstatement"),
                Label = item.Value<string>("label"),
                ActorNames = actorNames,
                LyricistsNames = lyricistNames,
                SingerNames = singerNames,
                MovieDirectorNames = movieDirectorNames,
                MovieProducerNames = movieProducerNames,
                MusicDirectorNames = musicDirectorNames,
                ParentalAdvisory = parentalAdvisory
            };

            var sequence = item.Value<int>("sequence");
            if (sequence >= 1)
            {
                product.Sequence = sequence;
            }

            return product;
        }

        private static void ParseArray(JToken item, List<string> addToList,  string jsonField)
        {
            var jsonActorNames = item.Value<JArray>(jsonField);

            foreach (var jsonActorName in jsonActorNames)
            {
                addToList.Add(jsonActorName.Value<string>());
            }
        }

        /// <summary>
        /// Extracts the tracks from the json.
        /// </summary>
        /// <param name="tracksToken">The tracks token.</param>
        /// <returns>A list of tracks</returns>
        private static List<Product> ExtractTracks(JToken tracksToken)
        {
            List<Product> tracks = null;

            if (tracksToken != null)
            {
                tracks = new ArrayJsonProcessor().ParseList(tracksToken, MusicClientCommand.ArrayNameItems, FromJToken);
            }

            return tracks;
        }
    }
}
