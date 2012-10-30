// -----------------------------------------------------------------------
// <copyright file="Price.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Newtonsoft.Json.Linq;

namespace Nokia.Music.Phone.Types
{
    /// <summary>
    /// Represents a Nokia Music Price
    /// </summary>
    public sealed class Price
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Price" /> class.
        /// </summary>
        internal Price()
        {
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double Value { get; private set; }

        /// <summary>
        /// Creates a Price from a JSON Object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A Price object</returns>
        internal static Price FromJToken(JToken item)
        {
            return new Price()
            {
                Value = item.Value<double>("value"),
                Currency = item.Value<string>("currency")
            };
        }
    }
}
