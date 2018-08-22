using Rg.Plugins.Popup.Services;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePricing : ContentPage
	{
        int selector=-1;
		public CreatePricing ()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent ();
            selector = -10;
            if (((List<PriceRange>)Application.Current.Properties["Prices"]).Count() == 0)
            {
                Name.IsVisible = true;
                Len.IsVisible = true;
                AddName.IsVisible = true;
                AddNew.IsVisible = false;
                maxDiam.IsVisible = false;
                price.IsVisible = false;
                AddPrice.IsVisible = false;
            }
            else {
                Name.IsVisible = false;
                Len.IsVisible = false;
                AddName.IsVisible = false;
                AddNew.IsVisible = true;
                maxDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
            }
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
            }
        }
        public void AddPriceName() {
            if (Name.Text != null && Len.Text!=null && double.Parse(Len.Text) > 0)
            {
                for (int i = 0; i < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count; i++) {
                    if (((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(i).GetName() == Name.Text) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Name is in use", "Name is in use", "OK");
                        });
                        return;
                    }
                }
                ((List<PriceRange>)Application.Current.Properties["Prices"]).Add(new PriceRange(Name.Text, "Tree", new SortedList<double, double>(), double.Parse(Len.Text)));
                NameOfPrices.Text = "Name: " + Name.Text + "Log Length: " + Len.Text+"m";
                ListOfPrices.Text = "";           
                pickPrice.Items.Clear();
                for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
                {
                    pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                }
                selector = ((List<PriceRange>)Application.Current.Properties["Prices"]).Count()-1;
                pickPrice.SelectedIndex = selector;
                Name.IsVisible = false;
                Len.IsVisible = false;
                AddName.IsVisible = false;
                AddNew.IsVisible = true;
                maxDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
                //SavePrice();

            }
            else if (Name.Text == null || Name.Text == "")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Name is invalid", "Name is invalid", "OK");
                });
            }
            else {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Length is invalid", "Length is invalid", "OK");
                });
            }
        }



        public void SelectPrice()
        {
            if (pickPrice.SelectedIndex > -1)
            {
                selector = pickPrice.SelectedIndex;
                popList(selector);
                AddPrice.IsVisible = true;
            }
        }

        private void popList(int select) {
            string prices = "";
            if (select > -1)
            {
                PriceRange ThisPrice = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(select);
                double[,] PriceList = ThisPrice.GetBracket();
                SortedList<double, double> brack = ThisPrice.GetBrack();
                for (int x = 0; x < brack.Count(); x++)
                {
                    prices = prices + "Max Diameter: " + brack.ElementAt(x).Key + "Price/m3" + brack.ElementAt(x).Value + "\n";
                }
                NameOfPrices.Text = "Name: " + ThisPrice.GetName() + "Log Length: " + ThisPrice.GetLength().ToString() + "m";
                ListOfPrices.Text = prices;
            }
        }
        private void AddPrice_Clicked(object sender, EventArgs e)
        {
            if (selector>-1 && maxDiam.Text != null && price.Text != null && double.Parse(maxDiam.Text) > 0 && double.Parse(price.Text) > 0) {
                if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(double.Parse(maxDiam.Text), double.Parse(price.Text)))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Size already exists", "Size already exists", "OK");
                    });
                }
                else {
                    popList(selector);
                    maxDiam.Text = null;
                    price.Text = null;
                   // SavePrice();
                };

            } else if (maxDiam.Text == null || double.Parse(maxDiam.Text)<= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Diameter is invalid", "Diameter is invalid", "OK");
                });
            }
            else if (price.Text == null || double.Parse(price.Text) <= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("price is invalid", "price is invalid", "OK");
                });
            }
        }
        public void NewPrice() {
            Name.IsVisible = true;
            Len.IsVisible = true;
            AddName.IsVisible = true;
            AddNew.IsVisible = false;
            maxDiam.IsVisible = false;
            price.IsVisible = false;
        }
        public async Task DelPrice()
        {
            MessagingCenter.Unsubscribe<DeleteConfirm>(this,"Delete");
            MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                ((List<PriceRange>)Application.Current.Properties["Prices"]).RemoveAt(selector);
                pickPrice.Items.Clear();
                for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
                {
                    pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                }
                Name.IsVisible = false;
                Len.IsVisible = false;
                AddName.IsVisible = false;
                AddNew.IsVisible = true;
                maxDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
                //SavePrice();

            });
            await PopupNavigation.PushAsync(DeleteConfirm.GetInstance());
            
        }
        void SaveTest() {
            SaveAll.GetInstance().SavePricing();
        }
        
    }
}