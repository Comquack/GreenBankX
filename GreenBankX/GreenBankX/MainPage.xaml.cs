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

namespace GreenBankX
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            Application.Current.Properties["Plots"] = new List<Plot>();
            Application.Current.Properties["Prices"] = new List<PriceRange>();
        }

        async void OpenMenu(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }

    }
}
