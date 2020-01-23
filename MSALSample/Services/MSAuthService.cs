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
        const string ClientID = "{YOUR APPLICATION CLIENT ID HERE}";
        readonly string[] Scopes = { "User.Read" };
        readonly IPublicClientApplication _pca;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public AuthService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)
                .WithIosKeychainSecurityGroup("io.thewissen.msalsample")
                .WithRedirectUri($"msal{ClientID}://auth")
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


        //await RefreshUserDataAsync(authResult?.AccessToken).ConfigureAwait(false);
        //await RefreshUserDataAsync(authResult?.AccessToken).ConfigureAwait(false);

        //readonly string[] Scopes = { "User.Read" };
        //const string GraphUrl = "https://graph.microsoft.com/v1.0/me";

        ///// <summary>
        ///// Refresh user date from the Graph api.
        ///// </summary>
        ///// <param name="token">The user access token.</param>
        ///// <returns>The current user with his associated informations.</returns>
        //private async Task RefreshUserDataAsync(string token)
        //{
        //    HttpClient client = new HttpClient();
        //    HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, GraphUrl);
        //    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        //    HttpResponseMessage response = await client.SendAsync(message);
        //    //User currentUser = null;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        string json = await response.Content.ReadAsStringAsync();
        //        //currentUser = JsonConvert.DeserializeObject<User>(json);
        //    }

        //    //return currentUser;
        //}
    }
}
