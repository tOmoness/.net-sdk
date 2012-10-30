// -----------------------------------------------------------------------
// <copyright file="ShowProductTask.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Nokia.Music.Phone.Internal;

namespace Nokia.Music.Phone.Tasks
{
    /// <summary>
    /// Provides a simple way to show Nokia Music Products
    /// </summary>
    public sealed class ShowProductTask : TaskBase
    {
        private string _appId = null;
        private string _productId = null;

        /// <summary>
        /// Gets or sets the optional App ID for passing through to Nokia Music.
        /// </summary>
        /// <value>
        /// The app ID.
        /// </value>
        public string AppId
        {
            get
            {
                return this._appId;
            }

            set
            {
                this._appId = value;
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
        /// Shows the Product Page in Nokia Music
        /// </summary>
        public void Show()
        {
            if (!string.IsNullOrEmpty(this._productId))
            {
                // Append the appId if one has been supplied...
                string appId;
                if (!string.IsNullOrEmpty(this._appId))
                {
                    appId = "&apikey=" + this._appId;
                }
                else
                {
                    appId = string.Empty;
                }

                this.Launch(
                    new Uri("nokia-music://show/product/?id=" + this._productId + appId),
                    new Uri("http://music.nokia.com/r/product/-/-/" + this._productId));
            }
            else
            {
                throw new InvalidOperationException("Please set a product ID before calling Show()");
            }
        }
    }
}
