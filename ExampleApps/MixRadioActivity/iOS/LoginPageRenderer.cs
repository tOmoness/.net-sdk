using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MixRadioActivity;
using MixRadioActivity.iOS;
using Nokia.Music.Types;
using PCLStorage;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer (typeof (LoginPage), typeof (LoginPageRenderer))]

namespace MixRadioActivity.iOS
{
	public class LoginPageRenderer : PageRenderer
	{

		bool IsShown;

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Fixed the issue that on iOS 8, the modal wouldn't be popped.
			// url : http://stackoverflow.com/questions/24105390/how-to-login-to-facebook-in-xamarin-forms
			if(	! IsShown ) {

				IsShown = true;

				var auth = new MixRadioAuth (ApiKeys.ClientId,
					ApiKeys.ClientSecret,
					ApiKeys.OAuthScope.ToString(), // this is ignored and replaced in the MixRadioAuth class
					new Uri (ApiKeys.OAuthAuthorizeUrl),
					new Uri (ApiKeys.OAuthRedirectUrl),
					new Uri (ApiKeys.OAuthTokenUrl));

				auth.Title = "Sign in to MixRadio";

				auth.Completed += async (sender, eventArgs) => {
					// We presented the UI, so it's up to us to dimiss it on iOS.
					if (eventArgs.IsAuthenticated) {
						await (App.Current as App).ActivityViewModel.LoadXamarinAuthTokenDetails(eventArgs.Account.Properties);

					} else {
						// The user cancelled
					}

					await (App.Current as App).MainPage.Navigation.PopModalAsync();
				};

				PresentViewController (auth.GetUI (), true, null);

			}

		}
	}
}

