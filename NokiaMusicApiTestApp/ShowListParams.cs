// -----------------------------------------------------------------------
// <copyright file="ShowListParams.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// Defines method calls this page can get data with.
    /// </summary>
    internal enum MethodCall
    {
        /// <summary>
        /// An unknown method
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Get Top Artists
        /// </summary>
        GetTopArtists = 1,

        /// <summary>
        /// Get Top Artists for a genre
        /// </summary>
        GetTopArtistsForGenre = 2,

        /// <summary>
        /// Get the available genres
        /// </summary>
        GetGenres = 3,

        /// <summary>
        /// Get the available mix groups
        /// </summary>
        GetMixGroups = 4,

        /// <summary>
        /// Get the available mixes in a group
        /// </summary>
        GetMixes = 5,

        /// <summary>
        /// Get Top Albums
        /// </summary>
        GetTopAlbums = 6,

        /// <summary>
        /// Get New Albums
        /// </summary>
        GetNewAlbums = 7,

        /// <summary>
        /// Search for items
        /// </summary>
        Search = 10,
    }

    /// <summary>
    /// Holds parameters for the ShowList page
    /// </summary>
    internal class ShowListParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowListParams"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public ShowListParams(MethodCall method)
            : this(method, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowListParams"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="id">The id.</param>
        public ShowListParams(MethodCall method, string id)
            : this(method, id, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowListParams" /> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="id">The id.</param>
        /// <param name="parameter">The parameter.</param>
        public ShowListParams(MethodCall method, string id, object parameter)
        {
            this.Method = method;
            this.Id = id;
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public MethodCall Method { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public object Parameter { get; set; }
    }
}
