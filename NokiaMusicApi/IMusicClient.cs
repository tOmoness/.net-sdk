// -----------------------------------------------------------------------
// <copyright file="IMusicClient.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif
using Nokia.Music.Types;
#if WINDOWS_PHONE_APP
using Windows.Security.Authentication.Web;
#endif

namespace Nokia.Music
{
    /// <summary>
    /// Defines the MixRadio API
    /// </summary>
    public partial interface IMusicClient
    {
        /// <summary>
        /// Searches for an Artist
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> SearchArtistsAsync(string searchTerm, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets artist search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing search suggestions</returns>
        Task<ListResponse<string>> GetArtistSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets artists that originate around a specified location
        /// </summary>
        /// <param name="latitude">The latitude to search around</param>
        /// <param name="longitude">The longitude to search around</param>
        /// <param name="maxdistance">The max distance (in KM) around the location to search</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetArtistsAroundLocationAsync(double latitude, double longitude, int maxdistance = 10, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the top artists
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="id">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the top artists for a genre
        /// </summary>
        /// <param name="genre">The genre to get results for.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetTopArtistsForGenreAsync(Genre genre, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets an artist by id
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A Response containing a Artist or an Error</returns>
        Task<Response<Artist>> GetArtistAsync(string id, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="id">The artist id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetSimilarArtistsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets similar artists for an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Artists or an Error</returns>
        Task<ListResponse<Artist>> GetSimilarArtistsAsync(Artist artist, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

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
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetArtistProductsAsync(string id, Category? category = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets products by an artist.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="category">The category.</param>
        /// <param name="orderBy">The field to sort the items by.</param>
        /// <param name="sortOrder">The sort order of the items.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetArtistProductsAsync(Artist artist, Category? category = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a product by id
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A Response containing a Product or an Error</returns>
        Task<Response<Product>> GetProductAsync(string id, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a mix by id
        /// </summary>
        /// <param name="id">The mix id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A Mix or an Error</returns>
        Task<Mix> GetMixAsync(string id, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a track sample uri.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>
        /// A uri to a sample clip of the track
        /// </returns>
        Uri GetTrackSampleUri(string id);

        /// <summary>
        /// Gets similar products for the supplied product.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetSimilarProductsAsync(string id, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets similar products for the supplied product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetSimilarProductsAsync(Product product, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetTopProductsAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

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
        Task<ListResponse<Product>> GetTopProductsForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a chart
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track charts are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetTopProductsForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Products or an Error</returns>
        Task<ListResponse<Product>> GetNewReleasesAsync(Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

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
        Task<ListResponse<Product>> GetNewReleasesForGenreAsync(string id, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a list of new releases
        /// </summary>
        /// <param name="genre">The genre.</param>
        /// <param name="category">The category - only Album and Track lists are available.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Products or an Error
        /// </returns>
        Task<ListResponse<Product>> GetNewReleasesForGenreAsync(Genre genre, Category category, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the available genres
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Genres or an Error</returns>
        Task<ListResponse<Genre>> GetGenresAsync(CancellationToken? cancellationToken = null);

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
        Task<ListResponse<MusicItem>> SearchAsync(string searchTerm = null, Category? category = null, string genreId = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null);

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
        /// <returns>A list of tracks</returns>
        Task<ListResponse<MusicItem>> SearchBpmAsync(int minBpm, int maxBpm, string genreId = null, OrderBy? orderBy = null, SortOrder? sortOrder = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets search suggestions.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="requestId">Id of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing search suggestions</returns>
        Task<ListResponse<string>> GetSearchSuggestionsAsync(string searchTerm, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, Guid? requestId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the Mix Groups available
        /// </summary>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing MixGroups or an Error</returns>
        Task<ListResponse<MixGroup>> GetMixGroupsAsync(int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

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
        Task<ListResponse<MixGroup>> GetMixGroupsAsync(string exclusiveTag, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets all Mixes available, regardless of grouping
        /// </summary>
        /// <param name="exclusiveTag">The optional exclusivity tag.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        Task<ListResponse<Mix>> GetAllMixesAsync(string exclusiveTag = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Mixes or an Error</returns>
        Task<ListResponse<Mix>> GetMixesAsync(string id, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="id">The mix group id.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        Task<ListResponse<Mix>> GetMixesAsync(string id, string exclusiveTag, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing Mixes or an Error</returns>
        Task<ListResponse<Mix>> GetMixesAsync(MixGroup group, CancellationToken? cancellationToken = null);
        
        /// <summary>
        /// Gets the Mixes available in a group
        /// </summary>
        /// <param name="group">The mix group.</param>
        /// <param name="exclusiveTag">The exclusive tag.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// A ListResponse containing Mixes or an Error
        /// </returns>
        Task<ListResponse<Mix>> GetMixesAsync(MixGroup group, string exclusiveTag, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A ListResponse containing the available languages</returns>
        Task<ListResponse<Language>> GetLanguagesAsync(CancellationToken? cancellationToken = null);

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
        /// An AuthResultCode indicating the result
        /// </returns>
        Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, WebBrowser browser, CancellationToken? cancellationToken = null);

#endif
#if NETFX_CORE
        /// <summary>
        /// Authenticates a user to enable the user data APIs.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="scopes">The scopes requested.</param>
        /// <param name="oauthRedirectUri">The OAuth completed URI.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, Scope scopes, string oauthRedirectUri = MusicClient.DefaultOAuthRedirectUri, CancellationToken? cancellationToken = null);

#endif
#if !PORTABLE
        /// <summary>
        /// Attempts to authenticate a user to enable the user data APIs using a cached access token.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>This overload of AuthenticateUserAsync can only be used once the user has gone through the OAuth flow and given permission to access their data</remarks>
        Task<AuthResultCode> AuthenticateUserAsync(string clientSecret, CancellationToken? cancellationToken = null);

#endif
#if WINDOWS_PHONE_APP
        /// <summary>
        /// Completes the authenticate user call.
        /// </summary>
        /// <param name="clientSecret">The client secret obtained during app registration</param>
        /// <param name="result">The result received through LaunchActivatedEventArgs.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// An AuthResultCode indicating the result
        /// </returns>
        /// <remarks>
        /// This method is for Windows Phone 8.1 use
        /// </remarks>
        Task<AuthResultCode> CompleteAuthenticateUserAsync(string clientSecret, WebAuthenticationResult result, CancellationToken? cancellationToken = null);

#endif
#if !PORTABLE
        /// <summary>
        /// Deletes any cached authentication token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>An async task</returns>
        Task DeleteAuthenticationTokenAsync(CancellationToken? cancellationToken = null);

#endif
#if PORTABLE
        /// <summary>
        /// For PCL apps (for example, Xamarin), allow the authentication
        /// to be undertaken separately within the consuming app for now
        /// </summary>
        /// <param name="accessToken">The OAuth2 access_token</param>
        /// <param name="expiresIn">When the token expires</param>
        /// <param name="refreshToken">The OAuth2 refresh_token</param>
        /// <param name="userId">The user id</param>
        /// <param name="territory">The user's territory</param>
        void SetAuthenticationTokenDetails(string accessToken, int expiresIn, string refreshToken, string userId, string territory);

#endif
        /// <summary>
        /// Gets the user play history.
        /// </summary>
        /// <param name="action">The event action to filter results by.</param>
        /// <param name="startIndex">The zero-based start index to fetch items from (e.g. to get the second page of 10 items, pass in 10).</param>
        /// <param name="itemsPerPage">The number of items to fetch.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A list of play history events</returns>
        /// <remarks>Requires the ReadUserPlayHistory scope</remarks>
        Task<ListResponse<UserEvent>> GetUserPlayHistoryAsync(UserEventAction? action = null, int startIndex = MusicClient.DefaultStartIndex, int itemsPerPage = MusicClient.DefaultItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets the top artists chart for the user
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A list of artists</returns>
        /// <remarks>Charts are available for the last week</remarks>
        Task<ListResponse<Artist>> GetUserTopArtistsAsync(DateTime? startDate = null, DateTime? endDate = null, int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Gets a list of recent mixes played by the user
        /// </summary>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>A list of mixes</returns>
        /// <remarks>Charts are available for the last week</remarks>
        Task<ListResponse<Mix>> GetUserRecentMixesAsync(int itemsPerPage = MusicClient.DefaultSmallItemsPerPage, CancellationToken? cancellationToken = null);
#endif
    }
}
