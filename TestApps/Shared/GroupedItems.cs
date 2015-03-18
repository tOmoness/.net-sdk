// -----------------------------------------------------------------------
// <copyright file="GroupedItems.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// Represents grouped items
    /// </summary>
    public class GroupedItems
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedItems"/> class.
        /// </summary>
        public GroupedItems()
        {
            this.Items = new ObservableCollection<object>();
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public ObservableCollection<object> Items { get; set; }
    }
}
