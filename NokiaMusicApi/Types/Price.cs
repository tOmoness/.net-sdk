// -----------------------------------------------------------------------
// <copyright file="Price.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json.Linq;

namespace Nokia.Music.Types
{
    /// <summary>
    /// Represents a Nokia Music Price
    /// </summary>
    public sealed class Price
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Price" /> class.
        /// </summary>
        public Price()
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
        /// Create a price object with specified parameters
        /// </summary>
        /// <param name="value">The price value</param>
        /// <param name="currency">The currency of the price</param>
        /// <returns>A Price object from specified parameters</returns>
        public static Price FromPriceInfo(double value, string currency)
        {
            return new Price()
            {
                Value = value,
                Currency = currency
            };
        }

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
