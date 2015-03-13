using System;
using Xamarin.Forms;

namespace MixRadioActivity
{
    public partial class MenuPage : ContentPage
    {
        private NavigationPage _activityPage = new NavigationPage(new ActivityPage());
        private NavigationPage _aboutPage = new NavigationPage(new AboutPage());

        public MenuPage()
        {
            InitializeComponent();
            var app = App.Current as App;

            Device.OnPlatform(
                () =>
                {
                    this.BackgroundColor = Color.White;
                },
                () =>
                {
                    this.BackgroundColor = Color.Black;
                    this.Icon = null;
                },
                () =>
                {
                    this.BackgroundColor = app.WindowsDarkTheme ? Color.Black : Color.White;
                });
        }

        internal MasterDetailPage ParentPage { get; set; }

        public void ShowActivity(object sender, EventArgs e)
        {
            ParentPage.Detail = _activityPage;
            ParentPage.IsPresented = false;
        }

        public void ShowAbout(object sender, EventArgs e)
        {
            ParentPage.Detail = _aboutPage;
            ParentPage.IsPresented = false;
        }
    }
}

