// -----------------------------------------------------------------------
// <copyright file="ShowProductTask.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Types;

namespace Nokia.Music.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia MixRadio Products
    /// </summary>
    public sealed class ShowProductTask : TaskBase
    {
        private string _clientId = null;
        private string _productId = null;

        /// <summary>
        /// Gets or sets the optional Client ID for passing through to Nokia MixRadio.
        /// </summary>
        /// <value>
        /// The client ID.
        /// </value>
        public string ClientId
        {
            get
            {
                return this._clientId;
            }

            set
            {
                this._clientId = value;
            }
        }

        /// <summary>
        /// Gets or sets the Product ID.
        /// </summary>
        /// <value>
        /// The product ID.
        /// </value>
        public string ProductId
        {
            get
            {
                return this._productId;
            }

            set
            {
                this._productId = value;
            }
        }

        /// <summary>
        /// Shows the Product Page in Nokia MixRadio
        /// </summary>
        public void Show()
        {
            if (!string.IsNullOrEmpty(this._productId))
            {
                // Append the clientId if one has been supplied...
                string clientId = string.Empty;
                if (!string.IsNullOrEmpty(this.ClientId))
                {
                    clientId = "?client_id=" + this.ClientId;
                }

                this.Launch(
                    new Uri(string.Format(Product.AppToAppShowUri, this._productId) + clientId),
                    new Uri(string.Format(Product.WebShowUri, this._productId)));
            }
            else
            {
                throw new InvalidOperationException("Please set a product ID before calling Show()");
            }
        }
    }
}
