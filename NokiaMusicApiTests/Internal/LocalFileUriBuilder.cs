// -----------------------------------------------------------------------
// <copyright file="LocalFileUriBuilder.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tests
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
        /// <param name="method">The method to call.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="appCode">The app code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="querystringParams">The querystring parameters.</param>
        /// <returns>
        /// A Uri to call
        /// </returns>
        public Uri BuildUri(ApiMethod method, string appId, string appCode, string countryCode, Dictionary<string, string> pathParams, Dictionary<string, string> querystringParams)
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
