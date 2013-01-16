using Nokia.Music.Phone.Types;
using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;

namespace ArtistsAround
{
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

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method
        /// is called only in System.Windows.Data.BindingMode.TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The System.Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
