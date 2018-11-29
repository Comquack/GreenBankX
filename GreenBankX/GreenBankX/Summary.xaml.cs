﻿using System;
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
        List<Plot> changedPlot;
        int GraphNo = -1;
        int Listhadler = -1;
        DetailsGraph doubletap = null;
        Tree doubletapTree;
        int year = DateTime.Now.Year;
		public Summary()
		{
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            changedPlot = new List<Plot>();
        }
        //activates when index is changed in the plot picker
        public async void SelectPlot()
        { string trees = "";
            Listhadler = 0;
            ShowGraph.IsVisible = true;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickPlot.SelectedIndex == pickPlot.Items.Count-1&& pickPlot.SelectedIndex>-1) {
                await Navigation.PushAsync(new CreatePlot());
                return;
            }
            if (pickPlot.SelectedIndex > -1 )
            {
                   Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName();
                if (ThisPlot.Owner != null&& ThisPlot.Owner != "") {
                    trees +="Owner: " + ThisPlot.Owner+"\n";
                }
                if (ThisPlot.NearestTown != null&& ThisPlot.NearestTown != "")
                {
                    trees += "Location: " + ThisPlot.NearestTown + "\n";
                }
                if (ThisPlot.Describe != null && ThisPlot.Describe != "")
                {
                    trees += "Comments: " + ThisPlot.Describe + "\n";
                }
                List <Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {
                    
                    ThisTree = TreeList.ElementAt(x);
                    TreeTails.Add(ThisTree);
                    pickTree.Items.Add(ThisTree.ID.ToString()); 
                }
                pickTree.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddTree"));
                DetailsList.IsVisible = true;
                LogClassList.IsVisible = false;
                LogList.IsVisible = false;
                ListOfTree.Text = "";
                GirthOT.Text = "";
                HeightOT.Text = "";
                DetailsList.ItemsSource = TreeTails;
                DetailsList.HeightRequest = (40 * Math.Min(TreeTails.Count,5)) + (10 * Math.Min(TreeTails.Count, 5)) +60;
                PlotTitle.Text = trees;
                pickTree.IsVisible = true;
                Oxy.IsVisible = false;
                ShowGraph.IsVisible = false;
            }
        }
        //activates when index for tree picker is changed
        public void SelectTree() {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutmanage2"]&& (bool)Application.Current.Properties["Tutdt"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Tree information", "This page shows you information about the selected tree.", "Continue", "Skip");
                    if (res)
                    {
                        await DisplayAlert("Tree information", "The \"Earlier\" and \"Later\" buttons change the data to the previous/next measurement that was made for that tree. The add measurement button allows you to add new measurement data for the tree.", "Next");
                        await DisplayAlert("Tree information", "The first page show for a tree is always the most recent measurement.", "Next");
                        Application.Current.Properties["Tutdt"] = false;
                    }
                    else
                    {
                        Application.Current.Properties["Tutdt"] = false;
                    }
                });
            }
            ShowGraph.SelectedIndex=-1;
            Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
            List<Tree> TreeList = ThisPlot.getTrees();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth") + "\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1 && pickTree.SelectedIndex < pickTree.Items.Count - 1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                GraphNo = ThisTree.GetHistory().Count - 1;
                LatEar();
                Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(20, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(90, GridUnitType.Auto);

            } else if (pickTree.SelectedIndex == pickTree.Items.Count - 1) {
                return;
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
                    bool res = await DisplayAlert("Manage Plots", "This page allows you to manage the plots you have created.", "Continue", "Skip");
                    if (res)
                    {
                        await DisplayAlert("Manage Plots", "After selecting a plot from the menu, you will be shown the list trees on the plot. The  plot can be added to by pressing the add to plot button, or by going to the measure trees page", "Next");
                        await DisplayAlert("View Trees", "If you tap a tree in the list twice, you will be shown additional details.", "Next");
                        await DisplayAlert("Plot Data", "The \"Plot data\" selector allows you to see data about the plot such as averages and number of logs per size bracket.", "Next");
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

        public async void AddNewTree()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"] = pickPlot.SelectedIndex;
                MessagingCenter.Unsubscribe<AddTreePop>(this, "Add");
        
               MessagingCenter.Subscribe<AddTreePop>(this, "Add", (sender) =>
               {
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    pickTree.Items.Clear();
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                   pickTree.Items.Add("Add new Tree");
                   pickTree.SelectedIndex = ThisPlot.getTrees().Count - 1;
                   SelectTree();
                   SaveAll.GetInstance().SaveTrees2();

               });
                await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
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


                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange() != null)
                {
                    
                    PriceRange thisRange = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange();
                    Calculator Calc = new Calculator();
                    Calc.SetPrices(thisRange);
                    double[,] result;
                    if (ThisTree.ActualMerchHeight == -1)
                    {
                        result = Calc.Calcs(girth, high);
                    }
                    else {
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
                            Lablels.Add("Too\n Small");
                            ListLablels.Add("Too Small");
                        }
                        else if (x == thisRange.GetBrack().Count - 1)
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                            ListLablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                        }
                        else
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm -\n" + thisRange.GetBrack().ElementAt(x + 1).Key.ToString() + "cm");
                            ListLablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x + 1).Key.ToString() + "cm");
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
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TreeID") + ": " + ThisTree.ID.ToString() + " " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Date") + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();

                   
                    Later.IsVisible = true;
                    OxyBar(title, Lablels, ItemsSource);
                    Later.IsVisible = true;
                    Earlier.IsVisible = true;
                    if (GraphNo <= 0)
                    {
                        Earlier.IsVisible = false;
                    }

                    if (GraphNo >= ThisTree.GetHistory().Count - 1)
                    {
                        Later.IsVisible = false;
                    }
                    stuff = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth") + ": " + Math.Round(girth, 2).ToString() + "\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + ": " + Math.Round(high, 2).ToString();
                    girthtext = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalLogs")+": " + result.GetLength(0) + "\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") + ": " + Math.Round(total, 2) + "k";
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
        public void Save()
        {
            //if (saveplot)
            //{
                SaveAll.GetInstance().SavePlots();
            //}
            //if (savetree)
            //{
                SaveAll.GetInstance().SaveTrees2();
            //}
            SaveAll.GetInstance().Kamel();
        }

        //data displaed changes when selection is changed
        private void ShowGraphpick()
        {
            if (ShowGraph.SelectedIndex > -1) {
                DetailsList.IsVisible = false;
                LogClassList.IsVisible = false;
                LogList.IsVisible = false;
            }
            //show regular data for each tree
            if (ShowGraph.SelectedIndex == 0) {
                SelectPlot();
                Earlier.IsVisible = false;
                Later.IsVisible = false;
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
                            await DisplayAlert("Plot has no trees", "This plot contains no trees.", "OK");
                            return;
                        });
                    
                }
                if (ThisPlot.GetRange() == null)
                {

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Plot has no prices", "This plot does not have a set price scheme.", "OK");
                        return;
                    });
                    return;

                }
                try
                {
                    int spangle = ThisPlot.GetRange().GetBrack().Count;
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Plot has no prices", "This plot does not have a set price scheme.", "OK");
                    });
                    return;
                }
                PriceRange thisRange = ThisPlot.GetRange();
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
                        Lablels.Add("Too\n Small");
                        ListLablels.Add("Too Small");
                    }
                    else if (x == thisRange.GetBrack().Count - 1)
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm\n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                        ListLablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                    }
                    else
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm -\n" + thisRange.GetBrack().ElementAt(x+1).Key.ToString() + "cm");
                        ListLablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x + 1).Key.ToString() + "cm");
                    }
                }

                int[] logs = new int[thisRange.GetBrack().Count + 1];
                double[] vols = new double[thisRange.GetBrack().Count + 1];
                double[] vals = new double[thisRange.GetBrack().Count + 1];
                double totalvol = 0;
                double totalDia = 0;
                int count=0;

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
                        total += result[x, 1];
                        totalDia += result[x, 3];
                            count++;
                    }
                    } catch { }
                   
                }
                // data by log class
                if (ShowGraph.SelectedIndex == 1)
                {
                    if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["TLogs"])
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Logs", "This page shows you data about the logs produced by this plot. Double tapping a log size will show a list of every log in that catagory and which tree it would come from.", "Continue");
                        });
                        Application.Current.Properties["TLogs"] = false;
                    }
                    Listhadler = 1;
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = "Total Logs for Plot (year:"+year.ToString()+ "):";


                    OxyBar(title, Lablels, ItemsSource);
                    for (int x = 1; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        Detail.Add(new DetailsGraph { label = ListLablels.ElementAt(x), volume = Math.Round(vols[x], 4), price = Math.Round(vals[x], 4) });
                    }
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = true;
                    LogList.IsVisible = false;
                    ListOfTree.Text = "";
                    GirthOT.Text = "";
                    HeightOT.Text = "";
                    LogClassList.ItemsSource = Detail;
                    LogClassList.HeightRequest = (40 * Detail.Count) + (10 * Detail.Count);
                    Later.IsVisible = true;
                Earlier.IsVisible = true;
                }
                else
                {
                    SelectPlot();
                    DetailsList.IsVisible = false;
                    Oxy.Model = new OxyPlot.PlotModel();
                    ListOfTree.Text = "Year: \n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanD") + " \n " + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanV") + " \n" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanP") +  "\nTotal Volume: \n Total Value: \n";
                    HeightOT.Text = "";
                    Later.IsVisible = true;
                    Earlier.IsVisible = true;
                    GirthOT.Text = year.ToString()+"\n"+Math.Round((totalDia / (double)count), 4).ToString() + "\n" + Math.Round((totalvol / (double)count), 4).ToString() + "\n" + Math.Round((total / (double)count), 4).ToString() + "\n" + Math.Round((totalvol), 4).ToString() + "\n" + Math.Round((total), 4).ToString() + "\n";
                    Later.IsVisible = true;
                    Earlier.IsVisible = true;
                }



            }//plot over time
            else if (ShowGraph.SelectedIndex == 3 && ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().Count>0) {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                PriceRange thisRange = ThisPlot.GetRange();
                Calculator Calc = new Calculator();
                Calc.SetPrices(thisRange);
                List<double> heightOtime = new List<double>();
                if (pickTree.SelectedIndex == -1)
                {
                    GirthOT.Text = "";

                    List<OxyPlot.DataPoint> ItemsSource = new List<OxyPlot.DataPoint>();
                    SortedList<int, List<double>> dates = new SortedList<int, List<double>>();
                    for (int x = 0; x < ThisPlot.getTrees().Count; x++)
                    {
                        SortedList<DateTime, (double, double,double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
                        try { dates.Add(thisHistory.First().Key.Year, new List<double>());}
                        catch { }
                        try { dates.Add(thisHistory.Last().Key.Year, new List<double>()); }
                        catch { }
                    }
                    for (int y = dates.First().Key; y <= dates.Last().Key; y++)
                    {
                        try { dates.Add(y, new List<double>()); }
                        catch { }
                    }
                    for (int x = 0; x < ThisPlot.getTrees().Count; x++)
                    {
                        SortedList<DateTime, (double, double,double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
                        for (int y = dates.First().Key; y <= dates.Last().Key; y++)
                        {
                            if (y >= thisHistory.First().Key.Year && y <= thisHistory.Last().Key.Year) {
                                ((List<double>)dates[y]).Add(thisHistory.Where(z => z.Key < DateTime.ParseExact((y + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value.Item2);
                            }
                        }
                    }
                    double previous = 0;
                    for (int y = dates.First().Key; y <= dates.Last().Key; y++)
                    {
                        try
                        {
                            previous = ((List<double>)dates[y]).Sum() / ((List<double>)dates[y]).Count;
                        }
                        catch { }
                        ItemsSource.Add(new OxyPlot.DataPoint(y, previous));
                    }
                    Oxy.Model = new OxyPlot.PlotModel
                    {
                        Title = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeanG")+" (year: " + year.ToString()+")"
                    };
                    Oxy.MinimumHeightRequest = 1000;
                    var LineSeries = new LineSeries
                    {
                        ItemsSource = ItemsSource

                    };
                    LinearAxis newAxis = new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Key = "Year",
                    };
                    newAxis.IsZoomEnabled = false;
                    newAxis.IsPanEnabled = false;
                    Oxy.Model.Axes.Add(newAxis);
                    var linearAxis1 = new LinearAxis
                    {
                        Position = AxisPosition.Left,
                    };
                    Oxy.Model.Series.Add(LineSeries);
                    linearAxis1.IsZoomEnabled = false;
                    linearAxis1.IsPanEnabled = false;
                    Oxy.Model.Axes.Add(linearAxis1);
                    Oxy.IsVisible = true;

                }
                else {
                    GirthOT.Text = "";
                    SortedList<DateTime, (double, double,double)> thisHistory = ThisPlot.getTrees().ElementAt(pickTree.SelectedIndex).GetHistory();
                    int yearmin = thisHistory.First().Key.Year;
                    int yearmax = thisHistory.Last().Key.Year;
                    List<OxyPlot.DataPoint> ItemsSource = new List<OxyPlot.DataPoint>();
                    HeightOT.Text = yearmin.ToString() + " to " + yearmax.ToString(); 
                    for (int y = yearmin; y <= yearmax; y++)
                    {
                        ItemsSource.Add(new OxyPlot.DataPoint(y, thisHistory.Where(z => z.Key < DateTime.ParseExact((y + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value.Item2));
                    }
                    Oxy.Model = new OxyPlot.PlotModel
                    {
                        Title = ThisPlot.getTrees().ElementAt(pickTree.SelectedIndex).ID.ToString() + " growth over time"
                    };
                    Oxy.MinimumHeightRequest = 1000;
                    var LineSeries = new LineSeries
                    {
                        ItemsSource = ItemsSource

                    };
                    LinearAxis newAxis = new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Key = "Year",
                    };
                    newAxis.IsZoomEnabled = false;
                    newAxis.IsPanEnabled = false;
                    Oxy.Model.Axes.Add(newAxis);
                    var linearAxis1 = new LinearAxis
                    {
                        Position = AxisPosition.Left,
                    };
                    Oxy.Model.Series.Add(LineSeries);
                    linearAxis1.IsZoomEnabled = false;
                    linearAxis1.IsPanEnabled = false;
                    Oxy.Model.Axes.Add(linearAxis1);
                    Oxy.IsVisible = true;



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
                Key = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LogClass"),
                ItemsSource = Lablels,
                Angle = 20
            };
            newAxis.IsZoomEnabled = false;
            newAxis.IsPanEnabled = false;
            Oxy.Model.Axes.Add(newAxis);
            var linearAxis1 = new LinearAxis
            {
                Position = AxisPosition.Left,
            };
            linearAxis1.IsZoomEnabled = false;
            linearAxis1.IsPanEnabled = false;
            Oxy.Model.Axes.Add(linearAxis1);
            Oxy.IsVisible = true;
        }

        private void DetailsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (doubletapTree == (Tree)DetailsList.SelectedItem)
            {
                
                if (Listhadler == 0)
                {
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    pickTree.SelectedIndex = ThisPlot.getTrees().IndexOf(doubletapTree);
                    
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = false;
                    LogList.IsVisible = false;
                    Listhadler = -1;
                    doubletapTree = null;
                }
            }
            else {
               doubletapTree = (Tree)DetailsList.SelectedItem;
            }
        }
        private void LogClassInfoPlot(int bracNo)
        {
            PlotTitle.Text = "";
            Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
            ObservableCollection<DetailsGraph2> Detail = new ObservableCollection<DetailsGraph2>();
            Calculator Calc = new Calculator();
            Calc.SetPrices(ThisPlot.GetRange());
            for (int y = 0; y < ThisPlot.getTrees().Count; y++)
            {
                Tree ThisTree = ThisPlot.getTrees().ElementAt(y);
                SortedList<DateTime, (double, double,double)> Thistory = ThisTree.GetHistory();
                try
                {
                    
                    (double, double,double) measure = Thistory.Where(z => z.Key < DateTime.ParseExact((year + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value;
                    double[,] result = Calc.Calcs(measure.Item1, measure.Item2);
                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        if ((int)result[x, 0] == bracNo)
                        {

                            Detail.Add(new DetailsGraph2 { ID = ThisTree.ID, girth = Math.Round(result[x, 3] * Math.PI, 2), price = Math.Round(result[x, 1], 2), volume = Math.Round(result[x, 2], 2) });
                        }
                    }
                }

                catch { }
            }
            LogList.ItemsSource = Detail;
            LogList.HeightRequest = HeightRequest = (40 * Math.Min(((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count, 5)) + (10 * Math.Min(((ObservableCollection<DetailsGraph2>)LogList.ItemsSource).Count, 5));
            DetailsList.IsVisible = false;
            LogClassList.IsVisible = false;
            LogList.IsVisible = false;
            LogList.IsVisible = true;
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
                    for (int x = 0; x < ((ObservableCollection<DetailsGraph>)LogClassList.ItemsSource).Count; x++) {
                        if (LogClassList.SelectedItem.ToString() == ((ObservableCollection<DetailsGraph>)LogClassList.ItemsSource).ElementAt(x).ToString()) {
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
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
                ((List<Plot>)Application.Current.Properties["Plots"]).Count();
                for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                {
                    pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlot.SelectedIndex = -1;
                }
                pickPlot.Items.Add("Add Plot");
            pickPlot.SelectedIndex = -1;
            ShowGraph.IsVisible = false;
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
        }
}