using System;
using Android.App;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MixRadioActivity;

[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]

namespace MixRadioActivity
{
    public class LoginPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            var activity = this.Context as Activity;

            var auth = new MixRadioAuth(ApiKeys.ClientId,
                ApiKeys.ClientSecret,
                ApiKeys.OAuthScope.ToString(), // this is ignored and replaced in the MixRadioAuth class
                new Uri(ApiKeys.OAuthAuthorizeUrl),
                new Uri(ApiKeys.OAuthRedirectUrl),
                new Uri(ApiKeys.OAuthTokenUrl));

            auth.Title = "Sign in to MixRadio";

            auth.Completed += async (sender, eventArgs) =>
            {
                // We presented the UI, so it's up to us to dimiss it on iOS.
                if (eventArgs.IsAuthenticated)
                {
                    var app = (App.Current as App);
                    await app.ActivityViewModel.LoadXamarinAuthTokenDetails(eventArgs.Account.Properties);

                    // Xamarin Forms doesn't seem to fire OnAppearing consistantly
                    // on iOS and Android currently (1.3.1)
                    // - so this kicks off the initial fetch of data for us...
                    app.ActivityViewModel.StartPolling();
                }
                else
                {
                    // The user cancelled
                }

                await (App.Current as App).MainPage.Navigation.PopModalAsync();
            };

            activity.StartActivity(auth.GetUI(activity));
        }
    }
}

