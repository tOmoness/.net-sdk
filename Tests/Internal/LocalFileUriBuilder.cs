// -----------------------------------------------------------------------
// <copyright file="LocalFileUriBuilder.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MixRadio.Commands;
using MixRadio.Internal;
using MixRadio.Internal.Request;

namespace MixRadio.Tests
{
    /// <summary>
    /// Local file URI builder for testing <c ref="ApiRequestHandler" />
    /// </summary>
    internal class LocalFileUriBuilder : IApiUriBuilder
    {
        private string _filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileUriBuilder" /> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public LocalFileUriBuilder(string filename)
        {
            this._filename = filename;
        }

        /// <summary>
        /// Builds an API URI
        /// </summary>
        /// <param name="command">The command to call.</param>
        /// <param name="settings">The music client settings.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        public Uri BuildUri(MusicClientCommand command, IMusicClientSettings settings, List<KeyValuePair<string, string>> querystringParams)
        {
            DirectoryInfo jsonDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\json"));
            if (jsonDir.Exists)
            {
                FileInfo[] json = jsonDir.GetFiles(this._filename);
                if (json.Length > 0)
                {
                    return new Uri("file://" + json[0].FullName.Replace(@"\", @"/"));
                }
            }

            throw new FileNotFoundException("Could not find required test file in " + jsonDir.FullName);
        }
    }
}
