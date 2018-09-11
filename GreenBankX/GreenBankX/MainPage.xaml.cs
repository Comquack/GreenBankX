using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Auth;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GreenBankX
{
	public partial class MainPage : ContentPage
    {
        Account account;
        AccountStore store;

        public MainPage()
		{
			InitializeComponent();
            Application.Current.Properties["Plots"] = new List<Plot>();
            Application.Current.Properties["Prices"] = new List<PriceRange>();

            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
        }

        async void OpenMenu()
        {
            await Navigation.PushAsync(new MenuPage());
        }
        async Task OnLoginTest()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constants.AndroidClientId;
                    redirectUri = Constants.AndroidRedirectUrl;
                   bool wait = Xamarin.Forms.DependencyService.Get<ILogin>().SignIn();
                    while (wait != true) { }
                    await DisplayAlert("Hello", Xamarin.Forms.DependencyService.Get<ILogin>().AccountName(), "OK");
                    await Navigation.PushAsync(new MenuPage());
                    break;
            }
        }
            void OnLoginClicked()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constants.AndroidClientId;
                    redirectUri = Constants.AndroidRedirectUrl;
                    Xamarin.Forms.DependencyService.Get<ILogin>().SignIn();
                    break;
            }
            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Constants.scopes,
                new Uri(Constants.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(Constants.AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;
           var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
           presenter.Login(authenticator);
        }
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            User user = null;
            if (e.IsAuthenticated)
            {
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = await response.GetResponseTextAsync();
                   // Application.Current.Properties["JSON"] = userJson;
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }

                if (account != null)
                {
                    store.Delete(account, Constants.AppName);
                }
                Application.Current.Properties["User"] = user;
                
                await store.SaveAsync(account = e.Account, Constants.AppName);
                await DisplayAlert("Hello", user.Name, "OK");
                await Navigation.PushAsync(new MenuPage());
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert("Error", "Error", "OK");
            });
            Debug.WriteLine("Authentication error: " + e.Message);
        }
    }
}
