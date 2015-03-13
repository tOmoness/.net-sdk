using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Nokia.Music;
using Nokia.Music.Types;
using Xamarin.Forms;

namespace MixRadioActivity
{
    public class App : Application
    {
		private AuthHelper AuthHelper;
		private MusicClient MixRadioClient;

		public ActivityViewModel ActivityViewModel { get; private set; }
		public string AppVersion { get; private set; }
		public IUriLauncher UriLauncher { get; private set; }
        public bool WindowsDarkTheme { get; set; }

		public App(IAuthPlatformSpecific platformSpecificAuth, string appVersion, IUriLauncher uriLauncher)
        {
			this.MixRadioClient = new MusicClient (ApiKeys.ClientId);
			this.AuthHelper = new AuthHelper (platformSpecificAuth, this.MixRadioClient);
			this.ActivityViewModel = new ActivityViewModel (this.MixRadioClient, this.AuthHelper);
			this.AppVersion = appVersion;
			this.UriLauncher = uriLauncher;
			this.MainPage = new RootPage ();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
