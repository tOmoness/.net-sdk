// -----------------------------------------------------------------------
// <copyright file="SimilarProductsCommand.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Nokia.Music.Phone.Internal;
using Nokia.Music.Phone.Internal.Parsing;

namespace Nokia.Music.Phone.Commands
{
    /// <summary>
    ///  Gets similar products to the supplied product
    /// </summary>
    internal sealed class SimilarProductsCommand : ProductCommand
    {
        protected override IJsonProcessor JsonProcessor
        {
            get
            {
                return new NamedItemListJsonProcessor();
            }
        }

        /// <summary>
        /// Appends the uri subpath and parameters specific to this API method
        /// </summary>
        /// <param name="uri">The base uri</param>
        internal override void AppendUriPath(System.Text.StringBuilder uri)
        {
            uri.AppendFormat("products/{0}/similar", this.ProductId);
        }
    }
}