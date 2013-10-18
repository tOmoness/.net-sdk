// -----------------------------------------------------------------------
// <copyright file="AboutPanel.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The About Panel
    /// </summary>
    public sealed partial class AboutPanel : UserControl
    {
        public AboutPanel()
        {
            this.InitializeComponent();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }

            SettingsPane.Show();
        }

        private async void OpenWebLink(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((sender as FrameworkElement).Tag as string));
        }
    }
}
