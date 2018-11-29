using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MeasureTree : ContentPage
    {
        int Listhadler;
        Tree doubletapTree;
           Tree ThisTree;
        ObservableCollection<SelectableData> plotTog;
        public MeasureTree()
        {
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            plotTog = new ObservableCollection<SelectableData>();
            pickPrice.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPrice"));
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlotOne.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            pickPlot.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPlot"));
        }
        public void RunCalc() {
            Calculator calc = new Calculator();
            Plot thispolt=null;
            if (pickPlotOne.SelectedIndex > -1) {
                thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
            }
            else {
                try { thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex); }
                catch { }
            }
            if (pickPrice.SelectedIndex > -1 || (thispolt != null && thispolt.GetRange() != null))
            {
                ObservableCollection<DetailsGraph2> Detail = new ObservableCollection<DetailsGraph2>();
                if (thispolt != null) { calc.SetPrices(thispolt.GetRange()); } else {
                calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex)); }
                double[,] result;
                if (Double.TryParse(merchheight.Text, out double ans) && ans > 0 && girth.Text != null && height.Text != null)
                {
                    result = calc.Calcs(double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1), double.Parse(height.Text), ans);
                }
                else if (girth.Text != null && height.Text != null)
                { result = calc.Calcs(double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1), double.Parse(height.Text)); }
                else { return; }
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
            Show(false);
            DetailsList.IsVisible = false;
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
            if (Double.TryParse(merchheight.Text, out double ans) && ans > 0 && girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1)
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
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateTime.Now, ans));
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
            if (Application.Current.Properties["Language"] != null) {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            DunLLoadin();
            pickPrice.Items.Clear();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            pickPrice.Items.Add("New Pricing");
            pickPlot.Items.Clear();
            pickPlotOne.Items.Clear();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlotOne.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
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
                girth.Placeholder = "Diameter (cm)";
                if (girth.Text != null) {
                    girth.Text = (Math.Round(double.Parse(girth.Text) / Math.PI, 3)).ToString();
                }
            }
            else {
                girth.Placeholder = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth");
                if (girth.Text != null)
                {
                    girth.Text = (Math.Round(double.Parse(girth.Text) * Math.PI, 3)).ToString();
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
            merchheight.Text = e.Value ? "" : "";
        }

        private void DetailsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            

            if (doubletapTree == ((SelectableData)DetailsList.SelectedItem).tree)
            {
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
                    ThisTree = doubletapTree;
                    Listhadler = -1;
                    doubletapTree = null;
                Show(true);
                pickPlot.IsVisible = false;
                Add.IsVisible = false;
                if (ThisTree.ActualMerchHeight > -1) {
                        merchheight.Text = ThisTree.ActualMerchHeight.ToString();
                        MerhH.IsToggled = true;
                    }
                    height.Text = ThisTree.MerchHeight.ToString();
                    DateMes.Date = ThisTree.GetHistory().Last().Key;
                    pickPlot.SelectedIndex = pickPlotOne.SelectedIndex;
                    girth.Text = ThisTree.Diameter.ToString();
                    pickPlotOne.SelectedIndex = -1;
                for (int x = 0; x < pickPrice.Items.Count; x++) {
                    pickPrice.SelectedIndex = pickPrice.Items.ElementAt(x) == ThisPlot.GetRange().GetName()?x: pickPrice.SelectedIndex;
                }
                
                    

            }
            else
            {
                doubletapTree = ((SelectableData)DetailsList.SelectedItem).tree;
            }

           
        }

        private void New_Clicked(object sender, EventArgs e)
        {
            Show(true);
            Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
            for (int x = 0; x < pickPrice.Items.Count; x++)
            {
                pickPrice.SelectedIndex = pickPrice.Items.ElementAt(x) == ThisPlot.GetRange().GetName() ? x : pickPrice.SelectedIndex;
            }
            pickPlot.SelectedIndex = pickPlotOne.SelectedIndex;
            pickPlotOne.SelectedIndex = -1;
            
        }

        private void FromPlot_Clicked(object sender, EventArgs e)
        {
            pickPlotOne.Focus();
        }

        private void NewTree_Clicked(object sender, EventArgs e)
        {
            Show(true);
            pickPlot.IsVisible = true;
            Add.IsVisible = true;
            DetailsList.IsVisible = false;
            pickPlotOne.SelectedIndex = -1;
        }

        private void pickPlotOne_SelectedIndexChanged(object sender, EventArgs e)
        {
            Listhadler = 0;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            plotTog.Clear();
            if (pickPlotOne.SelectedIndex > -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
                List<Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                for (int x = 0; x < TreeList.Count; x++)
                {
                    plotTog.Add(new SelectableData(TreeList.ElementAt(x), false));
                    ThisTree = TreeList.ElementAt(x);
                    TreeTails.Add(ThisTree);
                }
                DetailsList.IsVisible = true;
                LogList.IsVisible = false;
                DetailsList.ItemsSource = plotTog;
                Show(false);
                DetailsList.IsVisible = true;
                DetailsList.HeightRequest = (40 * Math.Min(TreeTails.Count, 5)) + (10 * Math.Min(TreeTails.Count, 5)) + 60;
            }
        }
        private void Show(bool toggle){
            AddFrame.IsVisible = toggle;
            pickPlot.IsVisible = toggle;
            pickPlot.SelectedIndex = -1;
            DateMes.IsVisible = toggle;
            Add.IsVisible = toggle;
            Estimate.IsVisible = toggle;
            girth.IsVisible = toggle;
            girth.Text = null;
            GirthDBH.IsVisible = toggle;
            height.IsVisible = toggle;
            height.Text = null;
            merchheight.IsVisible = false;
            merchheight.Text = null;
            //pickPrice.IsVisible = toggle;
            DetailsList.IsVisible = !toggle;
            MerhH.IsVisible = toggle;
            lab1.IsVisible = toggle;
            lab2.IsVisible = toggle;
            DetailsList.IsVisible = false;

        }
        public class SelectableData
        {
            public Tree tree { get; set; }
            public string Id { get; set; }
            public string Diameter { get; set; }
            public string MerchHeight { get; set; }
            public bool Selected { get; set; }
            public SelectableData(Tree str, bool boo) {
                tree = str;
                Id = str.ID.ToString();
                Diameter = str.Diameter.ToString();
                MerchHeight = str.MerchHeight.ToString();
                Selected = boo;
            }
        }

        private void All_Clicked(object sender, EventArgs e)
        {
            Plot thispolt = null;
            thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
            ObservableCollection<DetailsGraph2> Detail = new ObservableCollection<DetailsGraph2>();
            ObservableCollection<DetailsGraph2> DetailSort = new ObservableCollection<DetailsGraph2>();
            for (int x = 0; x < plotTog.Count; x++)
            {

                if (plotTog.ElementAt(x).Selected)
                {
                    Calculator calc = new Calculator();

                    if (thispolt != null && thispolt.GetRange() != null)
                    {

                        calc.SetPrices(thispolt.GetRange());
                        double[,] result;

                        result = calc.Calcs(thispolt.getTrees().ElementAt(x).Diameter, thispolt.getTrees().ElementAt(x).Merch, thispolt.getTrees().ElementAt(x).ActualMerchHeight);
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
                    }
                    while(Detail.Count>0)
                    {
                        string ans = Detail.ElementAt(0).label;
                    for (int y = 0; y < Detail.Count; y++)
                    {
                            if (Detail.ElementAt(y).label == ans) {
                                DetailSort.Add(Detail.ElementAt(y));
                                Detail.RemoveAt(y);
                                y--;
                            }
                    }
                }
                    LogList.ItemsSource = DetailSort;
                    LogList.IsVisible = true;
                }
            }
        }

        private void Switch_Toggled_1(object sender, ToggledEventArgs e)
        {
            New.IsVisible = true;
            All.IsVisible = false;
            for (int x = 0; x < plotTog.Count; x++) {
                All.IsVisible = All.IsVisible || plotTog.ElementAt(x).Selected;
                New.IsVisible = New.IsVisible && !plotTog.ElementAt(x).Selected;
            }
        }
    }
}