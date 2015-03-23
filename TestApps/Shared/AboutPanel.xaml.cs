// -----------------------------------------------------------------------
// <copyright file="AboutPanel.xaml.cs" company="MixRadio">
// Copyright (c) 2015, MixRadio
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.System;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MixRadio.TestApp
{
    /// <summary>
    /// The About Panel
    /// </summary>
    public sealed partial class AboutPanel : UserControl
    {
        public AboutPanel()
        {
            this.InitializeComponent();
#if WINDOWS_APP
            this.WinHeader.Visibility = Visibility.Visible;
#else
            this.WinHeader.Visibility = Visibility.Collapsed;
#endif
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
#if WINDOWS_APP
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }

            SettingsPane.Show();
#endif
        }

        private async void OpenWebLink(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((sender as FrameworkElement).Tag as string));
        }
    }
}
