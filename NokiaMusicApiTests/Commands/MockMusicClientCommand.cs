// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockMusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Nokia.Music.Tests.Commands
{
    using Nokia.Music.Commands;
    using Nokia.Music.Tests.Internal;

    internal class MockMusicClientCommand : MusicClientCommand<Response<object>>
    {
        internal MockMusicClientCommand()
        {
            this.ClientSettings = new MockMusicClientSettings("a", "gb", "en");
        }

        protected override void Execute()
        {
        }
    }
}