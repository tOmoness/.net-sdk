using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace MixRadioActivity.Converters
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
//				Debug.WriteLine ("BooleanToVisibilityConverter: value null");
				return false;
			}

			bool boolValue = (bool)value;
			if (parameter != null) {
				boolValue = !boolValue;
//				Debug.WriteLine ("BooleanToVisibilityConverter: inverting to " + boolValue.ToString ());
			}
			else {
//				Debug.WriteLine ("BooleanToVisibilityConverter: " + boolValue.ToString());
			}
			return boolValue;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

