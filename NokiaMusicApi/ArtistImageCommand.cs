// -----------------------------------------------------------------------
// <copyright file="ArtistImageCommand.cs" company="Nokia">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Nokia.Music.Internal.Response;

namespace Nokia.Music.Commands
{
    /// <summary>
    /// Artist Image command
    /// </summary>
    internal abstract class ArtistImageCommand : RawMusicClientCommand<Response<Uri>>
    {
        private Uri _responseUri;

        public int? Height { get; set; }

        public int? Width { get; set; }

        internal override bool FollowHttpRedirects
        {
            get { return false; }
        }

#if OPEN_INTERNALS
        public
#else
        internal
#endif
override void SetAdditionalResponseInfo(ResponseInfo responseInfo)
        {
            if (responseInfo.Headers.ContainsKey("Location"))
            {
                this._responseUri = new Uri(responseInfo.Headers["Location"].First());
            }
        }

        internal override void AppendUriPath(StringBuilder uri)
        {
            uri.Append(string.Format("creators/images/{0}/random/", this.GetSize()));
        }

        internal override Response<Uri> HandleRawResponse(Response<string> rawResponse)
        {
            return new Response<Uri>(rawResponse.StatusCode, this._responseUri, rawResponse.RequestId, rawResponse.FoundMixRadioHeader);
        }

        private string GetSize()
        {
            string size = "full";

            if (this.Width.HasValue)
            {
                if (!this.Height.HasValue)
                {
                    this.Height = this.Width;
                }

                size = string.Format(CultureInfo.InvariantCulture, "{0}x{1}", this.Width, this.Height);
            }

            return size;
        }
    }
}
