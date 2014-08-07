// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockMusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Nokia.Music.Commands;
using Nokia.Music.Tests.Internal;

namespace Nokia.Music.Tests.Commands
{
    internal class MockMusicClientCommand : RawMusicClientCommand<Response<string>>
    {
        internal MockMusicClientCommand()
        {
            this.ClientSettings = new MockMusicClientSettings("a", "gb", "en");
        }

        internal override Response<string> HandleRawResponse(Response<string> rawResponse)
        {
            return rawResponse;
        }
    }
}