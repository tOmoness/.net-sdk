using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MixRadio.TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        /// <summary>
        /// Constant for search scope for artists.
        /// </summary>
        public const string SearchScopeArtists = "artists";

        private bool _artistSearch = false;
        private bool _searchEnabled = false;

        public SearchPage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.SearchPage_Loaded);
            this.DarkImg.Visibility = App.Current.RequestedTheme == ApplicationTheme.Dark ? Visibility.Visible : Visibility.Collapsed;
            this.LightImg.Visibility = App.Current.RequestedTheme == ApplicationTheme.Light ? Visibility.Visible : Visibility.Collapsed;

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.HardwareButtons_BackPressed;
#endif
        }
#if WINDOWS_PHONE_APP

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
#endif

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                this._artistSearch = string.Compare((string)e.Parameter, SearchScopeArtists) == 0;
            }
            else
            {
                this._artistSearch = false;
            }

            if (this._artistSearch)
            {
                this.PageTitle.Text = "artist search";
            }
            else
            {
                this.PageTitle.Text = "search";
            }

            this._searchEnabled = true;
        }

        /// <summary>
        /// Puts focus on search box after initialization.
        /// </summary>
        /// <param name="sender">SearchPage object</param>
        /// <param name="e">Event arguments</param>
        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.SearchTerm.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            this.Loaded -= this.SearchPage_Loaded;
        }

        private void SearchTermKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // Dismiss keyboard
                this.Focus(FocusState.Programmatic);

                this.PerformSearch(this, new RoutedEventArgs());
            }
        }

        private async void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<string> items = new List<string>();
                if (sender.Text.Length > 1)
                {
                    if (this._artistSearch)
                    {
                        var result = await App.ApiClient.GetArtistSearchSuggestionsAsync(sender.Text);
                        if (result.Result != null)
                        {
                            items = result.Result;
                        }
                    }
                    else
                    {
                        var result = await App.ApiClient.GetSearchSuggestionsAsync(sender.Text);
                        if (result.Result != null)
                        {
                            items = result.Result;
                        }
                    }
                }

                sender.ItemsSource = items;
            }
        }

        /// <summary>
        /// Makes a search request with current search term.
        /// </summary>
        /// <param name="sender">SearchButton object</param>
        /// <param name="e">Event arguments</param>
        private async void PerformSearch(object sender, RoutedEventArgs e)
        {
            if (this.SearchTerm.Text.Trim().Length > 0 && this._searchEnabled)
            {
                // Scroll to top...
                if (this.Results.Items != null && this.Results.Items.Count > 0)
                {
                    this.Results.ScrollIntoView(this.Results.Items[0]);
                }

                this.Results.ItemsSource = null;
                this.Loading.Visibility = Visibility.Visible;

                this.SearchTerm.IsEnabled = false;
                this._searchEnabled = false;
                this.SearchButton.Opacity = 0.5;

                if (this._artistSearch)
                {
                    this.ResponseHandler(await App.ApiClient.SearchArtistsAsync(this.SearchTerm.Text, itemsPerPage: 20));
                }
                else
                {
                    this.ResponseHandler(await App.ApiClient.SearchAsync(this.SearchTerm.Text, itemsPerPage: 20));
                }
            }
        }

        /// <summary>
        /// Populates list box with search results.
        /// </summary>
        /// <param name="response">Search results from MixRadio API</param>
        /// <typeparam name="T">Any MusicItem from MixRadio API</typeparam>
        private void ResponseHandler<T>(ListResponse<T> response)
        {
            this.Loading.Visibility = Visibility.Collapsed;
            this.Results.ItemsSource = response.Result;

            if (response.Result != null && response.Result.Count == 0)
            {
                MessageBox.Show(@"No results found");
            }
            else if (response.Error != null)
            {
                MessageBox.Show(response.Error.Message);
            }

            this.SearchTerm.IsEnabled = true;
            this._searchEnabled = true;
            this.SearchButton.Opacity = 1.0;
        }

        /// <summary>
        /// Shows details of a product or artist.
        /// Tracks will be shown in MixRadio app.
        /// </summary>
        /// <param name="sender">Results listbox</param>
        /// <param name="e">Event arguments</param>
        private async void ShowItem(object sender, SelectionChangedEventArgs e)
        {
            await (App.Current as App).RouteItemClick(this.Results.SelectedItem, null);
            this.Results.SelectedIndex = -1;
        }
    }
}
