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
                double[,] result;
                if (Double.TryParse(merchheight.Text, out double ans) && ans > 0 && girth.Text != null && height.Text != null)
                {
                    result = calc.Calcs(double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1), double.Parse(height.Text),ans);
                }
                else if (girth.Text != null && height.Text != null)
                { result = calc.Calcs(double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1), double.Parse(height.Text)); }
                else{ return; }      
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
        public void DunLLoadin()
        {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutmes"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Measure trees", "This page allows you to input the measurements of trees and add them to your plots.", "Continue", "Skip");
                    if (res)
                    {
                        await DisplayAlert("Measure trees", "If a pricing scheme is selected,  the measure button will show you how many logs the tree will produce and their worth.", "Next");
                        Application.Current.Properties["Tutmes"] = false;
                        
                    }
                    else
                    {
                        Application.Current.Properties["Tutmes"] = false;
                    }
                });
            }
        }
        public void RunAdd() {
            if (Double.TryParse(merchheight.Text, out double ans)&&ans >0&& girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1)
            {
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
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateTime.Now,ans));
                    SaveAll.GetInstance().SaveTrees2();
                }
                else
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateMes.Date));
                    SaveAll.GetInstance().SaveTrees2();
                }
            }
            else if (girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1)
            {
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
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateTime.Now));
                    SaveAll.GetInstance().SaveTrees2();
                }
                else
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateMes.Date));
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
            DunLLoadin();
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
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out double ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 100 || double.Parse(e.NewTextValue) < 0))
            {
                height.Text = e.OldTextValue;
            }
        }

        private void girth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out double ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                girth.Text = e.OldTextValue;
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                girth.Placeholder = "DBH (cm)";
                if(girth.Text != null) {
                girth.Text =(Math.Round(double.Parse(girth.Text)/Math.PI,3)).ToString();
                }
            }
            else {
                girth.Placeholder = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth");
                if (girth.Text != null)
                {
                    girth.Text = (Math.Round(double.Parse(girth.Text) * Math.PI,3)).ToString();
                }
            }
        }

        private void merchheight_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 100 || double.Parse(e.NewTextValue) <= 0))
            {
                merchheight.Text = e.OldTextValue;
            }
            else if (double.TryParse(height.Text, out ans) && ans < double.Parse(e.NewTextValue))
                {
                    merchheight.Text = e.OldTextValue;
                }
        }

        private void MerhH_Toggled(object sender, ToggledEventArgs e)
        {
            merchheight.IsVisible = e.Value;
            merchheight.Text = e.Value ? "": "";
        }
    }
}