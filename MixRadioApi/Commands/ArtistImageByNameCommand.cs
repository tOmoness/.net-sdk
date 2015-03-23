// -----------------------------------------------------------------------
// <copyright file="ArtistImageByNameCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MixRadio.Commands
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