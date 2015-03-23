// -----------------------------------------------------------------------
// <copyright file="RawMusicClientCommand.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MixRadio.Commands
{
    /// <summary>
    /// Defines the Music Client Raw Command base class
    /// </summary>
    /// <typeparam name="TResult">The type of the returned object.</typeparam>
    internal abstract class RawMusicClientCommand<TResult> : MusicClientCommand<string, TResult> 
        where TResult : Response
    {
        internal override string HandleRawData(string rawData)
        {
            return rawData;
        }
    }
}