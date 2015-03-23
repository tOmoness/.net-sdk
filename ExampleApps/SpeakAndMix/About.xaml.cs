// -----------------------------------------------------------------------
// <copyright file="About.xaml.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SpeakAndMix
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            var pkgVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            this.Version.Text = "Version " + string.Format("{0}.{1}", pkgVersion.Major, pkgVersion.Minor);

            HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
        }

        private async void OpenWebLink(object sender, RoutedEventArgs e)
        {
            var uri = (sender as FrameworkElement).Tag as string;
            App.LogAnalyticsEvent("WPAbout", "ShowWeb", uri, 0);
            await Launcher.LaunchUriAsync(new Uri(uri));
        }

        private async void OpenOtherApps(object sender, RoutedEventArgs e)
        {
            App.LogAnalyticsEvent("WPAbout", "ShowOtherApps", null, 0);
            await Launcher.LaunchUriAsync(new Uri("zune:search?publisher=Steve Robbins&contenttype=app"));
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
    }
}
