using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public partial class MeasureTree : ContentPage
	{
		public MeasureTree ()
		{
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                
            }
            pickPrice.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPrice"));
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
             }
            pickPlot.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPlot"));
        }
        public void RunCalc() {
            Calculator calc = new Calculator();
            if (pickPrice.SelectedIndex > -1)
            {
                ObservableCollection<DetailsGraph2> Detail = new ObservableCollection<DetailsGraph2>();
                calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                double[,] result = calc.Calcs(double.Parse(girth.Text), double.Parse(height.Text));
                string resText0 = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogClass") + "\n";
                string resText1 = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Price") + "\n";
                string resText2 = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Volume") + "\n";
                SortedList<double, double> brack = calc.GetPrices().GetBrack();
                string[] unitcm = { "cm" };
                string[] unitm = { "m" };
                string[] unitm3 = { "m3" };
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    DetailsGraph2 answer = new DetailsGraph2 { volume = Math.Round(result[i, 2], 4), price = Math.Round(result[i, 1], 2) };
                    if (result[i, 0] == -1)
                    {
                        answer.label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TooSmall");
                    }
                    else if (result[i, 0] == brack.Count - 1)
                    {
                        answer.label = (brack.ElementAt((int)result[i, 0]).Key + unitcm[0] + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                    }
                    else
                    {
                        answer.label = (brack.ElementAt((int)result[i, 0]).Key) + "-" + brack.ElementAt((int)result[i, 0] + 1).Key + unitcm[0];

                    }
                    Detail.Add(answer);
                }
                LogList.ItemsSource = Detail;
                LogList.IsVisible = true;
            }
            else {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool result = await DisplayAlert("Please select a price scheme", "Please select a price scheme", "OK", "Add Price Scheme");
                    if (!result)
                    {
                        await Navigation.PushAsync(new CreatePricing());
                    }
                });
            }
        }

        public void RunAdd() {
            if (girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1) {
                int ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count();
                if (DateMes.Date > DateTime.Now)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DFute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                    });
                }
                else if (DateMes.Date == null)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text)), (double.Parse(height.Text)), ID, DateTime.Now));
                    SaveAll.GetInstance().SaveTrees2();
                }
                else {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text)), (double.Parse(height.Text)), ID, DateMes.Date));
                    SaveAll.GetInstance().SaveTrees2();
                }
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

        private void height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue)>=100|| double.Parse(e.NewTextValue) < 0)) {
                height.Text = e.OldTextValue;
            }
        }

        private void girth_TextChanged(object sender, TextChangedEventArgs e)
        { 
            if (e.NewTextValue !=null&& e.NewTextValue !=""&& ( double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                girth.Text = e.OldTextValue;
            }
        }
    }
}