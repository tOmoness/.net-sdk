// -----------------------------------------------------------------------
// <copyright file="MockBlankTerritoryCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MixRadio.Commands;

namespace MixRadio.Tests.Internal
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
    }
}
