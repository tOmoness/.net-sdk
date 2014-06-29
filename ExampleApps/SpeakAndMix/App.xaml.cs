// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="NOKIA">
// Copyright (c) 2014, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Nokia.Music;
using Nokia.Music.Types;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace SpeakAndMix
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            // Make sure you register for API credentials and put them in the APIKeys file
            App.MixRadioClient = new MusicClient(ApiKeys.ClientId);
        }

        public static string Country
        {
            get
            {
                return MixRadioClient.CountryCode;
            }
        }

        public static List<Mix> Mixes
        {
            get;
            private set;
        }

        private static MusicClient MixRadioClient
        {
            get;
            set;
        }

        public static async Task<List<Mix>> LoadMixes()
        {
            const int PageSize = 200;

            if (App.Mixes == null)
            {
                App.Mixes = new List<Mix>();
                ListResponse<Mix> result = await App.MixRadioClient.GetAllMixesAsync(itemsPerPage: PageSize);
                if (result.Succeeded)
                {
                    int totalResults = result.TotalResults.Value;
                    App.Mixes.AddRange(result);
                    while (App.Mixes.Count < totalResults)
                    {
                        result = await App.MixRadioClient.GetAllMixesAsync(startIndex: App.Mixes.Count, itemsPerPage: PageSize);
                        if (result.Succeeded)
                        {
                            App.Mixes.AddRange(result);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return App.Mixes;
        }

        public static async Task<Artist> FindArtist(string text)
        {
            var result = await App.MixRadioClient.SearchArtistsAsync(text, itemsPerPage: 1);
            if (result.Succeeded && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Logs an analytics event.
        /// </summary>
        /// <remarks>
        /// Wrapped here so that exceptions can be swallowed if you don't add the analytics xml file
        /// </remarks>
        /// <param name="category">The category.</param>
        /// <param name="action">The action.</param>
        /// <param name="label">The label.</param>
        /// <param name="value">The value.</param>
        public static void LogAnalyticsEvent(string category, string action, string label, int value)
        {
            // Google Analytics SDK not available for WPA81 yet...
            try
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendEvent(category, action, label, value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LogAnalyticsEvent error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.CacheSize = 1;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                // Retrieves the activation Uri.
                var voiceArgs = (IVoiceCommandActivatedEventArgs)args;
                var result = voiceArgs.Result;

                var frame = Window.Current.Content as Frame;

                if (frame == null)
                {
                    frame = new Frame();
                }

                // Navigates to MainPage, passing the Uri to it.
                frame.Navigate(typeof(MainPage), result);
                Window.Current.Content = frame;

                // Ensure the current window is active
                Window.Current.Activate();
            }

            base.OnActivated(args);
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}