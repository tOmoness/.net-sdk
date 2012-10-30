// -----------------------------------------------------------------------
// <copyright file="MusicClientAsync.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone
{
    /// <summary>
    /// Adaption of the IMusicClient API for WP8 async/await usage
    /// </summary>
    public sealed class MusicClientAsync : IMusicClientAsync
    {
        private IMusicClient _musicClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientAsync" /> class,
        /// using the RegionInfo settings to locate the user.
        /// </summary>
        /// <param name="appId">The App ID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The App Code obtained from api.developer.nokia.com</param>
        public MusicClientAsync(string appId, string appCode)
        {
            this._musicClient = new MusicClient(appId, appCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientAsync" /> class.
        /// </summary>
        /// <param name="appId">The App ID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The App Code obtained from api.developer.nokia.com</param>
        /// <param name="countryCode">The country code.</param>
        public MusicClientAsync(string appId, string appCode, string countryCode)
        {
            this._musicClient = new MusicClient(appId, appCode, countryCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicClientAsync" /> class.
        /// </summary>
        /// <param name="appId">The App ID obtained from api.developer.nokia.com</param>
        /// <param name="appCode">The App Code obtained from api.developer.nokia.com</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="requestHandler">The request handler.</param>
        /// <remarks>
        /// Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClientAsync(string appId, string appCode, string countryCode, IApiRequestHandler requestHandler)
        {
            this._musicClient = new MusicClient(appId, appCode, countryCode, requestHandler);
        }

        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> SearchArtists(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.SearchArtists(result => wrapper.TrySetResult(result), searchTerm, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetTopArtists(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.GetTopArtists(result => wrapper.TrySetResult(result), startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetTopArtistsForGenre(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.GetTopArtistsForGenre(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetTopArtistsForGenre(Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.GetTopArtistsForGenre(result => wrapper.TrySetResult(result), genre, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetSimilarArtists(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.GetSimilarArtists(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetSimilarArtists(Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this._musicClient.GetSimilarArtists(result => wrapper.TrySetResult(result), artist, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetArtistProducts(string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this._musicClient.GetArtistProducts(result => wrapper.TrySetResult(result), id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetArtistProducts(Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this._musicClient.GetArtistProducts(result => wrapper.TrySetResult(result), artist, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetTopProducts(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this._musicClient.GetTopProducts(result => wrapper.TrySetResult(result), category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetNewReleases(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this._musicClient.GetNewReleases(result => wrapper.TrySetResult(result), category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <returns>
        /// A ListResponse containing Genres or an Error
        /// </returns>
        public Task<ListResponse<Genre>> GetGenres()
        {
            var wrapper = new TaskCompletionSource<ListResponse<Genre>>();
            this._musicClient.GetGenres(result => wrapper.TrySetResult(result));
            return wrapper.Task;
        }

        /// <summary>
        /// Searches Nokia Music
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing MusicItems or an Error
        /// </returns>
        public Task<ListResponse<MusicItem>> Search(string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<MusicItem>>();
            this._musicClient.Search(result => wrapper.TrySetResult(result), searchTerm, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        public Task<ListResponse<MixGroup>> GetMixGroups(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<MixGroup>>();
            this._musicClient.GetMixGroups(result => wrapper.TrySetResult(result), startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixes(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Mix>>();
            this._musicClient.GetMixes(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixes(MixGroup group, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Mix>>();
            this._musicClient.GetMixes(result => wrapper.TrySetResult(result), group, startIndex, itemsPerPage);
            return wrapper.Task;
        }
    }
}
