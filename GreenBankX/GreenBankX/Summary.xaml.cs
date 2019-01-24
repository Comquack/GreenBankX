using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using GreenBankX.Resources;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Summary : ContentPage
	{
        int brac = -1;
        List<Plot> changedPlot;
        int GraphNo = -1;
        int Listhadler = -1;
        DetailsGraph doubletap = null;
        Tree doubletapTree;
        int year = DateTime.Now.Year;
        ObservableCollection<PlotContainer> plotty = new ObservableCollection<PlotContainer>();
        PlotContainer doubletap2;
        public Summary()
		{
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                PickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());

            }
            changedPlot = new List<Plot>();
        }
        //activates when index is changed in the plot picker
        public async void SelectPlot()
        { string trees = "";
            if (pickTree.SelectedIndex != -1) {
                pickTree.SelectedIndex = -1;
                return;
            }
            Girtdlab.IsVisible = true;
            Girtdswitch.IsVisible = true;
            PlotList.IsVisible = false;
            Listhadler = 0;
            ShowGraph.IsVisible = true;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickPlot.SelectedIndex == pickPlot.Items.Count-1&& pickPlot.SelectedIndex>-1) {
                await Navigation.PushAsync(new NotPopup());
                return;
            }
            if (pickPlot.SelectedIndex > -1)
            {
                ShowGraph.IsVisible = true;
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName()+" ";
                if (ThisPlot.Owner != null && ThisPlot.Owner != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Owner")+": " + ThisPlot.Owner + "\n";
                }
                if (ThisPlot.NearestTown != null && ThisPlot.NearestTown != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Location") + ": " + ThisPlot.NearestTown + "\n";
                }
                if (ThisPlot.Describe != null && ThisPlot.Describe != "")
                {
                    trees += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Comments") + ": " + ThisPlot.Describe + "\n";
                }
                List<string> IDlis = new List<string>();
                List<Tree> TreeList = ThisPlot.getTrees();
                List<DetailsGraph2> Detailstree = new List<DetailsGraph2>();
                Tree ThisTree;
               // pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {

                    ThisTree = TreeList.ElementAt(x);
                    Detailstree.Add(new DetailsGraph2() {girth = Math.Round(ThisTree.Diameter*(Girtdswitch.IsToggled?1 / Math.PI : 1),2), ID = ThisTree.Id, price = ThisTree.Merch,tree = ThisTree});
                    TreeTails.Add(ThisTree);
                    IDlis.Add(ThisTree.ID.ToString());
                }
                pickTree.ItemsSource = IDlis;
                DetailsList.IsVisible = true;
                LogClassList.IsVisible = false;
                LogList.IsVisible = false;
                ListOfTree.Text = "";
                GirthOT.Text = "";
                HeightOT.Text = "";
                DetailsList.ItemsSource = Detailstree;
                DetailsList.HeightRequest = (40 * Math.Min(TreeTails.Count, 5)) + (10 * Math.Min(TreeTails.Count, 5)) + 60;
                PlotTitle.Text = trees;
                pickTree.IsVisible = false;
                Oxy.IsVisible = false;
                
            }
            else {
                base.OnAppearing();
            }
        }
        //activates when index for tree picker is changed
        public void SelectTree() {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutmanage2"]&& (bool)Application.Current.Properties["Tutdt"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Tree information", "This page shows you information about the selected tree.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert("Tree information", "The \"Earlier\" and \"Later\" buttons change the data to the previous/next measurement that was made for that tree. The add measurement button allows you to add new measurement data for the tree.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Tree information", "The first page show for a tree is always the most recent measurement.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutdt"] = false;
                    }
                    else
                    {
                        Application.Current.Properties["Tutdt"] = false;
                    }
                });
            }
            //ShowGraph.SelectedIndex=-1;
            Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
            List<Tree> TreeList = ThisPlot.getTrees();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth") + "\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1 && pickTree.SelectedIndex < pickTree.Items.Count - 1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                GraphNo = ThisTree.GetHistory().Count - 1;
                PickPrice.Focus();
            } 
            else if (pickTree.SelectedIndex == -1)
            {
                SelectPlot();
                return;
            }   
        }

        public void DunLLoadin()
        {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutmanage"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Plot Summary", "This page allows you to view the summary of the plots you have created.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert("Plot Summary", "Double tapping on a plot will show you further details about the plot.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("View Trees", "If you tap a tree in the list twice, you will be shown additional details.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Plot Data", "The \"Plot data\" selector allows you to see data about the plot such as averages and number of logs per size bracket.",AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Show on Map", "By pressing 'show on map' (botom of list), you can see all of the plots on a map. By activating the switch next to each plot you can select specific plots to show on the map", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutmanage"] = false;
                        Application.Current.Properties["Tutmanage2"] = true;
                        Application.Current.Properties["TLogs"] = true;
                    }
                    else
                    {
                        Application.Current.Properties["Tutmanage"] = false;
                    }
                });
            }
        }

        //Renders tree informaition
        private void LatEar()
        {
            string trees = "";
            string girthtext = "";
            string stuff = "";
            double totVol = 0;
            DetailsList.IsVisible = false;
            LogClassList.IsVisible = false;
            LogList.IsVisible = false;
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);

                double girth = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item1;
                double high = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item2;
                Application.Current.Properties["HCounter"] = GraphNo;


                if (PickPrice.SelectedIndex>-1)
                { 
                    PriceRange thisRange = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(PickPrice.SelectedIndex);
                    Calculator Calc = new Calculator();
                    Calc.SetPrices(thisRange);
                    double[,] result;
                    if (ThisTree.ActualMerchHeight == -1)
                    {
                        result = Calc.Calcs(girth, high);
                    }
                    else
                    {
                        result = Calc.Calcs(girth, high, ThisTree.ActualMerchHeight);
                    }
                    double total = 0;
                    List<string> Lablels = new List<string>();
                    List<string> ListLablels = new List<string>();
                    List<ColumnItem> ItemsSource = new List<ColumnItem>();
                    for (int x = -1; x < thisRange.GetBrack().Count; x++)
                    {
                        ItemsSource.Add(new ColumnItem { CategoryIndex = x + 1 });
                        if (x == -1)
                        {
                            Lablels.Add("<" + Math.Round((thisRange.GetBrack().ElementAt(0).Key * (Girtdswitch.IsToggled ? 1 : Math.PI)),2).ToString() + "cm");
                        }
                        else if (x == thisRange.GetBrack().Count - 1)
                        {
                            Lablels.Add(">" + Math.Round(thisRange.GetBrack().ElementAt(x).Key * (Girtdswitch.IsToggled ? 1 : Math.PI),2) + "cm");
                        }
                        else
                        {
                            Lablels.Add(Math.Round(thisRange.GetBrack().ElementAt(x + 1).Key * (Girtdswitch.IsToggled ? 1 : Math.PI),2) + "cm");
                        }
                    }
                    int[] logs = new int[thisRange.GetBrack().Count + 1];

                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        logs[(int)result[x, 0] + 1]++;
                        total = +result[x, 1];
                        totVol += result[x, 2];
                    }
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = Math.Round((double)logs[x] / (double)(result.GetLength(0)) * 100, 2);
                    }
                    string title = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ThisTree.ID.ToString() + " " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Date") + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();

                    //Later.IsVisible = true;
                    OxyBar(title, Lablels, ItemsSource);
                    Oxy.IsVisible = true;
                    //Later.IsVisible = true;
                   // Earlier.IsVisible = true;
                    if (GraphNo <= 0)
                    {
                        Earlier.IsVisible = false;
                    }

                    if (GraphNo >= ThisTree.GetHistory().Count - 1)
                    {
                        Later.IsVisible = false;
                    }
                    stuff = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth") + ": " + Math.Round(girth, 2).ToString() + "\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + ": " + Math.Round(high, 2).ToString();
                    girthtext = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalLogs") + ": " + result.GetLength(0) + "\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") + ": " + Math.Round(total * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2)),2) + ((int)Application.Current.Properties["Currenselect"]==-1?"USD": ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1) ;
                    trees = "Tree ID: " + ThisTree.ID.ToString() + "at the date" + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    GirthOT.Text = girthtext;
                    ListOfTree.Text = stuff;
                    PlotTitle.Text = trees;
                    HeightOT.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalVol") + ": " + Math.Round(totVol, 4);

                }
            }
        }

        private void Earlier_Clicked(object sender, EventArgs e)
        {
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {    
                if (GraphNo > 0)
                {
                    GraphNo--;
                    LatEar();
                }

            }
            else if (pickPlot.SelectedIndex > -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                SortedList<int, List<double>> dates = new SortedList<int, List<double>>();
                for (int x = 0; x < ThisPlot.getTrees().Count; x++)
                {
                    SortedList<DateTime, (double, double,double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
                    try { dates.Add(thisHistory.First().Key.Year, new List<double>()); }
                    catch { }
                    try { dates.Add(thisHistory.Last().Key.Year, new List<double>()); }
                    catch { }
                }
                year = Math.Max(year - 1, dates.First().Key);

                ShowGraphpick();
                if (year <= dates.First().Key)
                {
                    Earlier.IsVisible = false;
                }
            }
            }

            private void Later_Clicked(object sender, EventArgs e)
        {
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                if (GraphNo < ThisTree.GetHistory().Count - 1)
                {
                    GraphNo++;
                    LatEar();
                }

            }
            else if(pickPlot.SelectedIndex > -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                SortedList<int, List<double>> dates = new SortedList<int, List<double>>();
                for (int x = 0; x < ThisPlot.getTrees().Count; x++)
                    {
                        SortedList<DateTime, (double, double,double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
                        try { dates.Add(thisHistory.First().Key.Year, new List<double>());}
                        catch { }
                        try { dates.Add(thisHistory.Last().Key.Year, new List<double>()); }
                        catch { }
                    }
                year = Math.Min(year+1, dates.Last().Key);
                ShowGraphpick();
                if (year >= dates.Last().Key)
                {
                    Later.IsVisible = false;
                }
            }

        }

        //data displaed changes when selection is changed
        private void ShowGraphpick()
        {
            if (ShowGraph.SelectedIndex == 0)
            {
                ShowGraphpick2();
                brac = -1;
                return;
            }
            else if (ShowGraph.SelectedIndex == -1) {
                return;
            }
            PickPrice.SelectedIndex = -1;
            PickPrice.Focus();
        }
        private void ShowGraphpick2()
        {
            ListOfTree.IsVisible = false;
            GirthOT.IsVisible = false;
            Girtdlab.IsVisible = true;
            Girtdswitch.IsVisible = true;
            if (ShowGraph.SelectedIndex > -1)
            {

            }
            else { brac = -1; }
            //show regular data for each tree
            if (ShowGraph.SelectedIndex == 0) {
                SelectPlot();
                Earlier.IsVisible = false;
                Later.IsVisible = false;
                ShowGraph.SelectedIndex -= 1;
            }
            // averages and data by log class
           else if (ShowGraph.SelectedIndex == 1 || ShowGraph.SelectedIndex == 2)
            {

                ObservableCollection<DetailsGraph> Detail = new ObservableCollection<DetailsGraph>();
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                if (ThisPlot.getTrees().Count<=0)
                {
                    
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NoTrees"), "This plot contains no trees.", "OK");
                            return;
                        });
                    
                }
                PriceRange thisRange = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(PickPrice.SelectedIndex);
                Calculator Calc = new Calculator();
                Calc.SetPrices(thisRange);

                double total = 0;
                List<string> Lablels = new List<string>();
                List<string> ListLablels = new List<string>();
                List<ColumnItem> ItemsSource = new List<ColumnItem>();
                for (int x = -1; x < thisRange.GetBrack().Count; x++)
                {
                    ItemsSource.Add(new ColumnItem { CategoryIndex = x+1 });
                    if (x == -1)
                    {
                        Lablels.Add("<" + Math.Round((thisRange.GetBrack().ElementAt(0).Key * (Girtdswitch.IsToggled ? 1 : Math.PI)), 2).ToString() + "cm");
                    }
                    else if (x == thisRange.GetBrack().Count - 1)
                    {
                        Lablels.Add(">" + Math.Round(thisRange.GetBrack().ElementAt(x).Key * (Girtdswitch.IsToggled ? 1 : Math.PI), 2) + "cm");
                    }
                    else
                    {
                        Lablels.Add(Math.Round(thisRange.GetBrack().ElementAt(x + 1).Key * (Girtdswitch.IsToggled ? 1 : Math.PI), 2) + "cm");
                    }
                }

                int[] logs = new int[thisRange.GetBrack().Count + 1];
                double[] vols = new double[thisRange.GetBrack().Count + 1];
                double[] vals = new double[thisRange.GetBrack().Count + 1];
                double totalvol = 0;
                double totalvolM = 0;
                double totalDia = 0;
                double totalTree = 0;
                int count=0;
                int tcount = 0;
                for (int y = 0; y < ThisPlot.getTrees().Count; y++)
                {

                    Tree ThisTree = ThisPlot.getTrees().ElementAt(y);
                    SortedList<DateTime, (double, double,double)> Thistory = ThisTree.GetHistory();
                    try {
                        (double, double,double) measure = Thistory.Where(z => z.Key < DateTime.ParseExact((year + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value;
                        double[,] result = Calc.Calcs(measure.Item1, measure.Item2);
                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        logs[(int)result[x, 0] + 1]++;
                        vols[(int)result[x, 0] + 1] += result[x, 2];
                        vals[(int)result[x, 0] + 1] += result[x, 1];
                        totalvol += result[x, 2];
                            totalvolM += ((result[x, 0]==-1)?0:1)*result[x, 2];
                        total += result[x, 1];
                        totalDia += Math.Max(result[x, 3],0);
                        count+= ((result[x, 0] == -1) ? 0 : 1);
                    }
                    } catch { }
                    totalTree += ThisTree.GetDia() / Math.PI;
                    tcount++;
                }
                // data by log class
                if (ShowGraph.SelectedIndex == 1)
                {
                    GirthOT.Text = "";
                    if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["TLogs"])
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Logs", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SummaryTute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"));
                        });
                        Application.Current.Properties["TLogs"] = false;
                    }
                    Listhadler = 1;
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = Math.Round((double)logs[x]/ (double)count*100,2);
                    }
                    //string title = "Total Logs for Plot (year:"+year.ToString()+ "):";
                    string title = "Total Logs for Plot";


                    OxyBar(title, Lablels, ItemsSource);
                    for (int x = 1; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        Detail.Add(new DetailsGraph { label = Lablels.ElementAt(x), volume = Math.Round(vols[x], 4), price = Math.Round(vals[x] * (((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2)),2), logs = logs[x] });
                    }
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = true;
                    LogclPr.Text =  AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") + " " + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1);
                    LogList.IsVisible = false;
                   ListOfTree.Text = "";
                    GirthOT.Text = "";
                    HeightOT.Text = "";
                    LogClassList.ItemsSource = Detail;
                    LogClassList.HeightRequest = (40 * Detail.Count) + (10 * Detail.Count);
                   // Later.IsVisible = true;
              //  Earlier.IsVisible = true;
                }
                else
                {

                        SelectPlot();
                    ListOfTree.IsVisible = true;
                    GirthOT.IsVisible = true;
                    DetailsList.IsVisible = false;
                   // Oxy.Model = new OxyPlot.PlotModel();
                    ListOfTree.Text = "Year:\n"
                        + "Number of Trees:\n"
                        + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanD") + "(Merchantable Logs):\n"
                        + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanD") + "(Trees):" + "\n"
                         + "Total Volume: \n"
                        +  "Total Volume" + "(Merchantable Logs):\n"
                       +"Total Value: \n"+"Number of Logs(Merchantable):\n"+"Area:";
                    HeightOT.IsVisible = false;
                    HeightOT.Text = "";
                    GirthOT.Text = year.ToString() + "\n"
                        + tcount + "\n"
                        + Math.Round((totalDia / (double)count), 2).ToString() + "cm\n"
                        + Math.Round((totalTree / (double)tcount), 2).ToString() + "cm\n"
                         + Math.Round((totalvol), 2).ToString() + "m\xB3\n"
                        + Math.Round((totalvolM), 2).ToString() + "m\xB3\n"
                        + Math.Round((total), 2).ToString() + ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1) + "\n"  
                        + count+"\n"
                        +ThisPlot.GetArea();
                    pickTree.IsVisible = false;
                    Girtdlab.IsVisible = false;
                    Girtdswitch.IsVisible = false;
                }



            }
        }
        //Render  a OxyPlot Graph containing a bar chart
        private void OxyBar(string title, List<string> Lablels, List<ColumnItem> ItemsSource) {

            Oxy.Model = new OxyPlot.PlotModel
            {
                Title = title
            };
            var barSeries = new ColumnSeries
            {
                ItemsSource = ItemsSource,
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"
            };

            Oxy.Model.Series.Add(barSeries);
            CategoryAxis newAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogClass"),
                ItemsSource = Lablels,
                Angle = 20
            };
            newAxis.IsZoomEnabled = false;
            newAxis.IsPanEnabled = false;
            Oxy.Model.Axes.Add(newAxis);
            var linearAxis1 = new LinearAxis
            { Maximum = 100,
                Title = "% of logs",
                Position = AxisPosition.Left,
            };
            linearAxis1.IsZoomEnabled = false;
            linearAxis1.IsPanEnabled = false;
            Oxy.Model.Axes.Add(linearAxis1);
            Oxy.IsVisible = true;
        }

        private void DetailsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (doubletapTree == ((DetailsGraph2)DetailsList.SelectedItem).tree)
            //{
                
            //    if (Listhadler == 0)
            //    {
            //        Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
            //        pickTree.SelectedIndex = ThisPlot.getTrees().IndexOf(doubletapTree);
                    
            //        DetailsList.IsVisible = false;
            //        LogClassList.IsVisible = false;
            //        LogList.IsVisible = false;
            //        Listhadler = -1;
            //        doubletapTree = null;
            //        ShowGraph.IsVisible = false;
            //    }
            //}
            //else {
            //   doubletapTree = ((DetailsGraph2)DetailsList.SelectedItem).tree;
            //}
        }
        private void LogClassInfoPlot(int bracNo)
        {
            PlotTitle.Text = "";
            Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
            ObservableCollection<DetailsGraph2> Detail = new ObservableCollection<DetailsGraph2>();
            Calculator Calc = new Calculator();
            Calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(PickPrice.SelectedIndex));
            for (int y = 0; y < ThisPlot.getTrees().Count; y++)
            {
                Tree ThisTree = ThisPlot.getTrees().ElementAt(y);
                SortedList<DateTime, (double, double,double)> Thistory = ThisTree.GetHistory();
                try
                {
                    
                    double[,] result = Calc.Calcs(ThisTree.GetDia(), ThisTree.Merch);
                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        if ((int)result[x, 0] == bracNo)
                        {

                            Detail.Add(new DetailsGraph2 { tree = ThisTree,ID = ThisTree.ID, girth = Math.Round(result[x, 3] *(Girtdswitch.IsToggled?1/Math.PI:1), 2), price = Math.Round(result[x, 1]*(((int)Application.Current.Properties["Currenselect"] == -1 ? 1 : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item2)),2), volume = Math.Round(result[x, 2], 2) });
                        }
                    }
                }

                catch { }
            }
            LogList.ItemsSource = null;
            LogList.ItemsSource = Detail;
            LogList.HeightRequest = HeightRequest = (40 * Math.Min(((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count, 5)) + (10 * Math.Min(((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count, 5));
            DetailsList.IsVisible = false;
            LogClassList.IsVisible = false;
            Loglistpr.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") +" "+  ((int)Application.Current.Properties["Currenselect"] == -1 ? "USD" : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1);
            LogList.IsVisible = true;
            Oxy.IsVisible = false;
            ListOfTree.Text = "";
           
            GirthOT.Text = "";
            HeightOT.Text = "";
        }

        private void LogClassList_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            if (doubletap == e.SelectedItem)
            {
                if (Listhadler == 1)
                {
                    for (int x = 0; x < ((ObservableCollection<DetailsGraph>)LogClassList.ItemsSource).Count; x++)
                    {
                        if (LogClassList.SelectedItem.ToString() == ((ObservableCollection<DetailsGraph>)LogClassList.ItemsSource).ElementAt(x).ToString())
                        {
                            brac = x;
                            LogClassInfoPlot(x);
                        }
                    }
                    Listhadler = -1;
                }
            }
            else
            {
                doubletap = (DetailsGraph)e.SelectedItem;
            }
        }

        protected override void  OnAppearing() {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            brac = -1;
            Girtdlab.IsVisible = false;
            Girtdswitch.IsVisible = false;
            PlotTitle.IsVisible = false;
            DunLLoadin();
            GraphNo = -1;
             Listhadler = -1;
             doubletap = null;
            pickTree.IsVisible = false;
            DetailsList.IsVisible = false;
            LogClassList.IsVisible = false;
             doubletapTree = null;
             year = DateTime.Now.Year;
            pickPlot.Items.Clear();
            plotty.Clear();
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
                ((List<Plot>)Application.Current.Properties["Plots"]).Count();
                for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                {
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                plotty.Add(new PlotContainer(thisPlot.GetName(), thisPlot.getTrees().Count, thisPlot.YearPlanted));
                    pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlot.SelectedIndex = -1;
                }
                PlotList.ItemsSource= plotty;
                pickPlot.Items.Add("Add Plot");
            pickPlot.SelectedIndex = -1;
            ShowGraph.IsVisible = false;
            PlotList.IsVisible = true;
            Oxy.IsVisible = false;
            }
        protected override void OnSizeAllocated(double width, double height) {
            base.OnSizeAllocated(width,height);
            if (DetailsList.ItemsSource.GetType() == new ObservableCollection<Tree>().GetType())
            {
                if (width < height)
                {
                    DetailsList.HeightRequest = (height/14 * Math.Min(((ObservableCollection<Tree>)DetailsList.ItemsSource).Count, 5)) + height / 10;
                }
                else
                {
                    DetailsList.HeightRequest = (width / 14 * ((ObservableCollection<Tree>)DetailsList.ItemsSource).Count)  + width / 10;
                }
            }
            if (LogList.ItemsSource.GetType() == new ObservableCollection<DetailsGraph2>().GetType())
            {
                if (width < height)
                {
                    LogList.HeightRequest = HeightRequest = (height / 14 * Math.Min(((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count, 5));
                }
                else
                {
                    LogList.HeightRequest = HeightRequest = (width / 14 * ((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count);
                }
            }
           
        }

        private void PlotList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubletap2 == PlotList.SelectedItem)
            {
                ShowGraph.IsVisible = true;
                pickPlot.SelectedIndex = plotty.IndexOf((PlotContainer)PlotList.SelectedItem);
            }
            else
            {
                doubletap2 = (PlotContainer)PlotList.SelectedItem;
            }
        }

        protected override bool OnBackButtonPressed() {
            if (DetailsList.IsVisible) {
                OnAppearing();
                PlotTitle.IsVisible = false;
                Earlier.IsVisible = false;
                Later.IsVisible = false;
                Girtdlab.IsVisible = false;
                Girtdswitch.IsVisible = false;
                return true;
            } else if (Oxy.IsVisible) {
                int store = pickPlot.SelectedIndex;
                pickPlot.SelectedIndex = -1;
                pickPlot.SelectedIndex = store;
                Earlier.IsVisible = false;
                Later.IsVisible = false;
                return true;
            }
            else {
                base.OnBackButtonPressed();
                return false;
            }
        }

       async private void ShowMap_Clicked(object sender, EventArgs e)
        {
            List<Plot> shower = new List<Plot>();
            Application.Current.Properties["PlotsOnMap"] = new List<Plot>();

            bool selected = false;
            for (int x = 0; x < plotty.Count; x++)
            {
                if (plotty.ElementAt(x).Selected) { shower.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x)); }
                selected = selected || plotty.ElementAt(x).Selected;
            }
            if (shower.Count == 0)
            {
                Application.Current.Properties["PlotsOnMap"] = Application.Current.Properties["Plots"];
            }
            else {
                Application.Current.Properties["PlotsOnMap"] = shower;
            }
            await PopupNavigation.Instance.PushAsync(new ShowonmapPopup());
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            bool selected = false;
            for (int x = 0; x < plotty.Count; x++)
            {
                selected = selected || plotty.ElementAt(x).Selected;
            }
            ShowMap.Text = selected ? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ShowOnMap") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ShowAllOnMap");
        }

        private void PickPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PickPrice.SelectedIndex > -1) {
                if (pickTree.SelectedIndex > -1)
                {
                    LatEar();
                }
                else
                {
                    ShowGraphpick2();
                }
            }

        }

        private void Girtdswitch_Toggled(object sender, ToggledEventArgs e)
        {
            GirthDetailsList.Text = Girtdswitch.IsToggled? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth");
            Girthloglist.Text = GirthDetailsList.Text;
            
            if (LogClassList.IsVisible)
            {  
                ShowGraphpick2();
            }
            else if (DetailsList.IsVisible) {
                SelectPlot();
            }
            if (brac > -1)
            {
                LogClassInfoPlot(brac);
            }

        }
    }

    public class PlotContainer
    {
        public string Name { get; set; }
        public int NoTrees { get; set; }
        public int DateMade { get; set; }
        public bool Selected { get; set; }
        public PlotContainer(string name, int NoT, int date)
        {
            Selected = false;
            Name = name;
            DateMade = date;
            NoTrees = NoT;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}