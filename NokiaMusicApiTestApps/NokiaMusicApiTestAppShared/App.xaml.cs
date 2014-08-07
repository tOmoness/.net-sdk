// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Nokia">
// Copyright (c) 2013, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Nokia.Music.Tasks;
using Nokia.Music.Types;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
#if WINDOWS_APP
using Windows.ApplicationModel.Search;
#endif
using Windows.Foundation;
using Windows.Storage;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private const string SettingCountryCode = "countrycode";
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif
#if WINDOWS_APP
        private Popup _settingsPopup;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            if (string.IsNullOrEmpty(ApiKeys.ClientId))
            {
                throw new ApiCredentialsRequiredException();
            }
            else
            {
                CreateApiClient(GetSettingsCountryCode());
            }
        }

#if WINDOWS_PHONE_APP
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
#endif

        /// <summary>
        /// Gets the Music Client.
        /// </summary>
        /// <returns>The Music Client.</returns>
        public static MusicClient ApiClient { get; private set; }

        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>
        /// The root frame.
        /// </value>
        public static Frame RootFrame
        {
            get
            {
                return Window.Current.Content as Frame;
            }
        }

        /// <summary>
        /// Gets the settings container.
        /// </summary>
        /// <value>
        /// The settings container.
        /// </value>
        private static ApplicationDataContainer SettingsContainer
        {
            get
            {
                const string SettingsKey = "AppSettings";
                if (!ApplicationData.Current.LocalSettings.Containers.ContainsKey(SettingsKey))
                {
                    return ApplicationData.Current.LocalSettings.CreateContainer(SettingsKey, ApplicationDataCreateDisposition.Always);
                }
                else
                {
                    return ApplicationData.Current.LocalSettings.Containers[SettingsKey];
                }
            }
        }

        /// <summary>
        /// Gets the country code from application settings.
        /// </summary>
        /// <returns>The country code from application settings</returns>
        public static string GetSettingsCountryCode()
        {
            if (SettingsContainer.Values.ContainsKey(SettingCountryCode))
            {
                return SettingsContainer.Values[SettingCountryCode] as string;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the API region in app settings and the ApiClient
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        public static void SaveCountryCode(string countryCode)
        {
            // Save for next time...
            SettingsContainer.Values[SettingCountryCode] = countryCode;

            // Now create the ApiClient...
            CreateApiClient(countryCode);
        }

        /// <summary>
        /// Routes clicks on an MusicItem to the right place...
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="player">The player if the page has one.</param>
        /// <returns>
        /// A boolean indicating if we rooted the object successfully
        /// </returns>
        public async Task<bool> RouteItemClick(object item, MediaElement player)
        {
            Artist artist = item as Artist;
            if (artist != null)
            {
                App.RootFrame.Navigate(typeof(DetailPage), artist);
                return true;
            }

            Product product = item as Product;
            UserEvent userEvent = item as UserEvent;
            if (userEvent != null && userEvent.Product != null)
            {
                product = userEvent.Product;
            }

            if (product != null)
            {
                if (product.Category == Category.Track)
                {
                    Debug.WriteLine("Track pressed");
                    await product.Show();
                }
                else if (product.Category == Category.Album)
                {
                    App.RootFrame.Navigate(typeof(DetailPage), product);
                }

                return true;
            }

            Genre genre = item as Genre;
            if (genre != null)
            {
                App.RootFrame.Navigate(typeof(DetailPage), genre);
                return true;
            }

            MixGroup group = item as MixGroup;
            if (group != null)
            {
                App.RootFrame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.GetMixes, group.Id, group.Name));
                return true;
            }

            Mix mix = item as Mix;
            if (mix != null)
            {
                await mix.Play();
                return true;
            }

            return false;
        }

#if WINDOWS_PHONE_APP
        protected async override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args != null && args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                var authArgs = args as WebAuthenticationBrokerContinuationEventArgs;
                if (authArgs != null)
                {
                    var frame = Window.Current.Content as Frame;
                    if (frame != null)
                    {
                        var mainPage = frame.Content as MainPage;
                        if (mainPage != null)
                        {
                            await mainPage.CompleteUserAuthAsync(authArgs.WebAuthenticationResult);
                        }
                    }
                }
            }
        }

#endif
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
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
#endif
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

#if WINDOWS_APP
            SearchPane.GetForCurrentView().SuggestionsRequested += new TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs>(this.OnSearchPaneSuggestionsRequested);
            SettingsPane.GetForCurrentView().CommandsRequested += this.OnSettingsCommandsRequested;
#endif
            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_APP
        /// <summary>
        /// This is the handler for Search activation.
        /// </summary>
        /// <param name="args">This is the list of arguments for search activation, including QueryText and Language</param>
        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.QueryText))
            {
                // navigate to landing page.
            }
            else
            {
                // display search results.
                RootFrame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.Search, null, args.QueryText));
            }
        }

#endif
        /// <summary>
        /// Creates the API client.
        /// </summary>
        /// <param name="countryCode">A country code.</param>
        private static void CreateApiClient(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                ApiClient = new MusicClient(ApiKeys.ClientId, countryCode);
            }
            else
            {
                ApiClient = null;
            }
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
            deferral.Complete();
        }

#if WINDOWS_APP
        /// <summary>
        /// Called when a search query is submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="SearchPaneQuerySubmittedEventArgs"/> instance containing the event data.</param>
        private void OnQuerySubmitted(object sender, SearchPaneQuerySubmittedEventArgs args)
        {
            RootFrame.Navigate(typeof(ShowListPage), new ShowListParams(MethodCall.Search, null, args.QueryText));
        }

        /// <summary>
        /// Called when search pane suggestions are requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SearchPaneSuggestionsRequestedEventArgs"/> instance containing the event data.</param>
        private async void OnSearchPaneSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs e)
        {
            var queryText = e.QueryText;

            if (!string.IsNullOrEmpty(queryText))
            {
                // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
                var deferral = e.Request.GetDeferral();
                try
                {
                    Debug.WriteLine("Requesting suggestions for query: " + queryText);
                    ListResponse<string> suggestions = await ApiClient.GetSearchSuggestionsAsync(queryText, 5);

                    if (suggestions.Result != null)
                    {
                        e.Request.SearchSuggestionCollection.AppendQuerySuggestions(suggestions.Result);
                        Debug.WriteLine("Suggestions provided for query: " + queryText);
                    }
                    else
                    {
                        Debug.WriteLine("No suggestions provided for query: " + queryText + " error: " + suggestions.Error.Message);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Suggestions could not be displayed");
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private void OnSettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand(
                "about",
                "About",
                (x) =>
                {
                    _settingsPopup = new Popup();
                    _settingsPopup.Closed += OnSettingsPopupClosed;
                    Window.Current.Activated += OnWindowActivated;
                    _settingsPopup.IsLightDismissEnabled = true;
                    _settingsPopup.Width = 346;
                    _settingsPopup.Height = Window.Current.Bounds.Height;

                    AboutPanel mypane = new AboutPanel();
                    mypane.Width = _settingsPopup.Width;
                    mypane.Height = Window.Current.Bounds.Height;

                    _settingsPopup.Child = mypane;
                    _settingsPopup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - _settingsPopup.Width);
                    _settingsPopup.SetValue(Canvas.TopProperty, 0);
                    _settingsPopup.IsOpen = true;
                });

            args.Request.ApplicationCommands.Add(cmd);
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                this._settingsPopup.IsOpen = false;
            }
        }

        private void OnSettingsPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= this.OnWindowActivated;
        }
#endif
    }
}
