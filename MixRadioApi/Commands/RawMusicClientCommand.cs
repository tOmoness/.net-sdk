// -----------------------------------------------------------------------
// <copyright file="RawMusicClientCommand.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Nokia.Music.Commands
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