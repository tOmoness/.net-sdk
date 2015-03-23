// -----------------------------------------------------------------------
// <copyright file="DebugLogger.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MixRadio.Internal
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
            Debug.WriteLine(string.Format("MixRadioApi | {0}", message), args);
        }

        public virtual void WriteException(Exception ex, params KeyValuePair<string, string>[] otherInfoItems)
        {
            Debug.WriteLine(ex == null ? "<null>" : ex.ToString());

            if (otherInfoItems != null && otherInfoItems.Length > 0)
            {
                var otherInfoItemsAsJObject = new JObject(otherInfoItems.Select(x => new JProperty(x.Key, x.Value)));

                Debug.WriteLine(otherInfoItemsAsJObject);
            }
        }

        public void WriteVerboseInfo(string message, params object[] args)
        {
#if VERBOSE_LOGGING
            this.WriteLog(message, args);
#endif
        }
    }
}