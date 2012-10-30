// -----------------------------------------------------------------------
// <copyright file="ApiUriBuilderTests.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nokia.Music.Phone.Internal;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests
{
    [TestFixture]
    public class ApiUriBuilderTests
    {
        private const string AppId = @"test";
        private const string AppCode = @"test";
        private const string Country = @"gb";

        [Test]
        public void CheckParametersAreValidated()
        {
            IApiUriBuilder builder = new ApiUriBuilder();

            // Check ApiMethod param...
            Assert.Throws(typeof(ArgumentOutOfRangeException), new TestDelegate(() => { builder.BuildUri(ApiMethod.Unknown, null, null, null, null, null); }));

            // Check API Key is required...
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(ApiMethod.CountryLookup, null, null, null, null, null); }));
            Assert.Throws(typeof(ApiCredentialsRequiredException), new TestDelegate(() => { builder.BuildUri(ApiMethod.CountryLookup, AppId, null, null, null, null); }));

            // Check Country Code is required...
            Assert.Throws(typeof(CountryCodeRequiredException), new TestDelegate(() => { builder.BuildUri(ApiMethod.Search, AppId, AppCode, null, null, null); }));
        }

        [Test]
        public void ValidateDeviceCountryUris()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.CountryLookup, AppId, AppCode, null, null, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateSearchUris()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/?app_id=test&app_code=test&domain=music&q=test");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.Search, AppId, AppCode, Country, null, new Dictionary<string, string>() { { "q", "test" } });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateGenreUris()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/genres/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.Genres, AppId, AppCode, Country, null, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnsureSimilarArtistIdIsValidated()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.SimilarArtists, AppId, AppCode, Country, null, null);
                }));

            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.SimilarArtists, AppId, AppCode, Country, new Dictionary<string, string>(), null);
                }));
        }

        [Test]
        public void ValidateSimilarArtistUri()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/creators/348877/similar/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.SimilarArtists, AppId, AppCode, Country, new Dictionary<string, string>() { { "id", "348877" } }, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnsureArtistProductsIdIsValidated()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ArtistProducts, AppId, AppCode, Country, null, null);
                }));

            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ArtistProducts, AppId, AppCode, Country, new Dictionary<string, string>(), null);
                }));
        }

        [Test]
        public void ValidateArtistProductsUri()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/creators/297011/products/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.ArtistProducts, AppId, AppCode, Country, new Dictionary<string, string>() { { "id", "297011" } }, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnsureProductChartCategoryIsValidated()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ProductChart, AppId, AppCode, Country, null, null);
                }));

            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ProductChart, AppId, AppCode, Country, new Dictionary<string, string>(), null);
                }));
        }

        [Test]
        public void ValidateProductChartUri()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/products/charts/album/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.ProductChart, AppId, AppCode, Country, new Dictionary<string, string>() { { "category", "album" } }, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnsureProductNewReleasesCategoryIsValidated()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ProductNewReleases, AppId, AppCode, Country, null, null);
                }));

            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.ProductNewReleases, AppId, AppCode, Country, new Dictionary<string, string>(), null);
                }));
        }

        [Test]
        public void ValidateProductNewReleasesUri()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/products/new/album/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.ProductNewReleases, AppId, AppCode, Country, new Dictionary<string, string>() { { "category", "album" } }, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateRecommendationsUri()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/recommendations/?app_id=test&app_code=test&domain=music&creator=Green%20Day&album=American%20Idiot&name=American%20Idiot&category=track");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.Recommendations, AppId, AppCode, Country, null, new Dictionary<string, string>() { { "creator", "Green Day" }, { "album", "American Idiot" }, { "name", "American Idiot" }, { "category", "track" } });
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidateMixGroupsUris()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/mixes/groups/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.MixGroups, AppId, AppCode, Country, null, null);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnsureMixesIdIsValidated()
        {
            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.Mixes, AppId, AppCode, Country, null, null);
                }));

            Assert.Throws(
                typeof(ArgumentNullException),
                new TestDelegate(() =>
                {
                    new ApiUriBuilder().BuildUri(ApiMethod.Mixes, AppId, AppCode, Country, new Dictionary<string, string>(), null);
                }));
        }

        [Test]
        public void ValidateMixesUris()
        {
            Uri expected = new Uri(@"http://api.ent.nokia.com/1.x/gb/mixes/groups/test/?app_id=test&app_code=test&domain=music");
            Uri result = new ApiUriBuilder().BuildUri(ApiMethod.Mixes, AppId, AppCode, Country, new Dictionary<string, string>() { { "id", "test" } }, null);
            Assert.AreEqual(expected, result);
        }
    }
}
