// -----------------------------------------------------------------------
// <copyright file="ArtistImageByIdCommand.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Artist Image command
    /// </summary>
    internal class ArtistImageByIdCommand : ArtistImageCommand
    {
        public string ArtistId { get; set; }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(MusicClientCommand.ParamId, this.ArtistId)
            };
        }
    }
}
