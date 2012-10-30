// -----------------------------------------------------------------------
// <copyright file="AboutPage.xaml.cs" company="Nokia">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace Nokia.Music.TestApp
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