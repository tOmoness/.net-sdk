// -----------------------------------------------------------------------
// <copyright file="ProductTests.cs" company="NOKIA">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Tests.Properties;
using Nokia.Music.Types;
using NUnit.Framework;

namespace Nokia.Music.Tests.Types
{
    /// <summary>
    /// Product tests
    /// </summary>
    [TestFixture]
    public class ProductTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            Product product = new Product() { Id = TestId, Name = TestName };

            Assert.AreEqual(TestId, product.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, product.Name, "Expected the property to persist");
        }

        [Test]
        public void TestLinkingProperties()
        {
            var item = new Product() { Id = TestId };
            var itemWithNullId = new Product();

            Assert.IsNotNull(item.AppToAppUri, "Expected App to App URI to be calculated");
            Assert.IsNull(itemWithNullId.AppToAppUri, "Expected App to App URI not to be calculated");

            Assert.IsNotNull(item.WebUri, "Expected Web URI to be calculated");
            Assert.IsNull(itemWithNullId.WebUri, "Expected Web URI not to be calculated");
        }

        [Test]
        public void TestOverrides()
        {
            Product product = new Product() { Id = TestId, Name = TestName };
            Assert.IsNotNull(product.GetHashCode(), "Expected a hash code");
            Assert.IsTrue(product.Equals(new Product() { Id = TestId }), "Expected equality");
            Assert.IsFalse(product.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void HashCodeCanBeRetrievedWhenIdIsNull()
        {
            Product product = new Product();
            Assert.IsNotNull(product.GetHashCode(), "Expected a hash code");
        }

        [Test]
        public void TestJsonParsing()
        {
            Assert.IsNull(Product.FromJToken(null), "Expected a null return");

            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.product_parse_tests));

            JArray items = json.Value<JArray>(MusicClientCommand.ArrayNameItems);

            // Test a full representation
            Product fullItem = Product.FromJToken(items[0]) as Product;
            Assert.IsNotNull(fullItem, "Expected a product object");
            Assert.IsNotNull(fullItem.TakenFrom, "Expected an album");
            Assert.IsNotNull(fullItem.Genres, "Expected genres");
            Assert.Greater(fullItem.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(fullItem.Id, "Expected an id");
            Assert.IsNotNull(fullItem.Name, "Expected a name");
            Assert.Greater(fullItem.Performers.Length, 0, "Expected performers");
            Assert.IsNotNull(fullItem.Price, "Expected a price");
            Assert.IsNotNull(fullItem.Thumb50Uri, "Expected a 50x50 thumb");
            Assert.IsNotNull(fullItem.Thumb100Uri, "Expected a 100x100 thumb");
            Assert.IsNotNull(fullItem.Thumb200Uri, "Expected a 200x200 thumb");
            Assert.IsNotNull(fullItem.Thumb320Uri, "Expected a 320x320 thumb");
            Assert.AreEqual(fullItem.Category, Category.Track, "Expected a track");
            Assert.IsNull(fullItem.Sequence, "Expected sequence to be null");
        }

        [Test]
        public void TestMovieMetaDataParsing()
        {
            var json = JObject.Parse(Encoding.UTF8.GetString(Resources.product_movie_metadata_parse_tests));

            Product product = Product.FromJToken(json);

            // Test a full representation
            Assert.IsNotNull(product, "Expected a product object");
            Assert.IsNotNull(product.ActorNames, "Expected Actor Names");
            Assert.AreEqual(2, product.ActorNames.Count, "Expected Actor Names count");
            Assert.IsNotNull(product.LyricistsNames, "Expected Lyricists");
            Assert.AreEqual(1, product.LyricistsNames.Count, "Expected Lyricists");
            Assert.IsNotNull(product.SingerNames, "Expected singer names");
            Assert.AreEqual(2, product.SingerNames.Count, "Expected singer names");
            Assert.IsNotNull(product.MovieDirectorNames, "Expected an Movie director names");
            Assert.AreEqual(1, product.MovieDirectorNames.Count, "Expected MovieDirectorNames count");
            Assert.IsNotNull(product.MovieProducerNames, "Expected a Movies producer names");
            Assert.AreEqual(1, product.MovieProducerNames.Count, "Expected Movie Producer Names count");
            Assert.IsNotNull(product.MusicDirectorNames, "Expected music director names");
            Assert.AreEqual(1, product.MusicDirectorNames.Count, "Expected Music Director Names count");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIdPropertyIsRequiredForShow()
        {
            Product product = new Product();
            product.Show();
        }

        [Test]
        public void TestShowGoesAheadWhenItCan()
        {
            Product product = new Product() { Id = TestId };
            product.Show();
            Assert.Pass();
        }

        [Test]
        public void AlbumFromJTokenParsesSuccessfully()
        {
            var json = JObject.Parse(Encoding.UTF8.GetString(Resources.single_product));
            Product album = Product.FromJToken(json);

            Assert.That(album, Is.Not.Null, "Album");
            Assert.That(album.Category, Is.EqualTo(Category.Album));
            Assert.That(album.Performers.Length, Is.EqualTo(1), "Product.Performers.Length");
            Assert.That(album.Performers[0].Name, Is.EqualTo("Rihanna"), "Product.Performers.Name");
            Assert.That(album.Performers[0].Id, Is.EqualTo("305681"), "Product.Performers.Id");

            Assert.That(album.Thumb100Uri.AbsoluteUri, Is.EqualTo("http://4.musicimg.ovi.com/u/1.0/image/252983708/?w=100&q=70"), "Thumb100Uri");
            Assert.That(album.Thumb200Uri.AbsoluteUri, Is.EqualTo("http://4.musicimg.ovi.com/u/1.0/image/252983708/?w=200&q=90"), "Thumb200Uri");
            Assert.That(album.Thumb50Uri.AbsoluteUri, Is.EqualTo("http://4.musicimg.ovi.com/u/1.0/image/252983708/?w=50&q=40"), "Thumb50Uri");
            Assert.That(album.Id, Is.EqualTo("31189154"), "Product.Id");
            Assert.That(album.Name, Is.EqualTo("Rated R"), "Product.Name");
            Assert.That(album.VariousArtists, Is.EqualTo(true), "VariousArtists");
            Assert.That(album.Label, Is.EqualTo("Def Jam"), "Label");
            Assert.That(album.StreetReleaseDate, Is.EqualTo(new DateTime(2009, 11, 23)));
            Assert.That(album.SellerStatement, Is.EqualTo("Sold by Sony"), "SellerStatement");
            Assert.That(album.TakenFrom, Is.Null);
            Assert.That(album.Duration, Is.EqualTo(0), "Duration");
            Assert.That(album.TrackCount, Is.EqualTo(13), "track count");

            Assert.That(album.Tracks[0].Category, Is.EqualTo(Category.Track));
            Assert.That(album.Tracks[0].VariousArtists, Is.EqualTo(false), "Track 1 : VariousArtists");
            Assert.That(album.Tracks[0].TrackCount, Is.Null, "Track 1 : track count");
        }
    }
}
