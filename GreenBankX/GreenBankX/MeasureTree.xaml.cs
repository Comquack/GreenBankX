using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasureTree : ContentPage
    {
        bool AllOne = false;
        Tree doubletapTree;
           Tree ThisTree;
        ObservableCollection<SelectableData> plotTog;
        public MeasureTree()
        {
            InitializeComponent();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                PickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            plotTog = new ObservableCollection<SelectableData>();
            //PickPrice.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPrice"));
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlotOne.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            pickPlot.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NewPlot"));
        }
        public async void RunCalc() {
            Calculator calc = new Calculator();
            Plot thispolt=null;
            if (pickPlotOne.SelectedIndex > -1) {
                thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
            }
            else {
                try { thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex); }
                catch { }
            }
            if (PickPrice.SelectedIndex > -1)
            {
                double totalv = 0;
                double totalp = 0;
                List<DetailsGraph2> Detail = new List<DetailsGraph2>();
                calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(PickPrice.SelectedIndex)); 
                double[,] result;
                if (calc.GetPrices() == null) {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        bool reslut = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PSPrice"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PSPrice"), "OK", "Add Price Scheme");
                        if (!reslut)
                        {
                            await Navigation.PushAsync(new CreatePricing());
                        }
                    });
                    return;
                }
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
                string[] unitm3 = { "m\xB3" };

                for (int i = 0; i < result.GetLength(0); i++)
                {
                    totalv += result[i, 2];
                    totalp += result[i, 1] * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2));
            DetailsGraph2 answer = new DetailsGraph2 { volume = Math.Round(result[i, 2], 4), price = Math.Round(result[i, 1] * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2)), 2), result=result,tree = ThisTree, brack =brack, resultrow = i };
                    if (result[i, 0] == -1)
                    {
                        answer.label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TooSmall");
                    }
                    else if (result[i, 0] == brack.Count - 1)
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key* (GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + unitcm[0] + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                    }
                    else
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key* (GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2)) + "-" + Math.Round(brack.ElementAt((int)result[i, 0] + 1).Key* (GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + unitcm[0];

                    }
                    Detail.Add(answer);
                }
                DetailsGraph2 answer2 = new DetailsGraph2 {volume= Math.Round(totalv,4), price = Math.Round(totalp,2), label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Totals") };
                Detail.Add(answer2);
                await PopupNavigation.Instance.PushAsync(MeasureResult.GetInstance(Detail));
                // LogList.ItemsSource = Detail;
                //LogList.IsVisible = true;
            }
            else {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool result = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PSPrice"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PSPrice"), "OK", "Add Price Scheme");
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
                    bool res = await DisplayAlert("Measure trees", "This page allows you to input the measurements of trees and add them to your plots.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert("Measure trees", "If a pricing scheme is selected,  the measure button will show you how many logs the tree will produce and their worth.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutmes"] = false;

                    }
                    else
                    {
                        Application.Current.Properties["Tutmes"] = false;
                    }
                });
            }
        }
        public async void RunAdd() {
            if (Double.TryParse(merchheight.Text, out double ans) && ans > 0 && girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1)
            {
                int ID;
                try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                catch { ID = 1; }
                if (DateMes.Date > DateTime.Now)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DFute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                    });
                }
                else if (DateMes.Date == null)
                {
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree")+": "+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    SaveAll.GetInstance().SaveTrees2();
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateTime.Now, ans));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree") + " " + ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName(), "OK");
                    });
                    girth.Text = null;
                    height.Text = null;
                    merchheight.Text = null;
                    MerhH.IsToggled = false;
                    try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                    catch { ID = 1; }
                    labID.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ID;
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    await Task.Delay(5000);
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                }
                else
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateMes.Date));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree") + " " + ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName(), "OK");
                    });
                    girth.Text = null;
                    height.Text = null;
                    merchheight.Text = null;
                    MerhH.IsToggled = false;
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    SaveAll.GetInstance().SaveTrees2();
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                    catch { ID = 1; }
                    labID.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ID;
                    await Task.Delay(5000);
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                }
            }
            else if (girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1)
            {
                int ID;
                try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                catch { ID = 1; }
                if (DateMes.Date > DateTime.Now)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DFute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                    });
                }
                else if (DateMes.Date == null)
                {
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    SaveAll.GetInstance().SaveTrees2();
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateTime.Now));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree") + " " + ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName(), "OK");
                    });
                    girth.Text = null;
                    height.Text = null;
                    merchheight.Text = null;
                    MerhH.IsToggled = false;
                    try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                    catch { ID = 1; }
                    labID.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ID;
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    await Task.Delay(5000);
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                }
                else
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree((double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1)), (double.Parse(height.Text)), ID, DateMes.Date));
                    SaveAll.GetInstance().SaveTrees2();
                    girth.Text = null;
                    height.Text = null;
                    merchheight.Text = null;
                    MerhH.IsToggled = false;
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    SaveAll.GetInstance().SaveTrees2();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SuccTree") + " " + ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName(), "OK");
                    });
                    try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
                    catch { ID = 1; }
                    labID.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ID;
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree") + ": " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    await Task.Delay(5000);
                    title.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                }
            }
        }

        private void pickPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PickPrice.SelectedIndex != -1 && !AllOne) {
                RunCalc();
                PickPrice.SelectedIndex  = - 1;
            }
            else if (PickPrice.SelectedIndex != -1 && AllOne)
            {
                All_Clicked();
                PickPrice.SelectedIndex = -1;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null) {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            DunLLoadin();
            PickPrice.Items.Clear();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                PickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            //PickPrice.Items.Add("New Pricing");
            pickPlot.Items.Clear();
            pickPlotOne.Items.Clear();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlotOne.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            PriceA.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") + "(" + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1) + ")";
            pickPlot.Items.Add("New Plot");
            labbott.IsVisible = false;
            GirthDBH2.IsVisible = false;

        }

        private async void pickPlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ID;
            if (pickPlot.SelectedIndex == ((List<Plot>)Application.Current.Properties["Plots"]).Count)
            {
                await Navigation.PushAsync(new NotPopup());
                return;
            }
            try { ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count - 1).Id + 1; }
            catch { ID = 1; }
            labID.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ID;
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
                girth.Placeholder = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter");
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
            if (e.NewTextValue == null || e.NewTextValue == "") { }
            else if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 100 || double.Parse(e.NewTextValue) <= 0))
            {
                merchheight.Text = e.OldTextValue;
            }
            else if (e.NewTextValue != null && e.NewTextValue != "" && double.TryParse(height.Text, out ans) && ans < double.Parse(e.NewTextValue))
            {
                merchheight.Text = e.OldTextValue;
            }
        }

        private void MerhH_Toggled(object sender, ToggledEventArgs e)
        {
            merchheight.IsVisible = e.Value;
            merchheight.Text = "";
        }

        private void DetailsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            

            if (doubletapTree == ((SelectableData)DetailsList.SelectedItem).tree)
            {
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
                    ThisTree = doubletapTree;
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
            pickPlot.SelectedIndex = pickPlotOne.SelectedIndex;
            pickPlotOne.SelectedIndex = -1;
            
        }

        private void FromPlot_Clicked(object sender, EventArgs e)
        {
            labbott.IsVisible = true;
            GirthDBH2.IsVisible = true;
            pickPlotOne.Focus();
        }

        private void NewTree_Clicked(object sender, EventArgs e)
        {
            labbott.IsVisible = false;
            GirthDBH2.IsVisible = false;
            Show(true);
            pickPlot.IsVisible = true;
            Add.IsVisible = true;
            DetailsList.IsVisible = false;
            pickPlotOne.SelectedIndex = -1;
        }

        private void pickPlotOne_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            plotTog.Clear();
            if (pickPlotOne.SelectedIndex > -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
                List<Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                for (int x = 0; x < TreeList.Count; x++)
                {
                    plotTog.Add(new SelectableData(TreeList.ElementAt(x), false, GirthDBH2.IsToggled));
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
            public SelectableData(Tree str, bool boo, bool gd) {
                tree = str;
                Id = str.ID.ToString();
                Diameter = Math.Round(tree.Diameter * (gd ? 1 / Math.PI:1), 2).ToString();
                MerchHeight = str.MerchHeight.ToString();
                Selected = boo;
            }
            public void GirthDiam(bool gd) {
                Diameter = Math.Round(tree.Diameter * (gd ? 1 / Math.PI:1),2).ToString();
            }
        }

        private void All_select() {
            AllOne = true;
            PickPrice.Focus();
        }

        private async void All_Clicked()
        {
            Plot thispolt = null;
            thispolt = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlotOne.SelectedIndex);
            List<DetailsGraph2> Detail = new List<DetailsGraph2>();
            ObservableCollection<DetailsGraph2> DetailSort = new ObservableCollection<DetailsGraph2>();
            double totalv = 0;
            double totalp = 0;
            for (int x = 0; x < plotTog.Count; x++)
            {

                if (plotTog.ElementAt(x).Selected)
                {
                    Calculator calc = new Calculator();

                    if (thispolt != null && PickPrice.SelectedIndex != -1)
                    {

                        calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(PickPrice.SelectedIndex));
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
                            totalv += result[i, 2];
                            totalp += result[i, 1] * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2));
                            DetailsGraph2 answer = new DetailsGraph2 { volume = Math.Round(result[i, 2], 4), price = Math.Round(result[i, 1] * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2)), 2), result = result,brack = brack, resultrow=i };
                            if (result[i, 0] == -1)
                            {
                                answer.label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TooSmall");
                            }
                            else if (result[i, 0] == brack.Count - 1)
                            {
                                answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key* (GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + unitcm[0] + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                            }
                            else
                            {
                                answer.label = Math.Round(brack.ElementAt((int)result[i, 0]).Key*(GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + "-" + Math.Round(brack.ElementAt((int)result[i, 0] + 1).Key*(GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + unitcm[0];

                            }
                            Detail.Add(answer);
                        }
                    }
                }
            }
            DetailsGraph2 answer2 = new DetailsGraph2 { volume = Math.Round(totalv,4), price = Math.Round(totalp,2), label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Totals") };
            Detail.Add(answer2);
            await PopupNavigation.Instance.PushAsync(MeasureResult.GetInstance(Detail));
        }

        private void Switch_Toggled_1(object sender, ToggledEventArgs e)
        {
            New.IsVisible = false;
            All.IsVisible = false;
            for (int x = 0; x < plotTog.Count; x++) {
                All.IsVisible = All.IsVisible || plotTog.ElementAt(x).Selected;
                New.IsVisible = New.IsVisible && !plotTog.ElementAt(x).Selected;
            }
        }


        private void Estimate_Clicked(object sender, EventArgs e)
        {
            AllOne = false;
            PickPrice.Focus();
        }

        private void GirthDBH2_Toggled(object sender, ToggledEventArgs e)
        {
            if (DetailsList.IsVisible) {
                List<SelectableData> TreeTails = new List<SelectableData>();
                ObservableCollection<SelectableData> TreeOrig = (ObservableCollection<SelectableData>)DetailsList.ItemsSource;
                foreach (SelectableData g in TreeOrig) {
                    g.GirthDiam(GirthDBH2.IsToggled);
                }
                DetailsList.ItemsSource = null;
                DetailsList.ItemsSource = TreeOrig;
               // int hold = pickPlotOne.SelectedIndex;
               // pickPlotOne.SelectedIndex = -1;
               // pickPlotOne.SelectedIndex = hold;
            }
            if (LogList.IsVisible&& LogList.ItemsSource!=null) {
                ObservableCollection<DetailsGraph2> deets = (ObservableCollection<DetailsGraph2>)LogList.ItemsSource;
                ObservableCollection<DetailsGraph2> deets2 = new ObservableCollection<DetailsGraph2>();
                foreach(DetailsGraph2 answer in deets) {
                    SortedList<double, double> brack = answer.brack;
                    double[,] result = answer.result;
                    int i = answer.resultrow;
                    if (result[i, 0] == -1)
                    {
                        //answer.label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TooSmall");
                    }
                    else if (result[i, 0] == brack.Count - 1)
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key * (GirthDBH2.IsToggled ? 1 / Math.PI : 1),2) + "cm" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                    }
                    else
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key*(GirthDBH2.IsToggled ? 1/Math.PI:1),2) + "-" + Math.Round(brack.ElementAt((int)result[i, 0] + 1).Key * (GirthDBH2.IsToggled ? 1 / Math.PI : 1),2) + "cm");

                    }
                    deets2.Add(answer);
                }
                LogList.ItemsSource = null;
                LogList.ItemsSource = deets2;
                LogList.IsVisible = true;
            }
        }

        private void Today_Clicked(object sender, EventArgs e)
        {
            DateMes.Date = DateTime.Now;
        }     
    }
}