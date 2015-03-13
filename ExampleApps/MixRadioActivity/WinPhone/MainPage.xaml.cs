using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace MixRadioActivity.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            Debug.WriteLine("MainPage created");

            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            var app = new MixRadioActivity.App(new AuthPlatformSpecific(), App.VersionNumber, new UriLauncher());
            double darkTheme = (double)Application.Current.Resources["PhoneDarkThemeOpacity"];
            app.WindowsDarkTheme = darkTheme == 1.0;
            Debug.WriteLine("app.WindowsDarkTheme = " + app.WindowsDarkTheme.ToString());
            LoadApplication(app);
        }
    }
}
