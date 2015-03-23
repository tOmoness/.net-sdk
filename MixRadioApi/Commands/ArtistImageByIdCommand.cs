// -----------------------------------------------------------------------
// <copyright file="ArtistImageByIdCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MixRadio.Commands
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
