using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Nokia.Music;
using Nokia.Music.Types;
using Xamarin.Forms;

namespace MixRadioActivity
{
    public class AuthHelper
    {
        private MusicClient _mixRadioClient;
        private IAuthPlatformSpecific _platformSpecific;

        public AuthHelper(IAuthPlatformSpecific platformSpecific, MusicClient mixRadioClient)
        {
            this._platformSpecific = platformSpecific;
            this._mixRadioClient = mixRadioClient;
        }

        /// <summary>
        /// Checks the current state 
        /// </summary>
        /// <returns>The user auth state.</returns>
        public async Task CheckUserAuthState()
        {
            if (_mixRadioClient.IsUserAuthenticated)
            {
                Debug.WriteLine("CheckUserAuthState: User Authenticated already");
                return;
            }

            var token = await this._platformSpecific.LoadTokenDetails();
            Debug.WriteLine("CheckForCachedToken: have a cached token? " + (!string.IsNullOrEmpty(token)).ToString());
            this.SetAuthenticationTokenDetails(token);

            Debug.WriteLine("CheckForCachedToken: IsUserTokenActive? " + _mixRadioClient.IsUserTokenActive.ToString());

            if (!_mixRadioClient.IsUserAuthenticated)
            {
                Debug.WriteLine("CheckForCachedToken: login required");
                return;
            }
            else if (!_mixRadioClient.IsUserTokenActive)
            {
                // refresh needed...
                Debug.WriteLine("CheckForCachedToken: starting refresh token");
                await this.RefreshToken();
            }
        }

        public async Task LoadXamarinAuthTokenDetails(Dictionary<string, string> authProperties)
        {
            var serialised = AuthToken.FromXamarinDictionary(authProperties).ToString();

            await this._platformSpecific.StoreTokenDetails(serialised);
            this.SetAuthenticationTokenDetails(serialised);
        }

        public async Task<bool> ObtainAuthTokenAsync(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var token = await this._mixRadioClient.GetAuthenticationTokenAsync(ApiKeys.ClientSecret, code);
                if (token != null)
                {
                    await this._platformSpecific.StoreTokenDetails(token.ToString());
                    Debug.WriteLine("ObtainAuthTokenAsync OK");
                    return true;
                }
            }

            return false;
        }

        private bool SetAuthenticationTokenDetails(string serialised)
        {
            try
            {
                if (!string.IsNullOrEmpty(serialised))
                {
                    Debug.WriteLine("SetAuthenticationTokenDetails: setting token details");
                    this._mixRadioClient.SetAuthenticationToken(AuthToken.FromJson(serialised));
                    return true;
                }
            }
            catch
            {
            }

            Debug.WriteLine("SetAuthenticationTokenDetails: token details invalid");
            return false;
        }

        private async Task<bool> RefreshToken()
        {
            try
            {
                AuthToken token = await this._mixRadioClient.RefreshAuthenticationTokenAsync(ApiKeys.ClientSecret);
                if (token != null)
                {
                    await this._platformSpecific.StoreTokenDetails(token.ToString());
                    Debug.WriteLine("RefreshToken OK");
                    return true;
                }
            }
            catch
            {
            }

            Debug.WriteLine("RefreshToken Failed");
            return false;
        }

    }
}

