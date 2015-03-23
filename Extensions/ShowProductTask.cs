// -----------------------------------------------------------------------
// <copyright file="ShowProductTask.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MixRadio.Types;

namespace MixRadio.Tasks
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
                var product = new Product { Id = this.ProductId };
                await this.Launch(product.AppToAppUri, product.WebUri).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("Please set a product ID before calling Show()");
            }
        }
    }
}
