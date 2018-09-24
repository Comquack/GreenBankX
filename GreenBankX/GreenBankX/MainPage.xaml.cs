using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Application.Current.Properties["Load"] = false;
            Application.Current.Properties["Plots"] = new List<Plot>();
            Application.Current.Properties["Prices"] = new List<PriceRange>();
            Application.Current.Properties["First"] = null;
            Application.Current.Properties["Last"] = null;
            Application.Current.Properties["Boff"] = " ";
            Application.Current.Properties["Signed"] = false;
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

    }
}
