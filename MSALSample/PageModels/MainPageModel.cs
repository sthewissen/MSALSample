using System;
using System.Threading.Tasks;
using MSALSample.Services;
using MvvmHelpers.Commands;
using PropertyChanged;

namespace MSALSample.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageModel : FreshMvvm.FreshBasePageModel
    {
        private readonly AuthService _authService;
        private readonly SimpleGraphService _simpleGraphService;

        public bool IsSignedIn { get; set; }
        public bool IsSigningIn { get; set; }
        public string Name { get; set; }

        public AsyncCommand SignInCommand { get; set; }
        public AsyncCommand SignOutCommand { get; set; }

        public MainPageModel()
        {
            _authService = new AuthService();
            _simpleGraphService = new SimpleGraphService();

            SignInCommand = new AsyncCommand(SignInAsync);
            SignOutCommand = new AsyncCommand(SignOutAsync);
        }



        async Task SignInAsync()
        {
            IsSigningIn = true;

            if (await _authService.SignInAsync())
            {
                Name = await _simpleGraphService.GetNameAsync();
                IsSignedIn = true;
            }

            IsSigningIn = false;
        }

        async Task SignOutAsync()
        {
            if (await _authService.SignOutAsync())
            {
                IsSignedIn = false;
            }
        }
    }
}
