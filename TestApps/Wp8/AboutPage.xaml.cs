// -----------------------------------------------------------------------
// <copyright file="AboutPage.xaml.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MixRadio.TestApp
{
    /// <summary>
    /// The About Page
    /// </summary>
    public partial class AboutPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistPage" /> class.
        /// </summary>
        public AboutPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Opens web browser to show API documentation.
        /// </summary>
        /// <param name="sender">API docs button</param>
        /// <param name="e">Event arguments</param>
        private void OpenHelpDocs(object sender, RoutedEventArgs e)
        {
            if (sender as Button != null)
            {
                Uri link = new Uri((sender as Button).Tag.ToString());
                WebBrowserTask browser = new WebBrowserTask() { Uri = link };
                browser.Show();
            }
        }
    }
}