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
            Application.Current.Properties["Language"] = null;
            Application.Current.Properties["Plots"] = new List<Plot>();
            Application.Current.Properties["PlotsOnMap"] = new List<Plot>();
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
            Application.Current.Properties["TLogs"] = false;
            Application.Current.Properties["Boff"] = " ";
            Application.Current.Properties["Signed"] = false;
            Application.Current.Properties["Account"] = null;
            Application.Current.Properties["User"] = null;
            Application.Current.Properties["Token"] = null;
            Application.Current.Properties["ThisPlot"] = null;
            Application.Current.Properties["ThisLocation"] = null;
            Application.Current.Properties["PriceStore"] = null;
            Application.Current.Properties["AvgGirth"] = -1;
            Application.Current.Properties["AvgH"] = -1;
            Application.Current.Properties["Currenlist"] = new List<(string, double)>();
            Application.Current.Properties["Currenselect"] = -1;
            Application.Current.Properties["Priceholder"] = null;
            Application.Current.Properties["Bounds"] = new List<TK.CustomMap.Position>();
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            OpenMenu();
        }
        async void OpenMenu()
        {
            try
            {
                if (((List<PriceRange>)Application.Current.Properties["Prices"]).Count == 0)
                {
                    SaveAll.GetInstance().LoadPriceFiles();
                }
                if (((List<Plot>)Application.Current.Properties["Plots"]).Count == 0)
                {
                    SaveAll.GetInstance().LoadPlotFiles();
                    SaveAll.GetInstance().LoadTreeFiles2();
                }
            }
            catch { }
            await Navigation.PushAsync(new MenuPage());
        }
    }
}
