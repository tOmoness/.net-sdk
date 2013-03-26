/*
 * Copyright © 2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Nokia.Music.Phone;
using Nokia.Music.Phone.Tasks;
using Nokia.Music.Phone.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Geolocation;

namespace BandsAround
{
    /// <summary>
    /// Main Page of the application with the Map and Pushpin controls.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // Music API with which to request nearby artists
        MusicClientAsync client = null;

        // A collection of artists represented on the map with Pushpins.
        private ObservableCollection<Artist> nearbyArtists = null;

        // Maximum number of Pushpins on the map at the same time
        private const int MAX_ARTISTS_COUNT = 200;

        // Geolocator for getting device's current location.
        private Geolocator geoLocator = null;

        // GeoCoordinate of the previous artist request.
        private GeoCoordinate prevSearchCoordinate = null;

        // Progress indicator to be shown while fetching data.
        private ProgressIndicator busyIndicator = new ProgressIndicator();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeMap(this.Map);
            this.Loaded += MainPage_Loaded;

            busyIndicator.IsVisible = false;
            busyIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, busyIndicator);
        }

        /// <summary>
        /// Centers the map on current location at application launch.
        /// Subsequently only makes the map visible.
        /// </summary>
        /// <param name="sender">This page</param>
        /// <param name="e">Event arguments</param>
        private async void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Make Map visible again after returning from about page.
            Map.Visibility = Visibility.Visible;

            if (geoLocator == null)
            {
                geoLocator = new Geolocator();

                try
                {
                    Geoposition pos = await geoLocator.GetGeopositionAsync();

                    if (pos != null && pos.Coordinate != null)
                    {
                        Map.SetView(new GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude), 10);
                        GetNearbyArtists();
                    }
                }
                catch (Exception /*ex*/)
                {
                    // Couldn't get current location. Location may be disabled.
                    MessageBox.Show("Current location cannot be obtained. It is "
                                  + "recommended that location service is turned "
                                  + "on in phone settings when using Bands Around.\n"
                                  + "\nNow centering on London.");
                    Map.Center = new GeoCoordinate(51.51, -0.12);
                    Map.SetView(new GeoCoordinate(51.51, -0.12), 10);
                    GetNearbyArtists();
                }
            }
        }

        /// <summary>
        /// Open Nokia Music application to show selected artist.
        /// </summary>
        /// <param name="sender">Selected Pushpin</param>
        /// <param name="e">Event arguments</param>
        private void PushPinClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new ShowArtistTask() { 
                ArtistId = ((sender as Pushpin).Tag as Artist).Id 
            }.Show();
        }

        /// <summary>
        /// Navigates to About page. Hides the map to avoid flickering
        /// pushpins while navigating between pages.
        /// </summary>
        /// <param name="sender">About menu item</param>
        /// <param name="e">Event arguments</param>
        private void AboutClicked(object sender, EventArgs e)
        {
            Map.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Requests nearby artists after map has been moved to a new position.
        /// </summary>
        /// <param name="sender">Map</param>
        /// <param name="e">Event arguments</param>
        private void OnResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            GetNearbyArtists();
        }

        /// <summary>
        /// Requests artists and bands originating nearby map center.
        /// Each artist/band is represented on the map as Pushpin.
        /// </summary>
        /// <param name="sender">Refresh menu item</param>
        /// <param name="e">Event arguments</param>
        private async void GetNearbyArtists()
        {
            // Only one artist search can be ongoing at a time.
            if (busyIndicator.IsVisible)
            {
                return;
            }

            busyIndicator.IsVisible = true;
            prevSearchCoordinate = Map.Center;

            // Sign up for api appId and appCode at http://api.developer.nokia.com
            if (client == null)
            {
                client = new MusicClientAsync(null, null);
            }

            var res = await client.GetArtistsAroundLocation(Map.Center.Latitude, Map.Center.Longitude, 50, 0, 40);
            if (res.Result != null)
            {
                if (nearbyArtists == null)
                {
                    nearbyArtists = new ObservableCollection<Artist>();
                    this.MapItems.ItemsSource = nearbyArtists;
                }
                for (int i = 0; i < res.Result.Count; i++)
                {
                    int removeBandIndex = 0;

                    // Add unique artists until MAX_ARTISTS_COUNT is reached.
                    // Removal is made beginning from the oldest results,
                    // excluding artists in the result set.
                    if (!nearbyArtists.Contains(res.Result[i]))
                    {
                        nearbyArtists.Add(res.Result[i]);
                    }

                    if (nearbyArtists.Count > MAX_ARTISTS_COUNT)
                    {
                        while (res.Result.Contains(nearbyArtists[removeBandIndex]))
                        {
                            removeBandIndex++;
                            if (removeBandIndex > MAX_ARTISTS_COUNT)
                            {
                                break;
                            }
                        }
                        nearbyArtists.RemoveAt(removeBandIndex);
                    }
                }
            }

            busyIndicator.IsVisible = false;

            // Make a new request if the map center changed during operation.
            if (prevSearchCoordinate != Map.Center)
            {
                GetNearbyArtists();
            }
        }

        #region Map Helpers

        // These helpers enable the Pushpin control from Windows Phone Toolkit
        // to be used in representing the artists and bands on the Map control.
        private void InitializeMap(Map map)
        {
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(map);
            IEnumerable<FieldInfo> runtimeFields = this.GetType().GetRuntimeFields();

            foreach (DependencyObject i in children)
            {
                SetChildItemField(i, runtimeFields);
            }
        }

        private void SetChildItemField(DependencyObject obj, IEnumerable<FieldInfo> fields)
        {
            var info = obj.GetType().GetProperty("Name");

            if (info != null)
            {
                string name = (string)info.GetValue(obj);

                if (name != null)
                {
                    foreach (FieldInfo j in fields)
                    {
                        if (j.Name == name)
                        {
                            j.SetValue(this, obj);
                            break;
                        }
                    }
                }
            }

            Panel panelItem = obj as Panel;
            if (panelItem != null)
            {
                foreach (UIElement child in panelItem.Children)
                {
                    SetChildItemField(child, fields);
                }
            }
        }
        #endregion
    }
}