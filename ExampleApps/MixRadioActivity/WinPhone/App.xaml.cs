﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MixRadioActivity.WinPhone.Resources;
using MixRadio;
using MixRadio.Types;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace MixRadioActivity.WinPhone
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        public static string VersionNumber = null;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            this.ParseManifestForVersion();

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            PhoneApplicationService.Current.ContractActivated += App_ContractActivated;
        }

        private async void App_ContractActivated(object sender, IActivatedEventArgs args)
        {
            if (args != null && args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                var authArgs = args as WebAuthenticationBrokerContinuationEventArgs;
                if (authArgs != null)
                {
                    var app = MixRadioActivity.App.Current as MixRadioActivity.App;
                    var result = authArgs.WebAuthenticationResult;
                    if (result != null)
                    {
                        if (result.ResponseStatus == WebAuthenticationStatus.Success)
                        {
                            Debug.WriteLine("WebAuthenticationBrokerContinuation: " + result.ResponseData);
                            var authResult = AuthResultCode.Unknown;
                            string code = null;

                            OAuthResultParser.ParseQuerystringForCompletedFlags(result.ResponseData, out authResult, out code);
                            if (authResult == AuthResultCode.Success)
                            {
                                Debug.WriteLine("WebAuthenticationBrokerContinuation: got auth code " + code);
                                await app.ActivityViewModel.ObtainAuthTokenAsync(code);
                                
                                // hide the login page now
                                await app.MainPage.Navigation.PopModalAsync();
                                return;
                            }
                            else
                            {
                                Debug.WriteLine("WebAuthenticationBrokerContinuation: error " + authResult.ToString());
                            }
                        }
                    }

                    // if the user cancelled, hide the login page
                    await app.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        private void ParseManifestForVersion()
        {
            Uri uriResource = new Uri("WMAppManifest.xml", UriKind.Relative);
            StreamResourceInfo resourceStream = Application.GetResourceStream(uriResource);
            if (resourceStream != null)
            {
                string version = GetManifestAppProperty(resourceStream.Stream, "Version");
                if (!string.IsNullOrEmpty(version))
                {
                    try
                    {
                        Version ver = new Version(version);
                        version = string.Format("{0}.{1}", ver.Major, ver.Minor);
                    }
                    catch
                    {
                    }
                    VersionNumber = version;
                    return;
                }
            }
            VersionNumber = "Unknown";
        }

        /// <summary>
        /// Gets a manifest property
        /// </summary>
        /// <param name="resourceStream">The resource stream</param>
        /// <param name="name">The attribute name</param>
        /// <returns>The property value</returns>
        private string GetManifestAppProperty(Stream resourceStream, string name)
        {
            if (resourceStream != null)
            {
                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    bool deploymentFound = false;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (deploymentFound)
                        {
                            int index = line.IndexOf(name + "=\"");
                            if (index >= 0)
                            {
                                int nameLen = name.Length + 2;
                                int closeQuote = line.IndexOf("\"", (int)(index + nameLen));
                                if (closeQuote >= 0)
                                {
                                    return line.Substring(index + nameLen, (closeQuote - index) - nameLen);
                                }
                            }
                        }
                        else
                        {
                            if (line.IndexOf("AppPlatformVersion") > -1)
                            {
                                deploymentFound = true;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
