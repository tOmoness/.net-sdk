// -----------------------------------------------------------------------
// <copyright file="GenresCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Types;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    /// Gets the available genres
    /// </summary>
    internal sealed class GenresCommand : MusicClientCommand<ListResponse<Genre>>
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        protected override void Execute()
        {
            this.RequestHandler.SendRequestAsync(
                ApiMethod.Genres,
                this.MusicClientSettings.AppId,
                this.MusicClientSettings.AppCode,
                this.MusicClientSettings.CountryCode,
                null,
                null,
                (Response<JObject> rawResult) =>
                    {
                        this.CatalogItemResponseHandler(rawResult, ArrayNameItems, new JTokenConversionDelegate<Genre>(Genre.FromJToken), Callback);
                    });
        }
    }
}