// -----------------------------------------------------------------------
// <copyright file="ParseHelper.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MixRadio.Internal.Parsing
{
    internal static class ParseHelper
    {
        /// <summary>
        /// Returns a matching enum value for specified string
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The response value</param>
        /// <param name="defaultValue">The default value to return if unable to parse</param>
        /// <returns>The matching value or the default value which should be unknown or none</returns>
        internal static T ParseEnumOrDefault<T>(string value, T defaultValue = default(T))
            where T : struct
        {
            if (value != null)
            {
                T result;

                if (Enum.TryParse(value, true, out result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Loads a JObject from the json.
        /// If you use JObject.Parse it automatically changes the date to utc.
        /// This keeps the date as it is in the json, allowing us to use local dates.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The jobject</returns>
        internal static JObject ParseWithDate(string json)
        {
            JsonReader reader = new JsonTextReader(new StringReader(json));
            reader.DateParseHandling = DateParseHandling.None;
            return JObject.Load(reader);
        }
    }
}
