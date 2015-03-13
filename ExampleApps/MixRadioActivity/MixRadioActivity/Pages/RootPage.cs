using System;
using Xamarin.Forms;

namespace MixRadioActivity
{
	public class RootPage : MasterDetailPage
	{
		public RootPage ()
		{
			var menuPage = new MenuPage ();
			menuPage.ParentPage = this;
			Master = menuPage;
			menuPage.ShowActivity (this, EventArgs.Empty);
			//Detail = new NavigationPage (new ActivityPage ());
		}
	}
}