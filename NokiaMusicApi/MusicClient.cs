// -----------------------------------------------------------------------
// <copyright file="MusicClient.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
#if SUPPORTS_ASYNC
using System.Threading.Tasks;
#endif
using Nokia.Music.Commands;
using Nokia.Music.Internal;
using Nokia.Music.Internal.Compression;
using Nokia.Music.Internal.Request;
using Nokia.Music.Types;

namespace Nokia.Music
{
    /// <summary>
    ///   The Nokia Music API client
    /// </summary>
    public sealed partial class MusicClient : IMusicClientSettings, IMusicClient
    {
        internal const int DefaultItemsPerPage = 10;
        internal const int DefaultStartIndex = 0;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class,
        ///   using the RegionInfo settings to locate the user.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        public MusicClient(string appId)
            : this(
                appId,
                RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(),
                ApiRequestHandlerFactory.Create())
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="countryCode"> The country code. </param>
        public MusicClient(string appId, string countryCode)
            : this(appId, countryCode, ApiRequestHandlerFactory.Create())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string appId, IApiRequestHandler requestHandler)
            : this(appId, RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(), requestHandler)
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="appId"> The App ID obtained from api.developer.nokia.com </param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string appId, string countryCode, IApiRequestHandler requestHandler)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw new ApiCredentialsRequiredException();
            }

            this.AppId = appId;
            this.RequestHandler = requestHandler;

            if (this.ValidateCountryCode(countryCode))
            {
                this.CountryCode = countryCode.ToLowerInvariant();
            }
            else
            {
                throw new InvalidCountryCodeException();
            }
        }

        /// <summary>
        /// Gets or sets the timeout duration for web requests.
        /// </summary>
        /// <value>
        /// The timeout duration in milliseconds.
        /// </value>
        public static int RequestTimeout
        {
            get { return TimedRequest.RequestTimeout; }
            set { TimedRequest.RequestTimeout = value; }
        }

        #region IMusicClientSettings Members

        /// <summary>
        /// Gets a value indicating whether the country code was based on region info.
        /// </summary>
        /// <value>
        /// <c>true</c> if the country code was based on region info; otherwise, <c>false</c>.
        /// </value>
        public bool CountryCodeBasedOnRegionInfo { get; private set; }

        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>
        /// The app id.
        /// </value>
        public string AppId { get; private set; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; private set; }

        #endregion

        /// <summary>
        /// Gets the request handler in use for testing purposes.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; private set; }

        /// <summary>
        /// Gets or sets the base API uri
        /// </summary>
        internal string BaseApiUri { get; set; }

        #region IMusicClient Members
#if !NETFX_CORE
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void SearchArtists(Action<ListResponse<Artist>> callback, string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.SearchArtistsInternal(callback, searchTerm, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> SearchArtistsAsync(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.SearchArtistsInternal(result => wrapper.TrySetResult(result), searchTerm, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetArtistSearchSuggestions(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3)
        {
            this.GetArtistSearchSuggestionsInternal(callback, searchTerm, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public Task<ListResponse<string>> GetArtistSearchSuggestionsAsync(string searchTerm, int itemsPerPage = 3)
        {
            var wrapper = new TaskCompletionSource<ListResponse<string>>();
            this.GetArtistSearchSuggestionsInternal(result => wrapper.TrySetResult(result), searchTerm, itemsPerPage);
            return wrapper.Task;
        }

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
        public void GetArtistsAroundLocation(Action<ListResponse<Artist>> callback, double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetArtistsAroundLocationInternal(callback, latitude, longitude, maxdistance, startIndex, itemsPerPage);
        }

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
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetArtistsAroundLocationAsync(double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetArtistsAroundLocationInternal(result => wrapper.TrySetResult(result), latitude, longitude, maxdistance, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtists(Action<ListResponse<Artist>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetTopArtistsInternal(callback, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetTopArtistsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetTopArtistsInternal(result => wrapper.TrySetResult(result), startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetTopArtistsForGenreInternal(callback, id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopArtistsForGenre(Action<ListResponse<Artist>> callback, Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            this.GetTopArtistsForGenreInternal(callback, genre.Id, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetTopArtistsForGenreInternal(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
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
        public Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetTopArtistsForGenreInternal(result => wrapper.TrySetResult(result), genre.Id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarArtists(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetSimilarArtistsInternal(callback, id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarArtists(Action<ListResponse<Artist>> callback, Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            this.GetSimilarArtistsInternal(callback, artist.Id, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> GetSimilarArtistsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetSimilarArtistsInternal(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
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
        public Task<ListResponse<Artist>> GetSimilarArtistsAsync(Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Artist>>();
            this.GetSimilarArtistsInternal(result => wrapper.TrySetResult(result), artist.Id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

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
        public void GetArtistProducts(Action<ListResponse<Product>> callback, string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetArtistProductsInternal(callback, id, category, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetArtistProducts(Action<ListResponse<Product>> callback, Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            this.GetArtistProductsInternal(callback, artist.Id, category, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
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
        public Task<ListResponse<Product>> GetArtistProductsAsync(string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetArtistProductsInternal(result => wrapper.TrySetResult(result), id, category, startIndex, itemsPerPage);
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
        public Task<ListResponse<Product>> GetArtistProductsAsync(Artist artist, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetArtistProductsInternal(result => wrapper.TrySetResult(result), artist.Id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a product by product id.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        public void GetProduct(Action<Response<Product>> callback, string id)
        {
            this.GetProductInternal(callback, id);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets products by id.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>
        /// A Response containing a Product or an Error
        /// </returns>
        public Task<Response<Product>> GetProductAsync(string id)
        {
            var wrapper = new TaskCompletionSource<Response<Product>>();
            this.GetProductInternal(result => wrapper.TrySetResult(result), id);
            return wrapper.Task;
        }

#endif
        /// <summary>
        /// Gets a track sample uri.
        /// </summary>
        /// <param name="id">The track id.</param>
        /// <returns>
        /// A uri to a sample clip of the track
        /// </returns>
        /// <exception cref="System.ArgumentNullException">id;id cannot be null</exception>
        public Uri GetTrackSampleUri(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "id cannot be null");
            }

            return new Uri(string.Format("{0}{1}/products/{2}/sample/?domain=music&app_id={3}", MusicClientCommand.DefaultBaseApiUri, this.CountryCode, id, this.AppId), UriKind.Absolute);
        }

#if !NETFX_CORE
        /// <summary>
        /// Gets similar products to the product provided.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarProducts(Action<ListResponse<Product>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetSimilarProductsInternal(callback, id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets similar products to the product provided.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="product">The product.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSimilarProducts(Action<ListResponse<Product>> callback, Product product, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product", "Product cannot be null");
            }

            this.GetSimilarProductsInternal(callback, product.Id, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a similar product for the supplied product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        public Task<ListResponse<Product>> GetSimilarProductsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetSimilarProductsInternal(result => wrapper.TrySetResult(result), id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets a similar product for the supplied product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        public Task<ListResponse<Product>> GetSimilarProductsAsync(Product product, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product", "Product cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetSimilarProductsInternal(result => wrapper.TrySetResult(result), product.Id, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopProducts(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetTopProductsInternal(callback, category, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetTopProductsAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetTopProductsInternal(result => wrapper.TrySetResult(result), category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

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
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public void GetTopProductsForGenre(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetTopProductsForGenreInternal(callback, id, category, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetTopProductsForGenre(Action<ListResponse<Product>> callback, Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            this.GetTopProductsForGenreInternal(callback, genre.Id, category, startIndex, itemsPerPage);
        }

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
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public Task<ListResponse<Product>> GetTopProductsForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetTopProductsForGenreInternal(result => wrapper.TrySetResult(result), id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

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
        public Task<ListResponse<Product>> GetTopProductsForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetTopProductsForGenreInternal(result => wrapper.TrySetResult(result), genre.Id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetNewReleases(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetNewReleasesInternal(callback, category, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetNewReleasesAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetNewReleasesInternal(result => wrapper.TrySetResult(result), category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

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
        public void GetNewReleasesForGenre(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetNewReleasesForGenreInternal(callback, id, category, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public void GetNewReleasesForGenre(Action<ListResponse<Product>> callback, Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            this.GetNewReleasesForGenreInternal(callback, genre.Id, category, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
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
        public Task<ListResponse<Product>> GetNewReleasesForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (genre == null)
            {
                throw new ArgumentNullException("genre", "genre cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetNewReleasesForGenreInternal(result => wrapper.TrySetResult(result), genre.Id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

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
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public Task<ListResponse<Product>> GetNewReleasesForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Product>>();
            this.GetNewReleasesForGenreInternal(result => wrapper.TrySetResult(result), id, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        public void GetGenres(Action<ListResponse<Genre>> callback)
        {
            this.GetGenresInternal(callback);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <returns>
        /// A ListResponse containing Genres or an Error
        /// </returns>
        public Task<ListResponse<Genre>> GetGenresAsync()
        {
            var wrapper = new TaskCompletionSource<ListResponse<Genre>>();
            this.GetGenresInternal(result => wrapper.TrySetResult(result));
            return wrapper.Task;
        }

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
        public void Search(Action<ListResponse<MusicItem>> callback, string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.SearchInternal(callback, searchTerm, category, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
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
        public Task<ListResponse<MusicItem>> SearchAsync(string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<MusicItem>>();
            this.SearchInternal(result => wrapper.TrySetResult(result), searchTerm, category, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetSearchSuggestions(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3)
        {
            this.GetSearchSuggestionsInternal(callback, searchTerm, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public Task<ListResponse<string>> GetSearchSuggestionsAsync(string searchTerm, int itemsPerPage = 3)
        {
            var wrapper = new TaskCompletionSource<ListResponse<string>>();
            this.GetSearchSuggestionsInternal(result => wrapper.TrySetResult(result), searchTerm, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetMixGroups(Action<ListResponse<MixGroup>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetMixGroups(callback, null, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="exclusiveTag">The exclusive tag</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        public void GetMixGroups(Action<ListResponse<MixGroup>> callback, string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            this.GetMixGroupsInternal(callback, exclusiveTag, startIndex, itemsPerPage);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        public Task<ListResponse<MixGroup>> GetMixGroupsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            return this.GetMixGroupsAsync(string.Empty, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        public Task<ListResponse<MixGroup>> GetMixGroupsAsync(string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var wrapper = new TaskCompletionSource<ListResponse<MixGroup>>();
            this.GetMixGroupsInternal(result => wrapper.TrySetResult(result), exclusiveTag, startIndex, itemsPerPage);
            return wrapper.Task;
        }

#endif
#if !NETFX_CORE
        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        public void GetMixes(Action<ListResponse<Mix>> callback, string id)
        {
            this.GetMixes(callback, id, null);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        public void GetMixes(Action<ListResponse<Mix>> callback, string id, string exclusiveTag)
        {
            this.GetMixesInternal(callback, id, exclusiveTag);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="group">The mix group.</param>
        public void GetMixes(Action<ListResponse<Mix>> callback, MixGroup group)
        {
            this.GetMixes(callback, group, null);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="group">The mix group.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <exception cref="System.ArgumentNullException">group;group cannot be null</exception>
        public void GetMixes(Action<ListResponse<Mix>> callback, MixGroup group, string exclusiveTag)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group", "group cannot be null");
            }

            this.GetMixes(callback, group.Id, exclusiveTag);
        }

#endif
#if SUPPORTS_ASYNC
        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixesAsync(string id)
        {
            return this.GetMixesAsync(id, null);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixesAsync(string id, string exclusiveTag)
        {
            var wrapper = new TaskCompletionSource<ListResponse<Mix>>();
            this.GetMixesInternal(result => wrapper.TrySetResult(result), id, exclusiveTag);
            return wrapper.Task;
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixesAsync(MixGroup group)
        {
            return this.GetMixesAsync(group, null);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public Task<ListResponse<Mix>> GetMixesAsync(MixGroup group, string exclusiveTag)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group", "group cannot be null");
            }

            var wrapper = new TaskCompletionSource<ListResponse<Mix>>();
            this.GetMixesInternal(result => wrapper.TrySetResult(result), group.Id, exclusiveTag);
            return wrapper.Task;
        }

#endif
        #endregion

        #region Internal Implementations
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void SearchArtistsInternal(Action<ListResponse<Artist>> callback, string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SearchArtistsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetArtistSearchSuggestionsInternal(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3)
        {
            var cmd = this.Create<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = true;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets artists that originate around a specified location
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="maxdistance">The max distance (in KM) around the location to search</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetArtistsAroundLocationInternal(Action<ListResponse<Artist>> callback, double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SearchArtistsCommand>();
            cmd.Location = new Location() { Latitude = latitude, Longitude = longitude };
            cmd.MaxDistance = maxdistance;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetTopArtistsInternal(Action<ListResponse<Artist>> callback, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<TopArtistsCommand>();
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetTopArtistsForGenreInternal(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.Create<TopArtistsCommand>();
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetSimilarArtistsInternal(Action<ListResponse<Artist>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SimilarArtistsCommand>();
            cmd.ArtistId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetArtistProductsInternal(Action<ListResponse<Product>> callback, string id, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<ArtistProductsCommand>();
            cmd.ArtistId = id;
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a product by product id.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        private void GetProductInternal(Action<Response<Product>> callback, string id)
        {
            var cmd = this.Create<ProductCommand>();
            cmd.ProductId = id;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets similar products to the product provided.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetSimilarProductsInternal(Action<ListResponse<Product>> callback, string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SimilarProductsCommand>();
            cmd.ProductId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetTopProductsInternal(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<TopProductsCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetTopProductsForGenreInternal(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.Create<TopProductsCommand>();
            cmd.Category = category;
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetNewReleasesInternal(Action<ListResponse<Product>> callback, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<NewReleasesCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <exception cref="System.ArgumentNullException">id;genre id cannot be null</exception>
        private void GetNewReleasesForGenreInternal(Action<ListResponse<Product>> callback, string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.Create<NewReleasesCommand>();
            cmd.Category = category;
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        private void GetGenresInternal(Action<ListResponse<Genre>> callback)
        {
            var cmd = this.Create<GenresCommand>();
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Searches Nokia Music
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void SearchInternal(Action<ListResponse<MusicItem>> callback, string searchTerm, Category? category = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<SearchCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetSearchSuggestionsInternal(Action<ListResponse<string>> callback, string searchTerm, int itemsPerPage = 3)
        {
            var cmd = this.Create<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = false;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="exclusiveTag">The exclusive tag</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        private void GetMixGroupsInternal(Action<ListResponse<MixGroup>> callback, string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.Create<MixGroupsCommand>();
            cmd.ExclusiveTag = exclusiveTag;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.Invoke(callback);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="callback">The callback to use when the API call has completed</param>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        private void GetMixesInternal(Action<ListResponse<Mix>> callback, string id, string exclusiveTag)
        {
            var cmd = this.Create<MixesCommand>();
            cmd.MixGroupId = id;
            cmd.ExclusiveTag = exclusiveTag;
            cmd.Invoke(callback);
        }
        #endregion
        /// <summary>
        /// Creates a command to execute
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>A Command to call</returns>
        private TCommand Create<TCommand>() where TCommand : MusicClientCommand, new()
        {
            return new TCommand
            {
                MusicClientSettings = this,
                RequestHandler = this.RequestHandler,
                BaseApiUri = this.BaseApiUri
            };
        }

        /// <summary>
        ///   Validates a country code.
        /// </summary>
        /// <param name="countryCode"> The country code. </param>
        /// <returns> A Boolean indicating that the country code is valid </returns>
        private bool ValidateCountryCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                return countryCode.Length == 2;
            }
            else
            {
                return false;
            }
        }
    }
}