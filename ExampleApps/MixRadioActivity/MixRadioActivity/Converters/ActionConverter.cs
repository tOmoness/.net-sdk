using System;
using Xamarin.Forms;
using MixRadio.Types;

namespace MixRadioActivity.Converters
{
	public class ActionConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var action = (UserEventAction)value;

			switch(action) {
				case UserEventAction.SkipNext:
					return "action_skipnext.png";
				case UserEventAction.Complete:
					return "action_complete.png";
				default:
					return null;
			}
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

