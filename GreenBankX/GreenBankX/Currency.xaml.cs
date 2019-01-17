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

        private void Conf_Clicked(object sender, EventArgs e)
        {

            if (Name.Text != null && double.TryParse(Rate.Text, out double rates)) {
                ((List<(string, double)>)Application.Current.Properties["Currenlist"]).Add((Name.Text, rates));
                nel.Add(new Currencys(Name.Text, rates));
                Currenlist.ItemsSource = null;
                Currenlist.ItemsSource = nel;
                SaveAll.GetInstance().SaveTrees2();
            }
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