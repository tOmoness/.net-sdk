// -----------------------------------------------------------------------
// <copyright file="DebugLogger.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;

namespace Nokia.Music.Internal
{
#if VERBOSE_LOGGING
    public
#else
    internal
#endif
    class DebugLogger
    {
        static DebugLogger()
        {
            Instance = new DebugLogger();
        }

        public static DebugLogger Instance
        {
            get;
#if !VERBOSE_LOGGING
            internal
#endif
            set;
        }

        public virtual void WriteLog(string message, params object[] args)
        {
            Debug.WriteLine(string.Format("NokiaMusicApi | {0}", message), args);
        }

        public void WriteVerboseInfo(string message, params object[] args)
        {
#if VERBOSE_LOGGING
            this.WriteLog(message, args);
#endif
        }
    }
}
