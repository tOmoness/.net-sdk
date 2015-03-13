using System;

using Xamarin.Forms;

namespace MixRadioActivity.Converters
{
	public class NiceDateConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var date = (DateTime)value;

			var diff = DateTime.UtcNow - date;
			if (diff.TotalSeconds < 60) {
				return "Just Now";
			}

			if (diff.TotalMinutes < 60) {
				return System.Convert.ToInt32(diff.TotalMinutes).ToString () + " minute(s) ago";
			}

			if (diff.TotalHours < 48) {
				return System.Convert.ToInt32(diff.TotalHours).ToString () + " hour(s) ago";
			}

			if (diff.TotalDays < 7) {
				return System.Convert.ToInt32(diff.TotalDays).ToString () + " day(s) ago";
			}

			return System.Convert.ToInt32(diff.TotalDays / 7).ToString () + " week(s) ago";
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}


