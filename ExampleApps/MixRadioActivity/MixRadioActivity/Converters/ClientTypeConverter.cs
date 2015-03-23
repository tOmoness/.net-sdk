using System;
using Xamarin.Forms;
using MixRadio.Types;

namespace MixRadioActivity.Converters
{
	public class ClientTypeConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var clientType = value as string;

			if (!string.IsNullOrEmpty (clientType)) {
				if (clientType.ToLowerInvariant ().Contains ("ios")) {
					return "device_ios.png";
				}
				if (clientType.ToLowerInvariant ().Contains ("android")) {
					return "device_android.png";
				}
				if (clientType.ToLowerInvariant ().Contains ("web")) {
					return "device_web.png";
				}
				if (clientType.ToLowerInvariant ().Contains ("wp") || clientType.ToLowerInvariant ().Contains ("windows")) {
					return "device_windows.png";
				}
				if (clientType.ToLowerInvariant ().Contains ("sonos")) {
					return "device_sonos.png";
				}
			}

			return "device_unknown.png";
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

