using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace MixRadioActivity
{
	public class TrackListPage : ListPage
	{
		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			Debug.WriteLine ("TrackListPage OnAppearing");
			this.SetList ("Track");

			var app = (App.Current as App);
			await app.ActivityViewModel.CheckUserAuthState ();
			await app.ActivityViewModel.StartPolling ();
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			Debug.WriteLine ("TrackListPage OnDisappearing");
			(App.Current as App).ActivityViewModel.StopPolling ();
		}
	}
}

