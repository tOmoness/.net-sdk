using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MixRadio.Types;
using MixRadio;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace MixRadioActivity
{
	public class ActivityViewModel : INotifyPropertyChanged
	{
		private MusicClient _mixRadioClient;
		private AuthHelper _authHelper;
		private bool _isBusy;
		private bool _keepPolling = true;
		private bool _checkingRefresh = false;

		public ObservableCollection<Artist> Artists { get; private set; }
		public ObservableCollection<UserEvent> Tracks { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ActivityViewModel (MusicClient client, AuthHelper authHelper)
		{
			this._mixRadioClient = client;
			this._authHelper = authHelper;
			this.Artists = new ObservableCollection<Artist> ();
			this.Tracks = new ObservableCollection<UserEvent> ();
		}

		public bool IsBusy
		{
			get 
			{
				return this._isBusy;
			}
			private set 
			{
				this._isBusy = value;
				this.FirePropertyChanged ("IsBusy");
			}
		}

		public bool IsUserAuthenticated
		{ 
			get
			{ 
				return this._mixRadioClient.IsUserAuthenticated;
			}
		}

		public bool IsUserTokenActive
		{ 
			get
			{ 
				return this._mixRadioClient.IsUserTokenActive;
			}
		}

        public Uri GetAuthUri()
        {
            return this._mixRadioClient.GetAuthenticationUri(ApiKeys.OAuthScope);
        }

		public async Task CheckUserAuthState()
		{
			this.IsBusy = true;
			await this._authHelper.CheckUserAuthState ();
			this.FirePropertyChanged ("IsUserAuthenticated");
			this.FirePropertyChanged ("IsUserTokenActive");
			this.IsBusy = false;
		}

        public async Task ObtainAuthTokenAsync(string code)
        {
            this.IsBusy = true;
            await this._authHelper.ObtainAuthTokenAsync(code);
            this.FirePropertyChanged("IsUserAuthenticated");
            this.FirePropertyChanged("IsUserTokenActive");
            this.IsBusy = false;
        }

		public async Task LoadXamarinAuthTokenDetails(Dictionary<string, string> authProperties)
		{
			this.IsBusy = true;
			await this._authHelper.LoadXamarinAuthTokenDetails (authProperties);
			this.FirePropertyChanged ("IsUserAuthenticated");
			this.FirePropertyChanged ("IsUserTokenActive");
			this.IsBusy = false;
		}

		public async Task StartPolling()
		{
			this._keepPolling = true;

			if (!this._checkingRefresh) {
				await this.TriggerRefresh ();
			}
		}

		public void StopPolling()
		{
			this._keepPolling = false;
		}

		private async Task TriggerRefresh()
		{
			Debug.WriteLine ("TriggerRefresh starting");
			this._checkingRefresh = true;

			while (this._keepPolling && this.IsUserTokenActive) {
				await this.Refresh ();
				if (this._keepPolling && this.IsUserTokenActive && this.Tracks.Count > 0) {
					Debug.WriteLine ("TriggerRefresh waiting a minute");
					await Task.Delay (TimeSpan.FromMinutes (1));
				}
			}

			Debug.WriteLine ("TriggerRefresh aborting");
			this._checkingRefresh = false;
		}

		private async Task Refresh()
		{
			if (this._mixRadioClient.IsUserAuthenticated && this._mixRadioClient.IsUserTokenActive) {
				Debug.WriteLine ("Refresh");
				this.IsBusy = (this.Tracks.Count == 0);

                // Only get artists once as it's a daily refresh...
                if (this.Artists.Count == 0)
                {
                    var artists = await this._mixRadioClient.GetUserTopArtistsAsync(itemsPerPage: 10);
                    foreach (var a in artists)
                    {
                        if (!this.Artists.Contains(a))
                        {
                            this.Artists.Add(a);
                        }
                    }
                    Debug.WriteLine(" - Artists = " + this.Artists.Count.ToString());
                }

				var tracks = await this._mixRadioClient.GetUserPlayHistoryAsync (UserEventAction.Complete | UserEventAction.SkipNext, 0, 50);
				if (tracks != null && tracks.Count > 0) {
					if (this.Tracks.Count == 0) {
                        var trackList = (from e in tracks
							orderby e.DateTime descending
							select e).ToList();

                        foreach (var e in trackList)
                        {
							this.Tracks.Add (e);
						}
					}
					else {
                        var trackList = (from e in tracks
							orderby e.DateTime ascending
							select e).ToList();

                        foreach (var e in trackList)
                        {
							var p = (from t in this.Tracks
								where t.Action == e.Action
								&& t.DateTime == e.DateTime
								&& t.Product.Id == e.Product.Id
								select t);
							if (p.Count() == 0) {
								this.Tracks.Insert (0, e);
							}
						}
					}

					Debug.WriteLine (" - Tracks = " + this.Tracks.Count.ToString ());
				}

				this.FirePropertyChanged ("Artists");
				this.FirePropertyChanged ("Tracks");
				this.IsBusy = false;
			} else {
				Debug.WriteLine ("Refresh - no user token");
			}
		}

		private void FirePropertyChanged(string name)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged (this, new PropertyChangedEventArgs (name));
			}
		}
    }
}

