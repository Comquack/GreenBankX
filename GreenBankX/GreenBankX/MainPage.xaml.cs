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
    }
}
