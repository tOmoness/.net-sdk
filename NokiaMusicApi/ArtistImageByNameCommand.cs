// -----------------------------------------------------------------------
// <copyright file="ArtistImageByNameCommand.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Artist Image command
    /// </summary>
    internal class ArtistImageByNameCommand : ArtistImageCommand
    {
        public string ArtistName { get; set; }

        internal override List<KeyValuePair<string, string>> BuildQueryStringParams()
        {
            var encodedName = Uri.EscapeDataString(this.ArtistName);

            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("name", encodedName)
            };
        }
    }
}