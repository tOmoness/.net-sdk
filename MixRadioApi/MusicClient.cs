// -----------------------------------------------------------------------
// <copyright file="MusicClient.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MixRadio.Commands;
using MixRadio.Internal;
using MixRadio.Internal.Authorization;
using MixRadio.Internal.Request;
using MixRadio.Types;
using Newtonsoft.Json.Linq;

namespace MixRadio
{
    /// <summary>
    /// The MixRadio API client
    /// </summary>
    public sealed partial class MusicClient : IMusicClientSettings, IMusicClient
    {
        /// <summary>
        /// The default OAuth2 redirect URI
        /// </summary>
        public const string DefaultOAuthRedirectUri = "https://account.mixrad.io/authorize/complete";

        internal const int DefaultItemsPerPage = 10;
        internal const int DefaultSmallItemsPerPage = 3;
        internal const int DefaultStartIndex = 0;

        private TokenResponse _oauthToken = null;

        /// <summary>
        /// Initializes the <see cref="MusicClient"/> class.
        /// </summary>
        static MusicClient()
        {
            MusicClient.RequestTimeout = 30000;
        }

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
                new ApiRequestHandler(new ApiUriBuilder()))
        {
            this.CountryCodeBasedOnRegionInfo = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        public MusicClient(string clientId, string countryCode)
            : this(clientId, countryCode, null, new ApiRequestHandler(new ApiUriBuilder()))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MusicClient" /> class.
        /// </summary>
        /// <param name="clientId">The Application Client ID obtained during registration</param>
        /// <param name="countryCode"> The country code. </param>
        /// <param name="language"> The language code. </param>
        public MusicClient(string clientId, string countryCode, string language)
            : this(clientId, countryCode, language, new ApiRequestHandler(new ApiUriBuilder()))
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

            this.ApiBaseUrl = MusicClientCommand.DefaultBaseApiUri;
            this.SecureApiBaseUrl = MusicClientCommand.DefaultSecureBaseApiUri;

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

            this.CurrentMusicClientSettings = this;
        }

        /// <summary>
        /// Gets or sets the timeout duration for web requests.
        /// </summary>
        /// <value>
        /// The timeout duration in milliseconds.
        /// </value>
        public static int RequestTimeout
        {
            get;
            set;
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

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string ApiBaseUrl { get; private set; }

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string SecureApiBaseUrl { get; private set; }

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
                return this._oauthToken != null && !string.IsNullOrEmpty(this._oauthToken.AccessToken);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user token is active.
        /// </summary>
        /// <remarks>
        /// If the token is not active, it needs refreshing with RefreshAccessTokenAsync
        /// </remarks>
        /// <value>
        /// <c>true</c> if the user token is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserTokenActive
        {
            get
            {
                return this._oauthToken != null && this._oauthToken.ExpiresUtc > this.ServerTimeUtc;
            }
        }

        /// <summary>
        /// Gets the request handler in use for testing purposes.
        /// </summary>
        /// <value>
        /// The request handler.
        /// </value>
        internal IApiRequestHandler RequestHandler { get; private set; }

#if SUPPORTS_OAUTH
        /// <summary>
        /// Gets or sets the auth header data provider.
        /// </summary>
        /// <value>
        /// The auth header data provider.
        /// </value>
        internal IAuthHeaderDataProvider AuthHeaderDataProvider { get; set; }
#endif

        /// <summary>
        /// Gets or sets the latest MusicClientSettings
        /// </summary>
        private IMusicClientSettings CurrentMusicClientSettings { get; set; }

        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public async Task<ListResponse<Artist>> SearchArtistsAsync(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchArtistsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;

            this.SetRequestId(cmd, requestId);

            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public async Task<ListResponse<string>> GetArtistSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = true;

            this.SetRequestId(cmd, requestId);

            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets artists that originate around a specified location
        /// </summary>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="maxdistance">The max distance (in KM) around the location to search</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public async Task<ListResponse<Artist>> GetArtistsAroundLocationAsync(double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchArtistsCommand>();
            cmd.Location = new Location() { Latitude = latitude, Longitude = longitude };
            cmd.MaxDistance = maxdistance;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public async Task<ListResponse<Artist>> GetTopArtistsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<TopArtistsCommand>();
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public async Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "genre id cannot be null");
            }

            var cmd = this.CreateCommand<TopArtistsCommand>();
            cmd.GenreId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an artist by id
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A Response containing a Artist or an Error</returns>
        public async Task<Response<Artist>> GetArtistAsync(string id, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchCommand>();
            cmd.Id = id;
            ListResponse<MusicItem> result = await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);

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
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Artists or an Error
        /// </returns>
        public async Task<ListResponse<Artist>> GetSimilarArtistsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SimilarArtistsCommand>();
            cmd.ArtistId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public async Task<ListResponse<Product>> GetArtistProductsAsync(string id, Category? category = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<ArtistProductsCommand>();
            cmd.ArtistId = id;
            cmd.Category = category;
            cmd.OrderBy = orderBy;
            cmd.SortOrder = sortOrder;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets products by id.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A Response containing a Product or an Error
        /// </returns>
        public async Task<Response<Product>> GetProductAsync(string id, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<ProductCommand>();
            cmd.ProductId = id;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a mix by id
        /// </summary>
        /// <param name="id">The mix id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A Mix or an Error
        /// </returns>
        public async Task<Mix> GetMixAsync(string id, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<MixDetailsCommand>();
            cmd.Id = id;

            var response = await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
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
            return new Uri(string.Format("{0}{1}{2}/products/{3}/sample/?domain=music&token={4}", this.ApiBaseUrl, MusicClientCommand.DefaultApiVersion, this.CountryCode, id, this.ClientId), UriKind.Absolute);
#else
            return new Uri(string.Format("{0}{1}{2}/products/{3}/sample/?domain=music&client_id={4}", this.ApiBaseUrl, MusicClientCommand.DefaultApiVersion, this.CountryCode, id, this.ClientId), UriKind.Absolute);
#endif
        }

        /// <summary>
        /// Gets a similar product for the supplied product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        public async Task<ListResponse<Product>> GetSimilarProductsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SimilarProductsCommand>();
            cmd.ProductId = id;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public async Task<ListResponse<Product>> GetTopProductsAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<TopProductsCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public async Task<ListResponse<Product>> GetTopProductsForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
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
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        public async Task<ListResponse<Product>> GetNewReleasesAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<NewReleasesCommand>();
            cmd.Category = category;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="id">The genre id.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        /// <exception cref="System.ArgumentNullException">genre;genre cannot be null</exception>
        public async Task<ListResponse<Product>> GetNewReleasesForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
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
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Genres or an Error
        /// </returns>
        public async Task<ListResponse<Genre>> GetGenresAsync(CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<GenresCommand>();
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Searches MixRadio
        /// </summary>
        /// <param name="searchTerm">Optional search term.</param>
        /// <param name="category">Optional category.</param>
        /// <param name="genreId">Optional genre id</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <remarks>A searchTerm or genreId should be supplied</remarks>
        /// <returns>A ListResponse containing MusicItems or an Error</returns>
        public async Task<ListResponse<MusicItem>> SearchAsync(string searchTerm = null, Category? category = null, string genreId = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null)
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

            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Searches for tracks with a Beats per Minute range.
        /// </summary>
        /// <param name="minBpm">The minimum BPM.</param>
        /// <param name="maxBpm">The maximum BPM.</param>
        /// <param name="genreId">The genre identifier.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A list of tracks
        /// </returns>
        public async Task<ListResponse<MusicItem>> SearchBpmAsync(int minBpm, int maxBpm, string genreId = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchCommand>();
            cmd.MinBpm = minBpm;
            cmd.MaxBpm = maxBpm;
            cmd.Category = Category.Track;
            cmd.GenreId = genreId;
            cmd.OrderBy = orderBy;
            cmd.SortOrder = sortOrder;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;

            this.SetRequestId(cmd, requestId);

            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing search suggestions
        /// </returns>
        public async Task<ListResponse<string>> GetSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<SearchSuggestionsCommand>();
            cmd.SearchTerm = searchTerm;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.SuggestArtists = false;

            this.SetRequestId(cmd, requestId);

            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        public async Task<ListResponse<MixGroup>> GetMixGroupsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            return await this.GetMixGroupsAsync(string.Empty, startIndex, itemsPerPage, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing MixGroups or an Error
        /// </returns>
        public async Task<ListResponse<MixGroup>> GetMixGroupsAsync(string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<MixGroupsCommand>();
            cmd.ExclusiveTag = exclusiveTag;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="showFeaturedArtists">Show the featuredArtists list?</param>
        /// <param name="cancellationToken">An optional CancellationToken</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public async Task<ListResponse<Mix>> GetMixesAsync(string id, bool showFeaturedArtists = false, CancellationToken? cancellationToken = null)
        {
            return await this.GetMixesAsync(id, (string)null, showFeaturedArtists, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="showFeaturedArtists">Show the featuredArtists list?</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public async Task<ListResponse<Mix>> GetMixesAsync(string id, string exclusiveTag, bool showFeaturedArtists = false, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "A group id must be supplied");
            }

            var cmd = this.CreateCommand<MixesCommand>();
            cmd.MixGroupId = id;
            cmd.ExclusiveTag = exclusiveTag;
            cmd.ShowFeaturedArtists = showFeaturedArtists;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all Mixes available, regardless of grouping
        /// </summary>
        /// <param name="exclusiveTag">The optional exclusivity tag.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="showFeaturedArtists">Show the featuredArtists list?</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        public async Task<ListResponse<Mix>> GetAllMixesAsync(string exclusiveTag = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, bool showFeaturedArtists = false, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<MixesCommand>();
            cmd.ExclusiveTag = exclusiveTag;
            cmd.StartIndex = startIndex;
            cmd.ItemsPerPage = itemsPerPage;
            cmd.ShowFeaturedArtists = showFeaturedArtists;
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing the available languages</returns>
        public async Task<ListResponse<Language>> GetLanguagesAsync(CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<LanguagesCommand>();
            return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an authentication URI for a given scope
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        /// <returns>
        /// An authentication URI
        /// </returns>
        public Uri GetAuthenticationUri(Scope scopes)
        {
            var uri = new Uri(string.Format("{0}{1}authorize/?response_type=code&client_id={2}&scope={3}", this.SecureApiBaseUrl, MusicClientCommand.DefaultApiVersion, Uri.EscapeDataString(this.ClientId), scopes.AsStringParam().Replace(" ", "+")));
            DebugLogger.Instance.WriteLog("GetAuthenticationUri {0}", uri);
            return uri;
        }

        /// <summary>
        /// Attempts to get an authentication token from an auth code.
        /// </summary>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="authCode">The authentication code obtained from a call to GetAuthenticationUri.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>An authentication token</returns>
        public async Task<AuthToken> GetAuthenticationTokenAsync(string clientSecret, string authCode, CancellationToken? cancellationToken = null)
        {
            if (this.IsUserAuthenticated && this.IsUserTokenActive)
            {
                return AuthToken.FromTokenResponse(this._oauthToken);
            }

            var response = await this.GetAuthenticationTokenInternal(clientSecret, authCode, null, cancellationToken);
            this._oauthToken = response;
            this.ExtractTokenProperties();
            return AuthToken.FromTokenResponse(this._oauthToken);
        }

        /// <summary>
        /// For PCL apps (for example, Xamarin), allow the authentication
        /// to be undertaken separately within the consuming app
        /// </summary>
        /// <param name="authToken">The AuthToken</param>
        public void SetAuthenticationToken(AuthToken authToken)
        {
            if (authToken != null)
            {
                this._oauthToken = authToken.ToTokenResponse();
                this.ExtractTokenProperties();
            }
            else
            {
                this._oauthToken = null;
                this.AuthHeaderDataProvider = null;
            }
        }

        /// <summary>
        /// Attempts to refresh an access token.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>
        /// This method should be called after SetAuthenticationTokenDetails if the IsUserTokenActive property returns false
        /// </remarks>
        public async Task<AuthToken> RefreshAuthenticationTokenAsync(string clientSecret, CancellationToken? cancellationToken = null)
        {
            if (this._oauthToken == null)
            {
                throw new UserAuthRequiredException();
            }

            if (this.IsUserAuthenticated && this.IsUserTokenActive)
            {
                return AuthToken.FromTokenResponse(this._oauthToken);
            }

            var response = await this.GetAuthenticationTokenInternal(clientSecret, null, this._oauthToken.RefreshToken, cancellationToken);
            this._oauthToken = response;
            this.ExtractTokenProperties();
            return AuthToken.FromTokenResponse(this._oauthToken);
        }

        /// <summary>
        /// Gets the user play history.
        /// </summary>
        /// <param name="action">Type of the action.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A list of play history events</returns>
        /// <remarks>Requires the ReadUserPlayHistory scope</remarks>
        public async Task<ListResponse<UserEvent>> GetUserPlayHistoryAsync(UserEventAction? action = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null)
        {
            if (!this.IsUserTokenActive)
            {
                throw new UserAuthRequiredException();
            }
            else
            {
                var cmd = this.CreateCommand<GetUserPlayHistoryCommand>();
                cmd.Action = action;
                cmd.StartIndex = startIndex;
                cmd.ItemsPerPage = itemsPerPage;
                await this.SetupSecureCommandAsync(cmd).ConfigureAwait(false);
                return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the top artists chart for the user
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A list of artists
        /// </returns>
        /// <remarks>
        /// Charts are available for the last week
        /// </remarks>
        public async Task<ListResponse<Artist>> GetUserTopArtistsAsync(DateTime? startDate = null, DateTime? endDate = null, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, CancellationToken? cancellationToken = null)
        {
            if (!this.IsUserTokenActive)
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
                await this.SetupSecureCommandAsync(cmd).ConfigureAwait(false);
                return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a list of recent mixes played by the user
        /// </summary>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A list of mixes
        /// </returns>
        /// <remarks>
        /// Charts are available for the last week
        /// </remarks>
        public async Task<ListResponse<Mix>> GetUserRecentMixesAsync(int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, CancellationToken? cancellationToken = null)
        {
            if (!this.IsUserTokenActive)
            {
                throw new UserAuthRequiredException();
            }
            else
            {
                var cmd = this.CreateCommand<GetUserRecentMixesCommand>();

                cmd.ItemsPerPage = itemsPerPage;
                await this.SetupSecureCommandAsync(cmd).ConfigureAwait(false);
                return await cmd.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Extracts OAuth token properties.
        /// </summary>
        private void ExtractTokenProperties()
        {
#if SUPPORTS_USER_OAUTH
            this.AuthHeaderDataProvider = new OAuthHeaderDataProvider(this._oauthToken.AccessToken, this._oauthToken.UserId.ToString("D"));

            if (!string.IsNullOrEmpty(this._oauthToken.Territory))
            {
                this.CountryCode = this._oauthToken.Territory;
                this.CountryCodeBasedOnRegionInfo = false;
            }
#endif
        }

        /// <summary>
        /// Gets an authentication token either from an authorizationCode or a refreshToken.
        /// </summary>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>The token</returns>
        private async Task<TokenResponse> GetAuthenticationTokenInternal(string clientSecret, string authorizationCode, string refreshToken, CancellationToken? cancellationToken = null)
        {
            var cmd = this.CreateCommand<GetAuthTokenCommand>();
            await this.SetupSecureCommandAsync(cmd).ConfigureAwait(false);

            cmd.AuthorizationCode = authorizationCode;
            cmd.ClientId = this.ClientId;
            cmd.ClientSecret = clientSecret;
            cmd.RefreshToken = refreshToken;

            var tokenResponse = await cmd.ExecuteAsync(cancellationToken);
            if (tokenResponse.Error != null)
            {
                throw tokenResponse.Error;
            }
            else
            {
                return tokenResponse.Result;
            }
        }

        /// <summary>
        /// Creates a command to execute
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <returns>A Command to call</returns>
        private TCommand CreateCommand<TCommand>() where TCommand : MusicClientCommand, new()
        {
            return new TCommand
            {
                ClientSettings = this.CurrentMusicClientSettings,
                RequestHandler = this.RequestHandler,
                BaseApiUri = this.ApiBaseUrl
            };
        }

        /// <summary>
        /// Adds settings required for building a secure command
        /// </summary>
        /// <typeparam name="TIntermediate">The type of the intermediate object.</typeparam>
        /// <typeparam name="TResult">The type of the returned object.</typeparam>
        /// <param name="command">The command </param>
        /// <param name="requiresOauth">If true add oAuth headers.  If false does not add oAuth headers</param>
        /// <returns>A task</returns>
        private async Task SetupSecureCommandAsync<TIntermediate, TResult>(MusicClientCommand<TIntermediate, TResult> command, bool requiresOauth = true)
            where TResult : Response
        {
            if (requiresOauth && this.AuthHeaderDataProvider != null)
            {
                command.UserId = await this.AuthHeaderDataProvider.GetUserIdAsync().ConfigureAwait(false);
                command.OAuth2 = new OAuth2(this.AuthHeaderDataProvider);
            }

            command.BaseApiUri = this.SecureApiBaseUrl;
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