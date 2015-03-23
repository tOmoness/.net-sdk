// -----------------------------------------------------------------------
// <copyright file="MusicClientCommand{TIntermediate}.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio.Commands
{
    /// <summary>
    /// Defines the Music Client Command base class
    /// </summary>
    /// <typeparam name="TIntermediate">The type of the intermediate object.</typeparam>
#if OPEN_INTERNALS
        public
#else
    internal
#endif
 abstract class MusicClientCommand<TIntermediate> : MusicClientCommand
    {
        internal abstract TIntermediate HandleRawData(string rawData);
    }
}