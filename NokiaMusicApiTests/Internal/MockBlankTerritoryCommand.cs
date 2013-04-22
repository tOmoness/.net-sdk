// -----------------------------------------------------------------------
// <copyright file="MockBlankTerritoryCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Commands;
using Nokia.Music.Internal;

namespace Nokia.Music.Tests.Internal
{
    /// <summary>
    /// Mock class to cover UseBlankTerritory property
    /// </summary>
    internal class MockBlankTerritoryCommand : MusicClientCommand
    {
        internal override bool UseBlankTerritory
        {
            get
            {
                return true;
            }
        }

        protected override void Execute()
        {
        }
    }
}
