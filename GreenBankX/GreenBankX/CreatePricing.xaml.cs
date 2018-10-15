﻿using GreenBankX.Resources;
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
                price.IsVisible = true;
                AddPrice.IsVisible = true;

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
                    ArrayList.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("minimumdiameter") + ": " + brack.ElementAt(x).Key + "\t\t\t\t" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Price") + ": " + brack.ElementAt(x).Value);
                }
                NameOfPrices.Text =AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPrice.GetName() + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogLength") + ": " + ThisPrice.GetLength().ToString() + "m";
                PriceArray = ArrayList.ToArray();
                PriceList.ItemsSource = PriceArray;
                PriceList.HeightRequest = (40 * PriceArray.Length) + (10 * PriceArray.Length);
            }
        }
        private void AddPrice_Clicked(object sender, EventArgs e)
        {
            if (selector>-1 && maxDiam.Text != null && price.Text != null && double.Parse(maxDiam.Text) > 0 && double.Parse(price.Text) > 0) {
                if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(double.Parse(maxDiam.Text), double.Parse(price.Text)))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SizeExists"), "OK");
                    });
                }
                else {
                    PopList(selector);
                    maxDiam.Text = null;
                    price.Text = null;
                };

            } else if (maxDiam.Text == null || double.Parse(maxDiam.Text)<= 0)
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
                maxDiam.IsVisible = true;
                price.IsVisible = true;
                AddPrice.IsVisible = true;
                //SavePrice();

            });
            await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());
            
        }
        void SaveTest() {
            SaveAll.GetInstance().SavePricing();
        }

        private void PriceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            Change.IsVisible = true;
            string name = PriceList.SelectedItem.ToString();
            for (int x = 0; x < PriceArray.Length; x++) {
                if (PriceArray.Cast<string>().ToList().ElementAt(x)== name) {
                    select2 = x;
                    maxDiam.Text =((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex).GetBrack().ElementAt(x).Key.ToString();
                    price.Text = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex).GetBrack().ElementAt(x).Value.ToString();
                }
            }
        }

        private void Change_Clicked(object sender, EventArgs e)
        {
            if (select2 > -1)
            {
                if (selector > -1 && maxDiam.Text != null && price.Text != null && double.Parse(maxDiam.Text) > 0 && double.Parse(price.Text) > 0)
                {
                   double key = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().ElementAt(select2).Key;
                    double value = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().ElementAt(select2).Value;
                    ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).GetBrack().RemoveAt(select2);
                    if (!((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(double.Parse(maxDiam.Text), double.Parse(price.Text)))
                    {
                        try {
                            ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(selector).addBrack(key, value);
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
                else if (maxDiam.Text == null || double.Parse(maxDiam.Text) <= 0)
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
    }
}