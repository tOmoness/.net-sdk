using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
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
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            this.InitializeComponent();
            this.InitializeMap(this.Map);
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Geolocator loc = new Geolocator();
            Geoposition pos = await loc.GetGeopositionAsync();
            if (pos != null && pos.Coordinate != null)
            {
                Map.Center = new GeoCoordinate(pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                Map.ZoomLevel = 10;

                // TODO: sign up for api appId and appCode at http://api.developer.nokia.com
                MusicClientAsync client = new MusicClientAsync(null, null);
                var res = await client.GetArtistsAroundLocation(pos.Coordinate.Latitude, pos.Coordinate.Longitude, 50, 0, 40);
                if (res.Result != null)
                {
                    this.MapItems.ItemsSource = res.Result;
                }
            }
        }
        
        private void PushPinClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("PushPinClicked: " + ((sender as Pushpin).Tag as Artist).Name);
            new ShowArtistTask() { ArtistId = ((sender as Pushpin).Tag as Artist).Id }.Show();
        }

        #region Map Helpers
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