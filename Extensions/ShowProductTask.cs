// -----------------------------------------------------------------------
// <copyright file="ShowProductTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show MixRadio Products
    /// </summary>
    public sealed class ShowProductTask : TaskBase
    {
        /// <summary>
        /// Gets or sets the Product ID.
        /// </summary>
        /// <value>
        /// The product ID.
        /// </value>
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        /// Shows the Product Page in MixRadio
        /// </summary>
        /// <returns>An async task to await</returns>
        public async Task Show()
        {
            if (!string.IsNullOrEmpty(this.ProductId))
            {
                await this.Launch(
                    new Uri(string.Format(Product.AppToAppShowUri, this.ProductId)),
                    new Uri(string.Format(Product.WebShowUri, this.ProductId))).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Please set a product ID before calling Show()");
            }
        }
    }
}
