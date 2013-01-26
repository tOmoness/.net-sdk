// -----------------------------------------------------------------------
// <copyright file="DebugLogger.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;

namespace Nokia.Music.Phone.Internal
{
    internal class DebugLogger
    {
        static DebugLogger()
        {
            Instance = new DebugLogger();
        }

        public static DebugLogger Instance { get; internal set; }

        public virtual void WriteLog(string message, params object[] args)
        {
            Debug.WriteLine(string.Format("NokiaMusicApi | {0}", message), args);
        }
    }
}
