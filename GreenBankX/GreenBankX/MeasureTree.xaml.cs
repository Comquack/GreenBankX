using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBankX.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MeasureTree : ContentPage
	{
		public MeasureTree ()
		{
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                
            }
            pickPrice.Items.Add(AppResource.ResourceManager.GetString("NewPrice"));
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
             }
            pickPlot.Items.Add(AppResource.ResourceManager.GetString("NewPlot"));
        }
        public void RunCalc() {
            Calculator calc = new Calculator();
            if (pickPrice.SelectedIndex > -1)
            {
                calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                double[,] result = calc.Calcs(double.Parse(girth.Text), double.Parse(height.Text));
                string resText0 = AppResource.ResourceManager.GetString("LogClass") + "\n";
                string resText1 = AppResource.ResourceManager.GetString("Price") + "\n";
                string resText2 = AppResource.ResourceManager.GetString("Volume") + "\n";
                SortedList<double, double> brack = calc.GetPrices().GetBrack();
                string[] unitcm = { "cm", "in" };
                string[] unitm = { "m", "yards" };
                string[] unitm3 = { "m3", "y3" };
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    if (result[i, 0] == -1)
                    {
                        resText0 = resText0 + AppResource.ResourceManager.GetString("TooSmall") + "\n";
                    }
                    else if (result[i, 0]==brack.Count-1) {
                        resText0 = resText0 + (brack.ElementAt((int)result[i, 0]).Key * Math.Pow(0.3937, pickPrice.SelectedIndex)) + unitcm[pickPrice.SelectedIndex] + AppResource.ResourceManager.GetString("OrLarger") + "\n";
                    }
                    else {
                        resText0 = resText0 + (brack.ElementAt((int)result[i, 0]).Key * Math.Pow(0.3937, pickPrice.SelectedIndex)) + "-" + brack.ElementAt((int)result[i, 0]+1).Key + unitcm[pickPrice.SelectedIndex] + "\n";
                    }
                    resText1 = resText1 + Math.Round(result[i, 1], 2) + " kip\n";
                    resText2 = resText2 + Math.Round(result[i, 2], 4) + "m3\n";
                }
                Result0.Text = resText0;
                Result1.Text = resText1;
                Result2.Text = resText2;
            }
        }

        public void RunAdd() {
            if (girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1) {
                int ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count();
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text)), (double.Parse(height.Text)), ID, DateTime.Now));
            }
        }

        private async void pickPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickPrice.SelectedIndex == ((List<PriceRange>)Application.Current.Properties["Prices"]).Count) {
                await Navigation.PushAsync(new CreatePricing());
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            pickPrice.Items.Clear();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            pickPrice.Items.Add("New Pricing");
            pickPlot.Items.Clear();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            pickPlot.Items.Add("New Plot");
        }

        private async void pickPlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickPlot.SelectedIndex == ((List<Plot>)Application.Current.Properties["Plots"]).Count)
            {
                await Navigation.PushAsync(new CreatePlot());
            }
        }
    }
}