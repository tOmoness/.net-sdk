using System;
using Xamarin.Forms;
using MixRadio.Types;

namespace MixRadioActivity.Converters
{
	public class ArtistNameConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var artists = value as Artist[];
			if(artists != null)
			{
				return artists [0].Name;
			}

			return null;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

