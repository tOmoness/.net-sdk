using System;
using Xamarin.Forms;

namespace MixRadioActivity
{
	public class ArtistListPage : ListPage
	{
		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			this.SetList ("Artist");
		}
	}
}

