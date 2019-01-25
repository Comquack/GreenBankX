using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBankX.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Currency : ContentPage
	{
        Currencys doubletap = null;
           List<Currencys> nel = new List<Currencys>();
        public Currency ()
		{
			InitializeComponent ();
            nel.Clear();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            nel.Add(new Currencys("USD", 1));
            foreach ((string, double) dol in (List<(string, double)>)Application.Current.Properties["Currenlist"]){
                nel.Add(new Currencys(dol.Item1, dol.Item2));
            }
            Selected.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesS") + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"]+1).Name);
            Currenlist.ItemsSource = null;
            Currenlist.ItemsSource = nel;
        }

        private void Currenlist_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubletap == Currenlist.SelectedItem)
            {
                Application.Current.Properties["Currenselect"] = ((List<Currencys>)Currenlist.ItemsSource).IndexOf(doubletap)-1;
                Selected.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesS") + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"]+1).Name);

            }
            else
            {
                if (((List<Currencys>)Currenlist.ItemsSource).IndexOf((Currencys)Currenlist.SelectedItem) == 0)
                {
                    DelC.IsVisible = false;
                }
                else { DelC.IsVisible = true; }
                doubletap = (Currencys)Currenlist.SelectedItem;
                
            }
        }

        private void AddC_Clicked(object sender, EventArgs e)
        {
            if (AddC.Text == "Add Currency")
            {
                Rate.IsVisible = true;
                Name.IsVisible = true;
                AddC.IsVisible = false;
                Conf.IsVisible = true;
            }

           
        }

        private async void Conf_Clicked(object sender, EventArgs e)
        {

            if (Name.Text != null && double.TryParse(Rate.Text, out double rates))
            {
                ((List<(string, double)>)Application.Current.Properties["Currenlist"]).Add((Name.Text, rates));
                nel.Add(new Currencys(Name.Text, rates));
                Currenlist.ItemsSource = null;
                Currenlist.ItemsSource = nel;
                title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                SaveAll.GetInstance().SaveTrees2();
                title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                Name.Text = null;
                Rate.Text = null;
                bool res = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesSet"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesSet2"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("yes"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("no"));
                if (res)
                {
                    Application.Current.Properties["Currenselect"] = ((List<Currencys>)Currenlist.ItemsSource).Count-2;
                    Selected.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesS") + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"] + 1).Name);
                }
                await Task.Delay(5000);
                title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies");
            }
        }
        private void Titleset() {

        }

        private async void DelC_Clicked(object sender, EventArgs e)
        {
           int delin = ((List<Currencys>)Currenlist.ItemsSource).IndexOf(doubletap) - 1;
            if (delin == -1) { return; }
            ((List<(string, double)>)Application.Current.Properties["Currenlist"]).RemoveAt(delin);
            nel.Clear();
            nel.Add(new Currencys("USD", 1));
            foreach ((string, double) dol in (List<(string, double)>)Application.Current.Properties["Currenlist"])
            {
                nel.Add(new Currencys(dol.Item1, dol.Item2));
            }
            Currenlist.ItemsSource = null;
            Currenlist.ItemsSource = nel;
            title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
            SaveAll.GetInstance().SaveTrees2();
            title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies")+ ": "+AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
            DelC.IsVisible = false;
            if (((int)Application.Current.Properties["Currenselect"])== delin) {
                Application.Current.Properties["Currenselect"] = -1;
                Selected.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CurrenciesS") + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"] + 1).Name);
            }
            await Task.Delay(5000);
            title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Currencies");
        }
        protected override void OnAppearing()
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            DelC.IsVisible = false;
            base.OnAppearing();

        }
    }
    public class Currencys
    {
        public string Name { get; set; }
        public double Rate { get; set; }

        public Currencys(string name, double rate)
        {
            Name = name;
            Rate = rate;
        }
        public override string ToString()
        {
            return Name;
        }
    }

    }