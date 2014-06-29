// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="NOKIA">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SpeakAndMix
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string[] _examples = new string[]
        {
            "mix me some rock",
            "mix me some jazz",
            "mix play me",
            "mix some grunge",
            "mix some pop",
            "mix me Lady Gaga",
            "mix me Green Day",
            "mix me some Nirvana",
        };

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.Cortana.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.StatusText.Text = string.Empty;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.SetReadyToGoAndExample();

            // We're only adding special behavior for "new" navigations -- Voice Commands will always trigger "new" navigations
            if (e.NavigationMode == NavigationMode.New)
            {
                var voiceResult = e.Parameter as SpeechRecognitionResult;
                if (voiceResult != null)
                {
                    await this.HandleVoiceCommand(voiceResult);
                }
                else
                {
                    // If we just freshly launched this app without a Voice Command, asynchronously try to install the Voice Commands.
                    // If the commands are already installed, no action will be taken--there's no need to check.
                    await Task.Run(() => this.InstallVoiceCommands());
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Installs the Voice Command Definition (VCD) file associated with the application.
        /// Based on OS version, installs a separate document based on version 1.0 of the schema or version 1.1.
        /// </summary>
        private async void InstallVoiceCommands()
        {
            const string VcdPath = "ms-appx:///VoiceCommandDefinitions.xml";

            try
            {
                Uri vcdUri = new Uri(VcdPath);
                var file = await StorageFile.GetFileFromApplicationUriAsync(vcdUri);
                await VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(file);

                App.LogAnalyticsEvent("InstallVoiceCommands", "OK", null, 0);
            }
            catch (Exception vcdEx)
            {
                Debug.WriteLine(vcdEx.ToString());
                this.StatusText.Text = "sorry, could not register voice commands - " + vcdEx.Message;

                GoogleAnalytics.EasyTracker.GetTracker().SendException("InstallVoiceCommands failed: " + vcdEx.Message, false);
            }
        }

        private async Task HandleVoiceCommand(SpeechRecognitionResult voiceResult)
        {
            if (voiceResult.Status == SpeechRecognitionResultStatus.Success)
            {
                this.SetBusyAnimation(true);
                this.StatusText.Text = "absorbing MixRadio's music knowledge...";

                var text = voiceResult.Text.ToLowerInvariant();

                App.LogAnalyticsEvent("Country", App.Country, null, 0);

                if (text.CompareTo("play me") == 0 || text.CompareTo("my favourites") == 0 || text.CompareTo("my favorites") == 0)
                {
                    this.SetBusyAnimation(false);
                    this.SetReadyToGoAndExample();

                    App.LogAnalyticsEvent("HandleVoiceCommand", voiceResult.Text + " => PlayMe", null, 0);

                    // Start Play Me...
                    await new Nokia.Music.Tasks.PlayMeTask().Show();
                }
                else
                {
                    // strip "some"...
                    text = text.Replace("some ", string.Empty);

                    // strip "me"...
                    text = text.Replace("me ", string.Empty);

                    // look for a mix...
                    await App.LoadMixes();

                    var candidates = (from m in App.Mixes
                                      where m.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1
                                      select m).ToArray();

                    if (candidates.Any())
                    {
                        // got one, choose one at random in case there are many (e.g. "rock" has loads)...
                        Random rnd = new Random();
                        int index = rnd.Next(candidates.Count() - 1);
                        var mix = candidates[index];

                        App.LogAnalyticsEvent("HandleVoiceCommand", voiceResult.Text + " => Curated " + mix.Id, null, 0);

                        this.SetBusyAnimation(false);
                        this.SetReadyToGoAndExample();
                        await mix.Play();
                    }
                    else
                    {
                        // try to find an artist...
                        var artist = await App.FindArtist(text);
                        this.SetBusyAnimation(false);
                        this.SetReadyToGoAndExample();

                        if (artist != null)
                        {
                            App.LogAnalyticsEvent("HandleVoiceCommand", voiceResult.Text + " => Artist " + artist.Id, null, 0);

                            // start the artist mix...
                            await artist.PlayMix();
                        }
                        else
                        {
                            // nothing found
                            this.StatusText.Text = "sorry, could not find any '" + text + "' to play.\r\ntry '" + this.GetExample() + "'";
                            App.LogAnalyticsEvent("HandleVoiceCommand", voiceResult.Text + " => No matches", null, 0);
                        }
                    }
                }
            }
            else
            {
                App.LogAnalyticsEvent("HandleVoiceCommand", "SpeechRecognitionResultStatus==" + voiceResult.Status.ToString(), null, 0);
            }
        }

        private void SetReadyToGoAndExample()
        {
            this.StatusText.Text = "ready to go! open Cortana and try '" + this.GetExample() + "'";
        }

        private string GetExample()
        {
            Random rnd = new Random();
            int index = rnd.Next(this._examples.Count() - 1);
            return this._examples[index];
        }

        private void SetBusyAnimation(bool show)
        {
            if (show)
            {
                this.Anim.Begin();
                this.Cortana.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.Anim.Stop();
                this.Cortana.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void ShowAbout(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(About));
        }
    }
}
