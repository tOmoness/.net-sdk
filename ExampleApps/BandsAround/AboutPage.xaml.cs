using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace BandsAround
{
    /// <summary>
    /// Page for displaying application information.
    /// </summary>
    public partial class AboutPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutPage()
        {
            InitializeComponent();
            UpdateVersionString();
        }

        /// <summary>
        /// Updates VersionText to contain correct version info.
        /// </summary>
        private void UpdateVersionString()
        {
            string appVersion = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            VersionText.Text = "Version: "+ appVersion;
        }
    }
}