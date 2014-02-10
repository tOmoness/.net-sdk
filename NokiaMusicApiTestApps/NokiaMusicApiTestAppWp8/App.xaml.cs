// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Nokia">
// Copyright © 2012-2013 Nokia Corporation. All rights reserved.
// Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
// Other product and company names mentioned herein may be trademarks
// or trade names of their respective owners. 
// See LICENSE.TXT for license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Nokia.Music;
using Nokia.Music.Tasks;
using Nokia.Music.Types;

namespace Nokia.Music.TestApp
{
    /// <summary>
    /// The Application class
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Constant for id parameter.
        /// </summary>
        public const string IdParam = "id";

        /// <summary>
        /// Constant for name parameter.
        /// </summary>
        public const string NameParam = "name";

        /// <summary>
        /// Constant for thumb parameter.
        /// </summary>
        public const string ThumbParam = "thumb";

        private const string SettingCountryCode = "countrycode";

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            this.UnhandledException += this.Application_UnhandledException;

            // Standard Silverlight initialization
            this.InitializeComponent();

            // Phone-specific initialization
            this.InitializePhoneApplication();

            if (string.IsNullOrEmpty(ApiKeys.ClientId))
            {
                throw new ApiCredentialsRequiredException();
            }
            else
            {
                CreateApiClient(GetSettingsCountryCode());
            }
        }

        /// <summary>
        /// Gets the Music Client.
        /// </summary>
        /// <returns>The Music Client.</returns>
        public static MusicClient ApiClient { get; private set; }

        /// <summary>
        /// Gets the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Gets the country code from application settings.
        /// </summary>
        /// <returns>The country code from application settings</returns>
        public static string GetSettingsCountryCode()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(SettingCountryCode))
            {
                return IsolatedStorageSettings.ApplicationSettings[SettingCountryCode] as string;
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
            IsolatedStorageSettings.ApplicationSettings[SettingCountryCode] = countryCode;
            IsolatedStorageSettings.ApplicationSettings.Save();

            // Now create the ApiClient...
            CreateApiClient(countryCode);
        }

        /// <summary>
        /// Routes clicks on an MusicItem to the right place...
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A boolean indicating if we rooted the object successfully</returns>
        public bool RouteItemClick(object item)
        {
            Artist artist = item as Artist;
            if (artist != null)
            {
                string thumb = string.Empty;
                if (artist.Thumb200Uri != null)
                {
                    thumb = HttpUtility.UrlEncode(artist.Thumb200Uri.ToString());
                }

                this.RootFrame.Navigate(new Uri("/ArtistPage.xaml?" + App.IdParam + "=" + artist.Id + "&" + App.NameParam + "=" + HttpUtility.UrlEncode(artist.Name) + "&" + App.ThumbParam + "=" + thumb, UriKind.Relative));
                return true;
            }

            Product product = item as Product;
            if (product != null)
            {
                if (product.Category == Category.Track)
                {
                    ShowProductTask task = new ShowProductTask() { ClientId = ApiKeys.ClientId, ProductId = product.Id };
                    task.Show();
                }
                else
                {
                    string thumb = string.Empty;
                    if (product.Thumb200Uri != null)
                    {
                        thumb = HttpUtility.UrlEncode(product.Thumb200Uri.ToString());
                    }

                    this.RootFrame.Navigate(new Uri("/AlbumPage.xaml?" + App.IdParam + "=" + product.Id + "&" + App.NameParam + "=" + HttpUtility.UrlEncode(product.Name) + "&" + App.ThumbParam + "=" + thumb, UriKind.Relative));
                }

                return true;
            }

            Genre genre = item as Genre;
            if (genre != null)
            {
                this.RootFrame.Navigate(new Uri("/GenrePage.xaml?" + IdParam + "=" + genre.Id + "&" + App.NameParam + "=" + HttpUtility.UrlEncode(genre.Name), UriKind.Relative));
                return true;
            }

            MixGroup group = item as MixGroup;
            if (group != null)
            {
                this.RootFrame.Navigate(new Uri("/ShowListPage.xaml?" + ShowListPage.MethodParam + "=" + MethodCall.GetMixes + "&" + IdParam + "=" + group.Id + "&" + NameParam + "=" + HttpUtility.UrlEncode(group.Name), UriKind.Relative));
                return true;
            }

            Mix mix = item as Mix;
            if (mix != null)
            {
                PlayMixTask mixPlayer = new PlayMixTask() { MixId = mix.Id };
                mixPlayer.Show();
                return true;
            }

            return false;
        }

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
        /// Handles when the album art is clicked and plays a clip if it's a track.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void AlbumArtClicked(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            if (image != null)
            {
                Product product = image.Tag as Product;

                if (product != null)
                {
                    if (product.Category == Category.Track)
                    {
                        ShowListPage listPage = this.RootFrame.Content as ShowListPage;
                        if (listPage != null)
                        {
                            listPage.PlayClip(product.Id);
                            e.Handled = true;
                        }

                        ArtistPage artistPage = this.RootFrame.Content as ArtistPage;
                        if (artistPage != null)
                        {
                            artistPage.PlayClip(product.Id);
                            e.Handled = true;
                        }

                        AlbumPage albumPage = this.RootFrame.Content as AlbumPage;
                        if (albumPage != null)
                        {
                            albumPage.PlayClip(product.Id);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // NokiaMusicException's give an error message...
            if (e.ExceptionObject as NokiaMusicException != null)
            {
                MessageBox.Show(e.ExceptionObject.Message);
                e.Handled = true;
                return;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            MessageBox.Show("oops, something went wrong...\r\n" + e.ExceptionObject.Message);
        }

        #region Phone application initialization
        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (this.phoneApplicationInitialized)
            {
                return;
            }

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            this.RootFrame = new TransitionFrame();
            this.RootFrame.Navigated += this.CompleteInitializePhoneApplication;

            // Handle navigation failures
            this.RootFrame.NavigationFailed += this.RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            this.phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (this.RootVisual != this.RootFrame)
            {
                this.RootVisual = this.RootFrame;
            }

            // Remove this handler since it is no longer needed
            this.RootFrame.Navigated -= this.CompleteInitializePhoneApplication;
        }

        #endregion
    }
}