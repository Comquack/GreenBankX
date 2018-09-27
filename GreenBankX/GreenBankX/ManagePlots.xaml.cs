using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using GreenBankX.Resources;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ManagePlots : ContentPage
	{
        List<Plot> changedPlot;
        int GraphNo = -1;
        int Listhadler = -1;
        bool saveplot = false;
        bool savetree = false;
        DetailsGraph doubletap = null;
        Tree doubletapTree;
        int year = DateTime.Now.Year;
		public ManagePlots ()
		{
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            ToolPricing.Text = "";
            changedPlot = new List<Plot>();
        }
        //activates when index is changed in the plot picker
        public async void SelectPlot()
        { string trees = "";
            Listhadler = 0;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            string IDs = "ID\n";
            string girths = AppResource.ResourceManager.GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetString("Height") + "\n";
            if (pickPlot.SelectedIndex == pickPlot.Items.Count-1&& pickPlot.SelectedIndex>-1) {
                await Navigation.PushAsync(new CreatePlot());
                return;
            }
            if (pickPlot.SelectedIndex > -1 )
            {
                
                   Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() + AppResource.ResourceManager.GetString("Area")+": " + Math.Round(ThisPlot.GetArea(),2)+"km2";

                List <Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {
                    
                    ThisTree = TreeList.ElementAt(x);
                    TreeTails.Add(ThisTree);
                    pickTree.Items.Add(ThisTree.ID.ToString()); 
                }
                pickTree.Items.Add("Add tree");
                DetailsList.IsVisible = true;
                LogClassList.IsVisible = false;
                LogList.IsVisible = false;
                ListOfTree.Text = "";
                GirthOT.Text = "";
                HeightOT.Text = "";
                ToolPricing.Text = "Change Pricing";
                DetailsList.ItemsSource = TreeTails;
                
                PlotTitle.Text = trees;
                pickTree.IsVisible = true;
                Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(10, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(90, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(0, GridUnitType.Auto);
                Oxy.IsVisible = false;
                ShowGraph.IsVisible = true;
                ToolDelete.Text = AppResource.ResourceManager.GetString("DeletePlot");
                ToolDeleteTree.Text = "";
            }
        }
        //activates when index for tree picker is changed
        public void SelectTree() {
            string girths = AppResource.ResourceManager.GetString("Girth") + "\n";
            string heights = AppResource.ResourceManager.GetString("Height") + "\n";
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1 && pickTree.SelectedIndex < pickTree.Items.Count - 1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                GraphNo = ThisTree.GetHistory().Count - 1;
                LatEar();
                Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(20, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(90, GridUnitType.Auto);

            } else if (pickTree.SelectedIndex == pickTree.Items.Count - 1) {
                AddNewTree();
                return;
            }
            else if (pickTree.SelectedIndex == -1)
            {
                SelectPlot();
                return;
            }

            ToolDeleteTree.Text = AppResource.ResourceManager.GetString("DeleteTree");
            AddMes.IsVisible = true;
            
        }

        public async void DelPlot() {
            if (pickPlot.SelectedIndex > -1)
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                    SaveAll.GetInstance().DeletePlot(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName());
                    ((List<Plot>)Application.Current.Properties["Plots"]).RemoveAt(pickPlot.SelectedIndex);
                    ToolDelete.Text = "";
                    ToolDeleteTree.Text = "";
                    ToolPricing.Text = "";
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = "";
                    PlotTitle.Text = "";
                    pickPlot.Items.Clear();
                    pickTree.Items.Clear();
                    pickTree.IsVisible = false;
                    AddTree.IsVisible = false;
                    AddMes.IsVisible = false;
                    ShowGraph.IsVisible = false;
                    doubletap = null;
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = false;
                    for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                    {
                        pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                        
                    }
                    pickPlot.Items.Add("Add Plot");
                    pickPlot.SelectedIndex = pickPlot.SelectedIndex - 1;
                    SelectPlot();
                    SaveAll.GetInstance().SavePlots();
                    SaveAll.GetInstance().SaveTrees2();

                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());
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
                   savetree = true;
                   SaveAll.GetInstance().SaveTrees2();

               });
                await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
            }
       }
        //adds new measurement to selected tree
        public async void AddTreeMes()
        {
            if (pickPlot.SelectedIndex > -1 && pickTree.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"] = pickPlot.SelectedIndex;
                Application.Current.Properties["TCounter"] = pickTree.SelectedIndex;
                Application.Current.Properties["TCounter"] = pickTree.SelectedIndex;
                MessagingCenter.Unsubscribe<AddMesPop>(this, "Append");

                MessagingCenter.Subscribe<AddMesPop>(this, "Append", (sender) =>
                {
                    LatEar();
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = false;
                    LogList.IsVisible = false;
                    SaveAll.GetInstance().SaveTrees2();
                    savetree = true;
                });
                MessagingCenter.Subscribe<AddMesPop>(this, "Alter", (sender) =>
                {
                    GraphNo = (int)Application.Current.Properties["HCounter"];
                    LatEar();
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = false;
                    LogList.IsVisible = false;
                    SaveAll.GetInstance().SaveTrees2();
                    savetree = true;
                });
                await PopupNavigation.Instance.PushAsync(AddMesPop.GetInstance());
            }
        }
        public async void DelTree()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                    string trees;

                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().RemoveAt(pickTree.SelectedIndex);
                    DeleteTree.IsVisible = false;
                    pickTree.Items.Clear();
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() +" "+ AppResource.ResourceManager.GetString("Area") + ": " + Math.Round(ThisPlot.GetArea(), 2) + "km2";
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);          
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    pickTree.Items.Add("Add new tree");
                    AddMes.IsVisible = false;
                    pickTree.SelectedIndex = TreeList.Count - 1;
                    SelectTree();
                    savetree = true;
                    SaveAll.GetInstance().SaveTrees2();
                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());

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
                    double[,] result = Calc.Calcs(girth, high);
                    double total = 0;
                    List<string> Lablels = new List<string>();
                    List<BarItem> ItemsSource = new List<BarItem>();
                    for (int x = -1; x < thisRange.GetBrack().Count; x++)
                    {
                        ItemsSource.Add(new BarItem { CategoryIndex = x+1 });
                        if (x == -1)
                        {
                            Lablels.Add("Too Small");
                        }
                        else if (x == thisRange.GetBrack().Count - 1)
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm" +AppResource.ResourceManager.GetString("OrLarger") + ":\n");
                        }
                        else
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x+1).Key.ToString() + "cm");
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
                    string title = AppResource.ResourceManager.GetString("TreeID") + ": " + ThisTree.ID.ToString() + " " + AppResource.ResourceManager.GetString("Date") + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();

                   
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
                    stuff = AppResource.ResourceManager.GetString("Girth") + ": " + Math.Round(girth, 2).ToString() + "\n" + AppResource.ResourceManager.GetString("Height") + ": " + Math.Round(high, 2).ToString();
                    girthtext = AppResource.ResourceManager.GetString("TotalLogs")+": " + result.GetLength(0) + "\n" + AppResource.ResourceManager.GetString("TotalPrice") + ": " + Math.Round(total, 2) + "k\n";
                    trees = "Tree ID: " + ThisTree.ID.ToString() + "at the date" + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    GirthOT.Text = girthtext;
                    ListOfTree.Text = stuff;
                    PlotTitle.Text = trees;
                    HeightOT.Text = AppResource.ResourceManager.GetString("NumberTrees") + ": " + Math.Round(totVol, 4);

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
                    SortedList<DateTime, (double, double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
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
                        SortedList<DateTime, (double, double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
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
        async public void ChangePricing()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"]= pickPlot.SelectedIndex;
                saveplot = true;
                 await PopupNavigation.Instance.PushAsync(ChangePrice.GetInstance());
                }
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
                PriceRange thisRange = ThisPlot.GetRange();
                Calculator Calc = new Calculator();
                Calc.SetPrices(thisRange);


                double total = 0;
                List<string> Lablels = new List<string>();
                List<BarItem> ItemsSource = new List<BarItem>();
                for (int x = -1; x < thisRange.GetBrack().Count; x++)
                {
                    ItemsSource.Add(new BarItem { CategoryIndex = x+1 });
                    if (x == -1)
                    {
                        Lablels.Add("Too Small");
                    }
                    else if (x == thisRange.GetBrack().Count - 1)
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm"+ AppResource.ResourceManager.GetString("OrLarger") + ":\n"); 
                    }
                    else
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x+1).Key.ToString() + "cm");  
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
                    SortedList<DateTime, (double, double)> Thistory = ThisTree.GetHistory();
                    try {
                        (double, double) measure = Thistory.Where(z => z.Key < DateTime.ParseExact((year + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value;
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
                    Listhadler = 1;
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = "Total Logs for Plot (year:"+year.ToString()+ "):";


                    OxyBar(title, Lablels, ItemsSource);
                    for (int x = 1; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        Detail.Add(new DetailsGraph { label = Lablels.ElementAt(x), volume = Math.Round(vols[x], 4), price = Math.Round(vals[x], 4) });
                    }
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = true;
                    LogList.IsVisible = false;
                    ListOfTree.Text = "";
                    GirthOT.Text = "";
                    HeightOT.Text = "";
                    LogClassList.ItemsSource = Detail;
                    Later.IsVisible = true;
                Earlier.IsVisible = true;
                }
                else
                {
                    SelectPlot();
                    DetailsList.IsVisible = false;
                    Oxy.Model = new OxyPlot.PlotModel();
                    ListOfTree.Text = "Year: \n" + AppResource.ResourceManager.GetString("MeanD") + " \n " + AppResource.ResourceManager.GetString("MeanV") + " \n" + AppResource.ResourceManager.GetString("MeanP") +  "\nTotal Volume: \n Total Value: \n";
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
                        SortedList<DateTime, (double, double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
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
                        SortedList<DateTime, (double, double)> thisHistory = ThisPlot.getTrees().ElementAt(x).GetHistory();
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
                        Title = AppResource.ResourceManager.GetString("MeanG")+" (year: " + year.ToString()+")"
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
                    SortedList<DateTime, (double, double)> thisHistory = ThisPlot.getTrees().ElementAt(pickTree.SelectedIndex).GetHistory();
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
        private void OxyBar(string title, List<string> Lablels, List<BarItem> ItemsSource) {

            Oxy.Model = new OxyPlot.PlotModel
            {
                Title = title
            };
            var barSeries = new BarSeries
            {
                ItemsSource = ItemsSource,
                LabelPlacement = LabelPlacement.Inside,
                LabelFormatString = "{0}"
            };

            Oxy.Model.Series.Add(barSeries);
            CategoryAxis newAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Key = AppResource.ResourceManager.GetString("LogClass"),
                ItemsSource = Lablels
            };
            newAxis.IsZoomEnabled = false;
            newAxis.IsPanEnabled = false;
            Oxy.Model.Axes.Add(newAxis);
            var linearAxis1 = new LinearAxis
            {
                Position = AxisPosition.Bottom,
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
                SortedList<DateTime, (double, double)> Thistory = ThisTree.GetHistory();
                try
                {
                    
                    (double, double) measure = Thistory.Where(z => z.Key < DateTime.ParseExact((year + 1).ToString(), "yyyy", CultureInfo.InvariantCulture)).Last().Value;
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
             GraphNo = -1;
             Listhadler = -1;
             saveplot = false;
             savetree = false;
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
    }

    public class DetailsGraph {
        public double volume { get; set; }
        public double price { get; set; }
        public string label { get; set; }
        public DetailsGraph()
        { }

        public override string ToString()
        {
            return label;
        }
    }

    public class DetailsGraph2
    {
        public double volume { get; set; }
        public double price { get; set; }
        public int ID { get; set; }
        public double girth { get; set; }
        public string label { get; set; }
        public DetailsGraph2()
        { }
    }
}