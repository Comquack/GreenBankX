using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Auth;

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
            Application.Current.Properties["Tutorial"] = false;
            Application.Current.Properties["Tutmen"] = true;
            Application.Current.Properties["Tutprice"] = true;
            Application.Current.Properties["Tutmes"] = true;
            Application.Current.Properties["Tutplot"] = true;
            Application.Current.Properties["Tutmanage"] = true;
            Application.Current.Properties["Tutmanage2"] = false;
            Application.Current.Properties["Tutdt"] = true;
            Application.Current.Properties["Boff"] = " ";
            Application.Current.Properties["Signed"] = false;
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
        }

        async void OpenMenu()
        {
            await Navigation.PushAsync(new MenuPage());
        }
    }
}
