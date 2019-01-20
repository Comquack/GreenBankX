using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            nel.Add(new Currencys("USD", 1));
            foreach ((string, double) dol in (List<(string, double)>)Application.Current.Properties["Currenlist"]){
                nel.Add(new Currencys(dol.Item1, dol.Item2));
            }
            Selected.Text = "Selected Currency: " + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"]+1).Name);
            Currenlist.ItemsSource = null;
            Currenlist.ItemsSource = nel;
        }

        private void Currenlist_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubletap == Currenlist.SelectedItem)
            {
                Application.Current.Properties["Currenselect"] = ((List<Currencys>)Currenlist.ItemsSource).IndexOf(doubletap)-1;
                Selected.Text = "Selected Currency: " + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"]+1).Name);

            }
            else
            {
                doubletap = (Currencys)Currenlist.SelectedItem;
                DelC.IsVisible = true;
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
                SaveAll.GetInstance().SaveTrees2();
                Name.Text = null;
                Rate.Text = null;
                bool res = await DisplayAlert("Set Currency", "Do you wish to add set this as the currency to use", "Yes", "No");
                if (res)
                {
                    Application.Current.Properties["Currenselect"] = ((List<Currencys>)Currenlist.ItemsSource).Count-2;
                    Selected.Text = "Selected Currency: " + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"] + 1).Name);

                }
            }
        }

        private void DelC_Clicked(object sender, EventArgs e)
        {
           int delin = ((List<Currencys>)Currenlist.ItemsSource).IndexOf(doubletap) - 1;
            ((List<(string, double)>)Application.Current.Properties["Currenlist"]).RemoveAt(delin);
            nel.Clear();
            nel.Add(new Currencys("USD", 1));
            foreach ((string, double) dol in (List<(string, double)>)Application.Current.Properties["Currenlist"])
            {
                nel.Add(new Currencys(dol.Item1, dol.Item2));
            }
            Currenlist.ItemsSource = null;
            Currenlist.ItemsSource = nel;
            SaveAll.GetInstance().SaveTrees2();
            DelC.IsVisible = false;
            if (((int)Application.Current.Properties["Currenselect"])== delin) {
                Application.Current.Properties["Currenselect"] = -1;
                Selected.Text = "Selected Currency: " + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : nel.ElementAt((int)Application.Current.Properties["Currenselect"] + 1).Name);

            }
        }
        protected override void OnAppearing()
        {

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