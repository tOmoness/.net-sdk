// -----------------------------------------------------------------------
// <copyright file="MusicClient.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif
using Newtonsoft.Json.Linq;
using Nokia.Music.Commands;
using Nokia.Music.Internal;
#if SUPPORTS_OAUTH
using Nokia.Music.Internal.Authorization;
#endif
using Nokia.Music.Internal.Compression;
using Nokia.Music.Internal.Request;
using Nokia.Music.Types;

namespace Nokia.Music
{
    /// <summary>
    /// The Nokia MixRadio API client
    /// </summary>
    public sealed partial class MusicClient : IMusicClientSettings, IMusicClient
    {
        internal const int DefaultItemsPerPage = 10;
        internal const int DefaultSmallItemsPerPage = 3;
        internal const int DefaultStartIndex = 0;
        internal const string DefaultOAuthRedirectUri = "https://account.music.nokia.com/authorize/complete";

#if SUPPORTS_USER_OAUTH
        internal const string TokenCacheFile = "NokiaMusicOAuthToken.json";
        private OAuthUserFlow _oauthFlowController = null;
        private TokenResponse _oauthToken = null;
#endif

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class,
        ///   using the RegionInfo settings to locate the user.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        public MusicClient(string clientId)
            : this(
                clientId,
                RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(),
                null,
                ApiRequestHandlerFactory.Create())
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        public MusicClient(string clientId, string countryCode)
            : this(clientId, countryCode, null, ApiRequestHandlerFactory.Create())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="language"> The language code. </param>
        public MusicClient(string clientId, string countryCode, string language)
            : this(clientId, countryCode, language, ApiRequestHandlerFactory.Create())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string clientId, IApiRequestHandler requestHandler)
            : this(clientId, RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower(), null, requestHandler)
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string clientId, string countryCode, IApiRequestHandler requestHandler)
            : this(clientId, countryCode, null, requestHandler)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="language"> The language code. </param>
        /// <param name="requestHandler"> The request handler. </param>
        /// <remarks>
        ///   Allows custom requestHandler for testing purposes
        /// </remarks>
        internal MusicClient(string clientId, string countryCode, string language, IApiRequestHandler requestHandler)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ApiCredentialsRequiredException();
            }

            this.BaseApiUri = MusicClientCommand.DefaultBaseApiUri;
            this.SecureBaseApiUri = MusicClientCommand.DefaultSecureBaseApiUri;

            this.ClientId = clientId;
            this.Language = language;
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
        /// Gets the app client id.
        /// </summary>
        /// <value>
        /// The app client id.
        /// </value>
        public string ClientId { get; private set; }

        /// <summary>
        /// Gets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; private set; }

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string Language { get; private set; }

        #endregion

        /// <summary>
        /// Gets the server UTC time.
        /// </summary>
        /// <value>
        /// The server UTC time.
        /// </value>
        public DateTime ServerTimeUtc
        {
            get
            {
                return this.RequestHandler.ServerTimeUtc;
            }
        }

#if SUPPORTS_USER_OAUTH
        /// <summary>
        /// Gets a value indicating whether the user has been authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserAuthenticated
        {
            get
            {
                return this._oauthToken != null && !string.IsNullOrEmpty(this._oauthToken.AccessToken) && this.IsUserTokenActive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user token is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user token is active; otherwise, <c>false</c>.
        /// </value>
        internal bool IsUserTokenActive
        {
            get
            {
                return this._oauthToken != null && this._oauthToken.ExpiresUtc > this.ServerTimeUtc;
            }
        }

#endif
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

        /// <summary>
        /// Gets or sets the secure base API uri
        /// </summary>
        internal string SecureBaseApiUri { get; set; }

#if SUPPORTS_OAUTH
        /// <summary>
        /// Gets or sets the auth header data provider.
        /// </summary>
        /// <value>
        /// The auth header data provider.
        /// </value>
        internal IAuthHeaderDataProvider AuthHeaderDataProvider { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        internal string UserId { get; set; }
#endif
        #region IMusicClient Members
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public Task<ListResponse<Artist>> SearchArtistsAsync(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null)
        {
            var cmd = this.CreateCommand<SearchArtistsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;

            this.SetRequestId(cmd, requestId);

            return cmd.InvokeAsync();
        }

        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public Task<ListResponse<string>> GetArtistSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null)
        {
            var cmd = this.CreateCommand<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = true;

            this.SetRequestId(cmd, requestId);

            return cmd.InvokeAsync();
        }

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
            var cmd = this.CreateCommand<SearchArtistsCommand>();
            cmd.Location = new Location() { Latitude = latitude, Longitude = longitude };
            cmd.MaxDistance = maxdistance;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
        }

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
            var cmd = this.CreateCommand<TopArtistsCommand>();
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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
        public Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.CreateCommand<TopArtistsCommand>();
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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

            return this.GetTopArtistsForGenreAsync(genre.Id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets an artist by id
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <returns>A Response containing a Artist or an Error</returns>
        public async Task<Response<Artist>> GetArtistAsync(string id)
        {
            var cmd = this.CreateCommand<SearchCommand>();
            cmd.Id = id;
            ListResponse<MusicItem> result = await cmd.InvokeAsync();

            // Convert this list response to an artist
            if (result.Result != null && result.Count == 1 && result[0] as Artist != null)
            {
                return new Response<Artist>(result.StatusCode, result[0] as Artist, result.RequestId);
            }
            else if (result.Error != null)
            {
                return new Response<Artist>(result.StatusCode, result.Error, result.ErrorResponseBody, result.RequestId);
            }
            else
            {
                return new Response<Artist>(result.StatusCode, null, result.RequestId);
            }
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
        public Task<ListResponse<Artist>> GetSimilarArtistsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.CreateCommand<SimilarArtistsCommand>();
            cmd.ArtistId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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

            return this.GetSimilarArtistsAsync(artist.Id, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="category">The category.</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public Task<ListResponse<Product>> GetArtistProductsAsync(string id, Category? category = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.CreateCommand<ArtistProductsCommand>();
            cmd.ArtistId = id;
            cmd.Category = category;
            cmd.OrderBy = orderBy;
            cmd.SortOrder = sortOrder;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
        }

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        /// <exception cref="System.ArgumentNullException">artist;Artist cannot be null</exception>
        public Task<ListResponse<Product>> GetArtistProductsAsync(Artist artist, Category? category = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (artist == null)
            {
                throw new ArgumentNullException("artist", "Artist cannot be null");
            }

            return this.GetArtistProductsAsync(artist.Id, category, orderBy, sortOrder, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets products by id.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>
        /// A Response containing a Product or an Error
        /// </returns>
        public Task<Response<Product>> GetProductAsync(string id)
        {
            var cmd = this.CreateCommand<ProductCommand>();
            cmd.ProductId = id;
            return cmd.InvokeAsync();
        }

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

#if OPEN_INTERNALS
            return new Uri(string.Format("{0}{1}/products/{2}/sample/?domain=music&token={3}", this.BaseApiUri, this.CountryCode, id, this.ClientId), UriKind.Absolute);
#else
            return new Uri(string.Format("{0}{1}/products/{2}/sample/?domain=music&client_id={3}", this.BaseApiUri, this.CountryCode, id, this.ClientId), UriKind.Absolute);
#endif
        }

        /// <summary>
        /// Gets a similar product for the supplied product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        public Task<ListResponse<Product>> GetSimilarProductsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.CreateCommand<SimilarProductsCommand>();
            cmd.ProductId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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

            return this.GetSimilarProductsAsync(product.Id, startIndex, itemsPerPage);
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
        public Task<ListResponse<Product>> GetTopProductsAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.CreateCommand<TopProductsCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
        }

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
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.CreateCommand<TopProductsCommand>();
            cmd.Category = category;
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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

            return this.GetTopProductsForGenreAsync(genre.Id, category, startIndex, itemsPerPage);
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
        public Task<ListResponse<Product>> GetNewReleasesAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            var cmd = this.CreateCommand<NewReleasesCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
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
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.CreateCommand<NewReleasesCommand>();
            cmd.Category = category;
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
        }

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

            return this.GetNewReleasesForGenreAsync(genre.Id, category, startIndex, itemsPerPage);
        }

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <returns>
        /// A ListResponse containing Genres or an Error
        /// </returns>
        public Task<ListResponse<Genre>> GetGenresAsync()
        {
            var cmd = this.CreateCommand<GenresCommand>();
            return cmd.InvokeAsync();
        }

        /// <summary>
        /// Searches Nokia MixRadio
        /// </summary>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="category">Optional category.</param>
        /// <param name="genreId">Optional genre id</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <remarks>A searchTerm or genreId should be supplied</remarks>
        /// <returns>A ListResponse containing MusicItems or an Error</returns>
        public Task<ListResponse<MusicItem>> SearchAsync(string searchTerm = null, Category? category = null, string genreId = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null)
        {
            var cmd = this.CreateCommand<SearchCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.Category = category;
            cmd.GenreId = genreId;
            cmd.OrderBy = orderBy;
            cmd.SortOrder = sortOrder;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;

            this.SetRequestId(cmd, requestId);

            return cmd.InvokeAsync();
        }

        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public Task<ListResponse<string>> GetSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null)
        {
            var cmd = this.CreateCommand<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = false;

            this.SetRequestId(cmd, requestId);

            return cmd.InvokeAsync();
        }

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
            var cmd = this.CreateCommand<MixGroupsCommand>();
            cmd.ExclusiveTag = exclusiveTag;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return cmd.InvokeAsync();
        }

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
            var cmd = this.CreateCommand<MixesCommand>();
            cmd.MixGroupId = id;
            cmd.ExclusiveTag = exclusiveTag;
            return cmd.InvokeAsync();
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

            return this.GetMixesAsync(group.Id, exclusiveTag);
        }

#if SUPPORTS_USER_OAUTH
#if WINDOWS_PHONE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="browser">The browser control to use to drive authentication.</param>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        /// <remarks>Sorry, this method is messy due to the platform differences</remarks>
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, WebBrowser browser, CancellationToken? cancellationToken = null)
        {
            if (browser == null)
            {
                throw new ArgumentNullException("browser", "You must supply a web browser to allow user interaction");
            }

#elif NETFX_CORE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="oauthRedirectUri">The OAuth completed URI.</param>
        /// <returns>
        /// An AuthResultCode value indicating the result
        /// </returns>
        /// <remarks>
        /// Sorry, this method is messy due to the platform differences!
        /// </remarks>
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, string oauthRedirectUri = MusicClient.DefaultOAuthRedirectUri)
        {
            if (string.IsNullOrEmpty(oauthRedirectUri))
            {
                throw new ArgumentNullException("oauthRedirectUri", "You must supply your OAuth Redirect URI to allow user interaction");
            }

#endif
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "You must supply your app client secret obtained during app registration");
            }

            if (this._oauthFlowController != null && this._oauthFlowController.IsBusy)
            {
                throw new InvalidOperationException("An authentication call is in progress already");
            }

            // See if we have a cached token...
            AuthResultCode cachedResult = await this.AuthenticateUserAsync(clientSecret);
            if (cachedResult == AuthResultCode.Success)
            {
                return cachedResult;
            }

            var cmd = this.CreateCommand<GetAuthTokenCommand>();
            this.SetupSecureCommand(cmd);

            this._oauthFlowController = new OAuthUserFlow(this.ClientId, clientSecret, cmd);

#if WINDOWS_PHONE
            Response<AuthResultCode> response = await this._oauthFlowController.AuthenticateUserAsync(this.SecureBaseApiUri, scopes, browser, cancellationToken);
#elif NETFX_CORE
            Response<AuthResultCode> response = await this._oauthFlowController.AuthenticateUserAsync(new Uri(oauthRedirectUri), this.SecureBaseApiUri, scopes);
#endif
            await this.StoreOAuthToken(this._oauthFlowController.TokenResponse, clientSecret);
            this._oauthFlowController = null;

            if (response.Error != null)
            {
                throw response.Error;
            }
            else
            {
                return response.Result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a user token is cached and the silent version of AuthenticateUserAsync can be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a user token is cached; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> IsUserTokenCached()
        {
            return await StorageHelper.FileExistsAsync(TokenCacheFile);
        }

        /// <summary>
        /// Attempts a silent authentication a user to enable the user data APIs using a cached access token.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>
        /// This overload of AuthenticateUserAsync can only be used once the user has gone through the OAuth flow and given permission to access their data.
        /// </remarks>
        public async Task<AuthResultCode> AuthenticateUserAsync(string clientSecret)
        {
            if (this.IsUserAuthenticated)
            {
                return AuthResultCode.Success;
            }

            var cmd = this.CreateCommand<GetAuthTokenCommand>();
            this.SetupSecureCommand(cmd);

            // Attempt to load a cached token...
            string cachedToken = await StorageHelper.ReadTextAsync(TokenCacheFile);

            if (!string.IsNullOrEmpty(cachedToken))
            {
#if NETFX_CORE
                // Encrypt to stop prying eyes on Win8
                string decodedJson = EncryptionHelper.Decrypt(cachedToken, clientSecret, this.ClientId);
#else
                string decodedJson = cachedToken;
#endif

                this._oauthToken = TokenResponse.FromJToken(JToken.Parse(decodedJson));
                this.ExtractTokenProperties();

                // Check expiry...
                if (!this.IsUserTokenActive)
                {
                    if (this._oauthFlowController != null && this._oauthFlowController.IsBusy)
                    {
                        throw new InvalidOperationException("An authentication call is in progress already");
                    }

                    // expired -> need to Refresh and cache
                    this._oauthFlowController = new OAuthUserFlow(this.ClientId, clientSecret, cmd);
                    Response<AuthResultCode> response = await this._oauthFlowController.ObtainToken(null, this._oauthToken.RefreshToken, AuthResultCode.Unknown);
                    if (response.Result == AuthResultCode.Success && this._oauthFlowController.TokenResponse != null)
                    {
                        await this.StoreOAuthToken(this._oauthFlowController.TokenResponse, clientSecret);
                    }
                    else
                    {
                        // Failed to refresh the token - remove the cached token in case it's causing problems...
                        await StorageHelper.DeleteFileAsync(TokenCacheFile);
                        response = new Response<AuthResultCode>(null, AuthResultCode.FailedToRefresh, Guid.Empty);
                    }

                    this._oauthFlowController = null;

                    if (response.Error != null)
                    {
                        throw response.Error;
                    }
                    else
                    {
                        return response.Result;
                    }
                }
                else
                {
                    return AuthResultCode.Success;
                }
            }

            return AuthResultCode.NoCachedToken;
        }

        /// <summary>
        /// Deletes any cached authentication token.
        /// </summary>
        /// <returns>
        /// An async task
        /// </returns>
        public async Task DeleteAuthenticationTokenAsync()
        {
            this._oauthToken = null;
            this.AuthHeaderDataProvider = null;
            this.UserId = null;

            if (await StorageHelper.FileExistsAsync(TokenCacheFile))
            {
                await StorageHelper.DeleteFileAsync(TokenCacheFile);
            }
        }

        /// <summary>
        /// Gets the user play history.
        /// </summary>
        /// <param name="action">Type of the action.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>A list of play history events</returns>
        /// <remarks>Requires the ReadUserPlayHistory scope</remarks>
        public Task<ListResponse<UserEvent>> GetUserPlayHistoryAsync(UserEventAction? action = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage)
        {
            if (!this.IsUserAuthenticated)
            {
                throw new UserAuthRequiredException();
            }
            else
            {
                var cmd = this.CreateCommand<GetUserPlayHistoryCommand>();
                cmd.Action = action;
                cmd.StartIndex = startIndex;
                cmd.ItemsPerPage = itemsPerPage;
                this.SetupSecureCommand(cmd);
                return cmd.InvokeAsync();
            }
        }

        /// <summary>
        /// Gets the top artists chart for the user
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>
        /// A list of artists
        /// </returns>
        /// <remarks>
        /// Charts are available for the last week
        /// </remarks>
        public Task<ListResponse<Artist>> GetUserTopArtistsAsync(DateTime? startDate = null, DateTime? endDate = null, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage)
        {
            if (!this.IsUserAuthenticated)
            {
                throw new UserAuthRequiredException();
            }
            else
            {
                var cmd = this.CreateCommand<GetUserTopArtistsCommand>();
                if (startDate.HasValue)
                {
                    cmd.StartDate = startDate.Value;
                }

                if (endDate.HasValue)
                {
                    cmd.EndDate = endDate.Value;
                }

                cmd.ItemsPerPage = itemsPerPage;
                this.SetupSecureCommand(cmd);
                return cmd.InvokeAsync();
            }
        }

        /// <summary>
        /// Stores the OAuth token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>
        /// A Task for async execution
        /// </returns>
        private async Task StoreOAuthToken(TokenResponse token, string clientSecret)
        {
            if (this._oauthFlowController.TokenResponse != null)
            {
                // Grab results
                this._oauthToken = token;
                this.ExtractTokenProperties();

#if NETFX_CORE
                // Encrypt to stop prying eyes on Win8
                string content = EncryptionHelper.Encrypt(token.ToJToken().ToString(), clientSecret, this.ClientId);
#else
                string content = token.ToJToken().ToString();
#endif
                // store results in isostorage
                await StorageHelper.WriteTextAsync(TokenCacheFile, content);
            }
        }

        /// <summary>
        /// Extracts OAuth token properties.
        /// </summary>
        private void ExtractTokenProperties()
        {
            this.AuthHeaderDataProvider = new OAuthHeaderDataProvider(this._oauthToken.AccessToken);
            this.UserId = this._oauthToken.UserId.ToString("D");

            if (!string.IsNullOrEmpty(this._oauthToken.Territory))
            {
                this.CountryCode = this._oauthToken.Territory;
            }
        }

#endif
        #endregion

        /// <summary>
        /// Creates a command to execute
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>A Command to call</returns>
        private TCommand CreateCommand<TCommand>() where TCommand : MusicClientCommand, new()
        {
            return new TCommand
            {
                ClientSettings = this,
                RequestHandler = this.RequestHandler,
                BaseApiUri = this.BaseApiUri
            };
        }

#if SUPPORTS_OAUTH
        /// <summary>
        /// Adds settings required for building a secure command
        /// </summary>
        /// <typeparam name="TResponse">The type of the command.</typeparam>
        /// <param name="command">The command </param>
        private void SetupSecureCommand<TResponse>(SecureMusicClientCommand<TResponse> command)
            where TResponse : Response
        {
            command.OAuth2 = new OAuth2(this.AuthHeaderDataProvider);
            command.SecureApiBaseUri = this.SecureBaseApiUri;
            command.UserId = this.UserId;
        }

#endif
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

        /// <summary>
        /// Sets the request id for the given command
        /// </summary>
        /// <param name="cmd">The command</param>
        /// <param name="requestId">The request id</param>
        private void SetRequestId(MusicClientCommand cmd, Guid? requestId)
        {
            if (requestId.HasValue)
            {
                cmd.RequestId = requestId.Value;
            }
        }
    }
}