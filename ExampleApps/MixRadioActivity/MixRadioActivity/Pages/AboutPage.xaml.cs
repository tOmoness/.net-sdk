using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Diagnostics;

namespace MixRadioActivity
{
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			InitializeComponent ();
			Device.OnPlatform (
				() => {
					this.Icon.Source = "Icon-60.png";
				},
				() => {
					this.Icon.Source = "icon.png";
					this.Note.TextColor = Color.Black;
				},
				() => {
					this.Icon.Source = "assets/logo.png";
					this.Note.TextColor = Color.Black;
                    this.LinkBtn.TextColor = Color.FromHex("#ff2e80");
                    this.Title = this.Title.ToUpperInvariant();
                }
			);
        }

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			this.Version.Text = "Version " + (App.Current as App).AppVersion;
		}

		public void OpenHelpDocs(object sender, EventArgs e)
		{
			Debug.WriteLine ("OpenHelpDocs");
			(App.Current as App).UriLauncher.LaunchUri (new Uri ("http://dev.mixrad.io/doc"));
		}
	}
}

