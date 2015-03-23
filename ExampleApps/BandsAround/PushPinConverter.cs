using MixRadio.Types;
using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;

namespace BandsAround
{
    /// <summary>
    /// Value converter for artist origin Location -> GeoCoordinate conversion.
    /// </summary>
    public class PushPinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Location location = value as Location;
            if (location != null)
            {
                return new GeoCoordinate(location.Latitude, location.Longitude);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
