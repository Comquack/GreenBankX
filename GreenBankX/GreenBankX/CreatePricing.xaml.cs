using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePricing : ContentPage
	{
        int selector=-1;
        int select2 = -1;
        public CreatePricing ()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent ();
            selector = -10;
            Change.IsVisible = false;
 
            if (((List<PriceRange>)Application.Current.Properties["Prices"]).Count() == 0)
            {
                Name.IsVisible = true;
                Len.IsVisible = true;
                AddName.IsVisible = true;
                AddNew.IsVisible = false;
                maxDiam.IsVisible = false;
                minDiam.IsVisible = false;
                price.IsVisible = false;
                AddPrice.IsVisible = false;
            }
            else {
                Name.IsVisible = false;
                Len.IsVisible = false;
                AddName.IsVisible = false;
                AddNew.IsVisible = true;
                maxDiam.IsVisible = true;
                minDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
            }
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
            }
            
        }

        public void DunLLoadin() {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutprice"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Pricing", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePrice0"), "Continue", "Skip");
                    if (res)
                    {
                        await DisplayAlert("Prices", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePrice1"), "Next");
                        await DisplayAlert("Prices", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePrice2"), "Next");
                        await DisplayAlert("Prices", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePrice3"), "Next");
                        Application.Current.Properties["Tutprice"] = false;
                    }
                    else
                    {
                        Application.Current.Properties["Tutprice"] = false;
                    }
                });
            }
        }
        public void AddPriceName() {
            if (Name.Text != null && Len.Text!=null && double.Parse(Len.Text) > 0)
            {
                for (int i = 0; i < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count; i++) {
                    if (((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(i).GetName() == Name.Text) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NameInUse"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NameInUse"), "OK");
                        });
                        return;
                    }
                }
                ((List<PriceRange>)Application.Current.Properties["Prices"]).Add(new PriceRange(Name.Text, "Tree", new SortedList<double, double>(), double.Parse(Len.Text)));
                NameOfPrices.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth")+": " + Name.Text + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogLength")+": " + Len.Text+"m";
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
                minDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
                SaveAll.GetInstance().SavePricing();

            }
            else if (Name.Text == null || Name.Text == "")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("IsInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name")+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("IsInvalid"), "OK");
                });
            }
            else {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Length"+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("IsInvalid"), "Length" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("IsInvalid"), "OK");
                });
            }
        }



        public void SelectPrice()
        {
            if (pickPrice.SelectedIndex > -1)
            {
                selector = pickPrice.SelectedIndex;
                PopList(selector);
                AddPrice.IsVisible = true;
                AddName.IsVisible = false;
                Name.IsVisible = false;
                Len.IsVisible = false;
                minDiam.IsVisible = true;
                maxDiam.IsVisible = true;

            }
        }

        private void PopList(int select) {
            //Fills list of prices
            if (select > -1)
            {
                List<string> ArrayList = new List<string>();
                PriceRange ThisPrice = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(select);
                SortedList<double, double> brack = ThisPrice.GetBrack();
                for (int x = 0; x < brack.Count(); x++)
                {
                    if (x == brack.Count() - 1)
                    {
                        ArrayList.Add(brack.ElementAt(x).Key + "(cm) and larger" + "\t\t\t\t" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Price") + ": " + brack.ElementAt(x).Value);
                    }
                    else
                    {
                        ArrayList.Add(brack.ElementAt(x).Key + "(cm) to" +  brack.ElementAt(x+1).Key+"(cm)" + "\t\t\t\t" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Price") + ": " + brack.ElementAt(x).Value);
                    }
                    }
                NameOfPrices.Text =AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPrice.GetName() + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogLength") + ": " + ThisPrice.GetLength().ToString() + "m";
                PriceArray = ArrayList.ToArray();
                PriceList.ItemsSource = PriceArray;
                PriceList.HeightRequest = (40 * PriceArray.Length) + (10 * PriceArray.Length);
            }
        }
        private void AddPrice_Clicked(object sender, EventArgs e)
        {
            if (selector>-1 && minDiam.Text != null && price.Text != null && double.Parse(minDiam.Text) > 0 && double.Parse(price.Text) > 0) {
                if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(double.Parse(minDiam.Text), double.Parse(price.Text)))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), "OK");
                    });
                }
                else {
                    PopList(selector);
                    minDiam.Text = maxDiam.Text;
                    maxDiam.Text = null;
                    price.Text = null;
                    SaveAll.GetInstance().SavePricing();
                };

            } else if (minDiam.Text == null || double.Parse(minDiam.Text)<= 0||(maxDiam.Text != null&& double.Parse(minDiam.Text)> double.Parse(maxDiam.Text)))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), "OK");
                });
            }
            else if (price.Text == null || double.Parse(price.Text) <= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), "OK");
                });
            }
        }
        public void NewPrice() {
            Name.IsVisible = true;
            Len.IsVisible = true;
            AddName.IsVisible = true;
            AddNew.IsVisible = false;
            maxDiam.IsVisible = false;
            minDiam.IsVisible = false;
            price.IsVisible = false;
        }
        public async Task DelPrice()
        {
            MessagingCenter.Unsubscribe<DeleteConfirm>(this,"Delete");
            MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {

                for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                {
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetRange() == ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector))
                    {
                        ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).SetRange(null);
                    }
                }
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
                maxDiam.IsVisible = false;
                minDiam.IsVisible = false;
                price.IsVisible = true;
                AddPrice.IsVisible = false;
                SaveAll.GetInstance().SavePricing();

            });
            await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());
            
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DunLLoadin();
        }
        void SaveTest() {
            
        }

        private void PriceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            Change.IsVisible = true;
            string name = PriceList.SelectedItem.ToString();
            for (int x = 0; x < PriceArray.Length; x++) {
                if (PriceArray.Cast<string>().ToList().ElementAt(x)== name) {
                    select2 = x;
                    minDiam.Text =((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex).GetBrack().ElementAt(x).Key.ToString();
                    price.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex).GetBrack().ElementAt(x).Value.ToString();
                }
            }
        }

        private void Change_Clicked(object sender, EventArgs e)
        {
            if (select2 > -1)
            {
                if (selector > -1 && minDiam.Text != null && price.Text != null && double.Parse(minDiam.Text) > 0 && double.Parse(price.Text) > 0)
                {
                   double key = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().ElementAt(select2).Key;
                    double value = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().ElementAt(select2).Value;
                    ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().RemoveAt(select2);
                    if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(double.Parse(minDiam.Text), double.Parse(price.Text)))
                    {
                        try {
                            ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(key, value);
                            SaveAll.GetInstance().SavePricing();

                        } catch { }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), "OK");
                        });
                    }
                    else
                    {
                        PopList(selector);
                        maxDiam.Text = null;
                        price.Text = null;
                    };

                }
                else if (minDiam.Text == null || double.Parse(minDiam.Text) <= 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DiaInvalid"), "OK");
                    });
                }
                else if (price.Text == null || double.Parse(price.Text) <= 0)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PriceInvalid"), "OK");
                    });
                }
            }
        }

        private void Len_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 100 || double.Parse(e.NewTextValue) < 0))
            {
                Len.Text = e.OldTextValue;
            }

        }

        private void maxDiam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                maxDiam.Text = e.OldTextValue;
            }
        }
        private void minDiam_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                minDiam.Text = e.OldTextValue;
            }
        }
    }
}