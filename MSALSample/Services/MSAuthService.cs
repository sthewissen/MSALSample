using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Essentials;

namespace MSALSample.Services
{
    public class AuthService
    {
        readonly string SignatureHash = DeviceInfo.Platform == DevicePlatform.Android ? "{YOUR_SIGNATURE_HASH}" : string.Empty;
        readonly string ClientID = "{YOUR_CLIENT_ID}";
        readonly string[] Scopes = { "User.Read" };
        readonly IPublicClientApplication _pca;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public AuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)
                .WithIosKeychainSecurityGroup("io.thewissen.msalsample")
                .WithRedirectUri($"msauth.io.thewissen.msalsample://auth{SignatureHash}")
                .WithAuthority("https://login.microsoftonline.com/common")
                .Build();
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    return true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
