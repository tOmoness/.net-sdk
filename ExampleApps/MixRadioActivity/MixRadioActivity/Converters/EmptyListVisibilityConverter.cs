using System;
using System.Collections;
using System.Diagnostics;
using Xamarin.Forms;

namespace MixRadioActivity.Converters
{
	public class EmptyListVisibilityConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			ICollection collection = value as ICollection;
			if (collection != null)
			{
				//Debug.WriteLine("EmptyListVisibilityConverter collection.Count=" + collection.Count.ToString());
				if (parameter == null)
				{
					return collection.Count == 0;
				}
				else
				{
					return collection.Count != 0;
				}
			}

			return false;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}

