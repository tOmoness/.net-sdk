// -----------------------------------------------------------------------
// <copyright file="AboutPage.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
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