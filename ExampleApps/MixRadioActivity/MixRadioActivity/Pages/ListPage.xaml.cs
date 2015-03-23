using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MixRadioActivity.Converters;
using Xamarin.Forms;
using MixRadio.Types;

namespace MixRadioActivity
{
    public partial class ListPage : ContentPage
    {
        public ListPage()
        {
            this.InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("ListPage OnAppearing");
            var app = App.Current as App;
            Device.OnPlatform(
                () => {
                    this.NoContentPanel.BackgroundColor =
                    this.NotLoggedInPanel.BackgroundColor =
                    this.LoadingPanel.BackgroundColor = Color.White;
                },
                () =>
                {
                    this.NoContentPanel.BackgroundColor =
                    this.NotLoggedInPanel.BackgroundColor =
                    this.LoadingPanel.BackgroundColor = Color.Black;
                },
                () =>
                {
                    this.NoContentPanel.BackgroundColor =
                    this.NotLoggedInPanel.BackgroundColor =
                    this.LoadingPanel.BackgroundColor = app.WindowsDarkTheme ? Color.Black : Color.White;
                });
        }

        public async void LoginClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("CheckForCachedToken: starting login");
            await this.Navigation.PushModalAsync(new LoginPage());
        }

        public void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var artist = e.Item as Artist;
            if (artist != null)
            {
                Uri uri = null;
                Device.OnPlatform(
                     () => { uri = new Uri("mixradio://play/artist/" + artist.Id); },
                     () => { uri = new Uri("mixradio://play/artist/" + artist.Id); },
                     () => { uri = new Uri("mixradio://play/artist/name/" + artist.Name); });
                ;
                (App.Current as App).UriLauncher.LaunchUri(uri);
            }
        }

        protected void SetList(string name)
        {
            this.BindingContext = (App.Current as App).ActivityViewModel;
            string propertyName = name + "s";
            string template = name + "Template";
            int rowHeight = (int)Resources[template + "Height"];
            if (Device.OS == TargetPlatform.WinPhone)
            {
                rowHeight += 20;
            }

            this.List.RowHeight = rowHeight;
            this.List.ItemTemplate = Resources[template] as DataTemplate;
            this.List.SetBinding(ListView.ItemsSourceProperty, propertyName);
            var showOnEmpty = new Binding(propertyName, BindingMode.Default, new EmptyListVisibilityConverter());
            var hideOnEmpty = new Binding(propertyName, BindingMode.Default, new EmptyListVisibilityConverter(), "invert");
            this.List.SetBinding(VisualElement.IsVisibleProperty, hideOnEmpty);
            this.NoContentPanel.SetBinding(VisualElement.IsVisibleProperty, showOnEmpty);
        }
    }
}

