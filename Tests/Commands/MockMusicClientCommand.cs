// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockMusicClientCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MixRadio;
using MixRadio.Commands;
using MixRadio.Tests.Internal;

namespace MixRadio.Tests.Commands
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