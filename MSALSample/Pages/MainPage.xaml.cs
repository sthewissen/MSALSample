using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSALSample.Services;
using Xamarin.Forms;

namespace MSALSample.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void ButtonSignin_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new  AAuthService();
            await service.SignInAsync();
        }

        async void ButtonSignout_Clicked(System.Object sender, System.EventArgs e)
        {
            var service = new  AAuthService();
            await service.SignOutAsync();
        }
    }
}
