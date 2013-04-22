// -----------------------------------------------------------------------
// <copyright file="IMusicClient.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
#if SUPPORTS_ASYNC
using System.Threading.Tasks;
#endif
using Nokia.Music.Types;

namespace Nokia.Music
{
    /// <summary>
    /// Defines the Nokia Music API
    /// </summary>
    public partial interface IMusicClient
    {
#if !NETFX_CORE
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void SearchArtists(Action<ListResponse<Artist>> callback, string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> SearchArtistsAsync(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetArtistSearchSuggestions(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing search suggestions</returns>
        Task<ListResponse<string>> GetArtistSearchSuggestionsAsync(string searchTerm, int itemsPerPage = 3);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets artists that originate around a specified location
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="maxdistance">The max distance (in KM) around the location to search</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetArtistsAroundLocation(Action<ListResponse<Artist>> callback, double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets artists that originate around a specified location
        /// </summary>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="maxdistance">The max distance (in KM) around the location to search</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetArtistsAroundLocationAsync(double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopArtists(Action<ListResponse<Artist>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetSimilarArtists(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetSimilarArtists(Action<ListResponse<Artist>> callback, Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetSimilarArtistsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetSimilarArtistsAsync(Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetArtistProducts(Action<ListResponse<Product>> callback, string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetArtistProducts(Action<ListResponse<Product>> callback, Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetArtistProductsAsync(string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetArtistProductsAsync(Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a product by product id.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        void GetProduct(Action<Response<Product>> callback, string id);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a product by id
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>A Response containing a Product or an Error</returns>
        Task<Response<Product>> GetProductAsync(string id);

#endif
        /// <summary>
        /// Gets a track sample uri.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>
        /// A uri to a sample clip of the track
        /// </returns>
        Uri GetTrackSampleUri(string id);

#if !NETFX_CORE
        /// <summary>
        /// Gets similar products for the supplied product id.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetSimilarProducts(Action<ListResponse<Product>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets similar products for the supplied product id.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="product">The product.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetSimilarProducts(Action<ListResponse<Product>> callback, Product product, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets similar products for the supplied product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetSimilarProductsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets similar products for the supplied product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetSimilarProductsAsync(Product product, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);
#endif
#if !NETFX_CORE
       /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopProducts(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetTopProductsAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopProductsForGenre(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetTopProductsForGenre(Action<ListResponse<Product>> callback, Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetTopProductsForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetTopProductsForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetNewReleases(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetNewReleasesAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetNewReleasesForGenre(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetNewReleasesForGenre(Action<ListResponse<Product>> callback, Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetNewReleasesForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetNewReleasesForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        void GetGenres(Action<ListResponse<Genre>> callback);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <returns>A ListResponse containing Genres or an Error</returns>
        Task<ListResponse<Genre>> GetGenresAsync();

#endif
#if !NETFX_CORE
        /// <summary>
        /// Searches Nokia Music
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void Search(Action<ListResponse<MusicItem>> callback, string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Searches Nokia Music
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing MusicItems or an Error</returns>
        Task<ListResponse<MusicItem>> SearchAsync(string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetSearchSuggestions(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing search suggestions</returns>
        Task<ListResponse<string>> GetSearchSuggestionsAsync(string searchTerm, int itemsPerPage = 3);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetMixGroups(Action<ListResponse<MixGroup>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="exclusiveTag">The exclusive tag</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        void GetMixGroups(Action<ListResponse<MixGroup>> callback, string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing MixGroups or an Error</returns>
        Task<ListResponse<MixGroup>> GetMixGroupsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        Task<ListResponse<MixGroup>> GetMixGroupsAsync(string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage);

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        void GetMixes(Action<ListResponse<Mix>> callback, string id);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        void GetMixes(Action<ListResponse<Mix>> callback, string id, string exclusiveTag);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="group">The mix group.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        void GetMixes(Action<ListResponse<Mix>> callback, MixGroup group, string exclusiveTag);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="group">The mix group.</param>
        void GetMixes(Action<ListResponse<Mix>> callback, MixGroup group);

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <returns>A ListResponse containing Mixes or an Error</returns>
        Task<ListResponse<Mix>> GetMixesAsync(string id);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        Task<ListResponse<Mix>> GetMixesAsync(string id, string exclusiveTag);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <returns>A ListResponse containing Mixes or an Error</returns>
        Task<ListResponse<Mix>> GetMixesAsync(MixGroup group);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        Task<ListResponse<Mix>> GetMixesAsync(MixGroup group, string exclusiveTag);
#endif
    }
}
