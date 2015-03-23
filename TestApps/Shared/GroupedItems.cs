// -----------------------------------------------------------------------
// <copyright file="GroupedItems.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace MixRadio.TestApp
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
