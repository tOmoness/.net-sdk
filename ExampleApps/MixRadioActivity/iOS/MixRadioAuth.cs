using System;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using MixRadio.Types;
using PCLStorage;

namespace MixRadioActivity
{
    public class MixRadioAuth : OAuth2Authenticator
    {
        public MixRadioAuth(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null)
            : base(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync)
        {
        }

        public override Task<Uri> GetInitialUrlAsync()
        {
            return Task.FromResult<Uri>((App.Current as App).ActivityViewModel.GetAuthUri());
        }
	}
}

