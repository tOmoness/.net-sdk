using System;
using MixRadioActivity;
using MixRadioActivity.WinPhone;
using Windows.Security.Authentication.Web;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer (typeof (LoginPage), typeof (LoginPageRenderer))]

namespace MixRadioActivity.WinPhone
{
	public class LoginPageRenderer : PageRenderer
	{
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            var app = (MixRadioActivity.App.Current as MixRadioActivity.App);
            WebAuthenticationBroker.AuthenticateAndContinue(app.ActivityViewModel.GetAuthUri(), new Uri(ApiKeys.OAuthRedirectUrl));
		}
	}
}

