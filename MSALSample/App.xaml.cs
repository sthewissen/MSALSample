using System;
using MSALSample.PageModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSALSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = FreshMvvm.FreshPageModelResolver.ResolvePageModel<MainPageModel>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
