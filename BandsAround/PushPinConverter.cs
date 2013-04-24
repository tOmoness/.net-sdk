/*
 * Copyright © 2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Nokia.Music.Types;
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
