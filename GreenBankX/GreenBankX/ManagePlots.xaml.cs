using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        bool saveplot = false;
        bool savetree = false;
        string doubletap = "";
        int year = DateTime.Now.Year;
		public ManagePlots ()
		{


            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            changedPlot = new List<Plot>();
            ((List<Plot>)Application.Current.Properties["Plots"]).Count();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());   
            }
            Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(10, GridUnitType.Auto);
            Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(90, GridUnitType.Auto);
            Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(0, GridUnitType.Star);
            ShowGraph.IsVisible = false;
        }
        //activates when index is changed in the plot picker
        public void SelectPlot()
        { string trees = "";
            List<string> Detail = new List<string>();
            string IDs = "ID\n";
            string girths = AppResource.ResourceManager.GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetString("Height") + "\n";
            if (pickPlot.SelectedIndex > -1)
            {
                
                   Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() + AppResource.ResourceManager.GetString("Area")+": " + Math.Round(ThisPlot.GetArea(),2)+"km2";

                List <Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    IDs += ThisTree.ID.ToString() + "\n";
                    girths += Math.Round(ThisTree.GetDia(),2).ToString() + "\n";
                    heights += Math.Round(ThisTree.Merch,2).ToString() + "\n";
                    pickTree.Items.Add(ThisTree.ID.ToString());
                    Detail.Add("ID: " + ThisTree.ID.ToString() + "\t" + AppResource.ResourceManager.GetString("Girth") + ": " + Math.Round(ThisTree.GetDia(), 2).ToString() + "cm\t" + AppResource.ResourceManager.GetString("Height") + ": " + Math.Round(ThisTree.Merch, 2).ToString()+"m");
                }
                DetailsList.IsVisible = true;
                // ListOfTree.Text = IDs;
                //GirthOT.Text = girths;
                //HeightOT.Text = heights;

                DetailsList.ItemsSource = Detail.ToArray();
                PlotTitle.Text = trees;
                pickTree.IsVisible = true;
                Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(10, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(90, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(0, GridUnitType.Auto);
                Oxy.IsVisible = false;
                ShowGraph.IsVisible = true;
                ToolDelete.Text = AppResource.ResourceManager.GetString("DeletePlot");
                ToolDeleteTree.Text = "";
                ToolAddTree.Text = AppResource.ResourceManager.GetString("AddTree");
                ToolAddMes.Text = "";
            }
        }
        //activates when index for tree picker is changed
        public void SelectTree() {
            string trees = "";
            string girths = AppResource.ResourceManager.GetString("Girth") + "\n";
            string heights = AppResource.ResourceManager.GetString("Height") + "\n";
            if (pickTree.SelectedIndex > -1&& pickPlot.SelectedIndex > -1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                    GraphNo = ThisTree.GetHistory().Count-1;
                LatEar();
                    Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(20, GridUnitType.Auto);
                    Graphgrid.RowDefinitions.ElementAt(2).Height = new GridLength(90, GridUnitType.Auto);

            }
            else if (pickTree.SelectedIndex == -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                List<Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    trees += "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "\n";
                }
                ListOfTree.Text = trees;
            }

            ToolDeleteTree.Text = AppResource.ResourceManager.GetString("DeleteTree");
            ToolAddMes.Text = AppResource.ResourceManager.GetString("AddMeasurement");
            AddMes.IsVisible = true;
            
        }

        public async void DelPlot() {
            if (pickPlot.SelectedIndex > -1)
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                    string trees = "";
                    SaveAll.GetInstance().DeletePlot(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetName());
                    ((List<Plot>)Application.Current.Properties["Plots"]).RemoveAt(pickPlot.SelectedIndex);
                    ToolDelete.Text = "";
                    ToolDeleteTree.Text = "";
                    ToolAddTree.Text = "";
                    ToolAddMes.Text = "";
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = trees;
                        pickPlot.Items.Clear();
                    pickTree.Items.Clear();
                    pickTree.IsVisible = false;
                    AddTree.IsVisible = false;
                    AddMes.IsVisible = false;
                    ShowGraph.IsVisible = false;
                    for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                    {
                        pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                    }
                    ListOfTree.Text = trees;
                    pickPlot.SelectedIndex = pickPlot.SelectedIndex - 1;
                    SelectPlot();


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
                   string IDs = "ID\n";
                   string girths = AppResource.ResourceManager.GetString("Girth") + "\n";
                   string heights = AppResource.ResourceManager.GetString("Height") + "\n";
                   string trees = "";
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                   trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() + AppResource.ResourceManager.GetString("Area") + ": " + Math.Round(ThisPlot.GetArea(), 2) + "km2";
                   pickTree.Items.Clear();
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        IDs += ThisTree.ID.ToString() + "\n";
                        girths +=ThisTree.GetDia().ToString() + "\n";
                        heights += ThisTree.Merch.ToString() + "\n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                   PlotTitle.Text = trees;
                    ListOfTree.Text = IDs;
                    GirthOT.Text = girths;
                    HeightOT.Text = heights;
                    pickTree.SelectedIndex = ThisPlot.getTrees().Count - 1;
                    SelectTree();
                   savetree = true;
                   changedPlot.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex));

               });
                await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
            }
       }
        //adds new measurement to selected tree
        public async void AddTreeMes()
        {
            string IDs = "ID\n";
            string girths = AppResource.ResourceManager.GetString("Girth") + "\n";
            string heights = AppResource.ResourceManager.GetString("Height") + "\n";
            if (pickPlot.SelectedIndex > -1 && pickTree.SelectedIndex > -1)
            {
                Application.Current.Properties["Counter"] = pickPlot.SelectedIndex;
                Application.Current.Properties["TCounter"] = pickTree.SelectedIndex;
                MessagingCenter.Unsubscribe<AddMesPop>(this, "Append");

                MessagingCenter.Subscribe<AddMesPop>(this, "Append", (sender) =>
                {
                    string trees = "";
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;

                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        IDs +=ThisTree.ID.ToString() + "\n";
                        girths +=ThisTree.GetDia().ToString() + "\n";
                        heights +=ThisTree.Merch.ToString() + "\n"; 

                    }
                    PlotTitle.Text = trees;
                    ListOfTree.Text = IDs;
                    GirthOT.Text = girths;
                    HeightOT.Text = heights;
                    savetree = true;
                    changedPlot.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex));
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
                    string IDs = "ID\n";
                    string girths = AppResource.ResourceManager.GetString("Girth") + "\n";
                    string heights = AppResource.ResourceManager.GetString("Height") + "\n";
                    string trees;

                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().RemoveAt(pickTree.SelectedIndex);
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = "";
                    pickTree.Items.Clear();
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() + AppResource.ResourceManager.GetString("Area") + ": " + Math.Round(ThisPlot.GetArea(), 2) + "km2";
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);          
                        IDs = IDs + ThisTree.ID.ToString() + "\n";
                        girths += ThisTree.GetDia().ToString() + "\n";
                        heights +=ThisTree.Merch.ToString() + "\n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = IDs;
                    GirthOT.Text = girths;
                    HeightOT.Text = heights;
                    AddMes.IsVisible = false;
   
                    ToolAddMes.Text = "";
                    changedPlot.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex));
                    savetree = true;
                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());

            }
        }

        private void LatEar()
        {
            string trees = "";
            string girthtext = "";
            string stuff = "";
            double totVol = 0;
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);

                double girth = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item1;
                double high = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item2;


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
                        ItemsSource.Add(new BarItem { CategoryIndex = x });
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
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm");
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

                    OxyBar(title, Lablels, ItemsSource);
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
                    trees = "Tree ID:" + ThisTree.ID.ToString() + "at the date" + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    GirthOT.Text = girthtext;
                    ListOfTree.Text = stuff;
                    PlotTitle.Text = trees;
                    HeightOT.Text = "Total Volume: " + Math.Round(totVol, 4);

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
                    PlotTitle.Text = GraphNo.ToString();
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
            }

        }
        public void Save()
        {
            if (saveplot)
            {
                SaveAll.GetInstance().SavePlots();
            }
            if (savetree)
            {
                SaveAll.GetInstance().SaveTrees(0, changedPlot);
            }
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
        public void ChangeTree()
        {

        }

        private void ShowGraphpick()
        {
            if (ShowGraph.SelectedIndex == 0) {
                SelectPlot();
                Earlier.IsVisible = false;
                Later.IsVisible = false;
            }
            if (ShowGraph.SelectedIndex == 1 || ShowGraph.SelectedIndex == 2)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                PriceRange thisRange = ThisPlot.GetRange();
                Calculator Calc = new Calculator();
                Calc.SetPrices(thisRange);


                double total = 0;
                List<string> Lablels = new List<string>();
                List<BarItem> ItemsSource = new List<BarItem>();
                string logclasses = AppResource.ResourceManager.GetString("LogClass")+"\n";
                String volumes = "total Volume\n";
                String worth = AppResource.ResourceManager.GetString("TotalPrice") + "\n";
                for (int x = -1; x < thisRange.GetBrack().Count; x++)
                {
                    ItemsSource.Add(new BarItem { CategoryIndex = x });
                    if (x == -1)
                    {
                        Lablels.Add("Too Small");
                        //logclasses += "Too Small:\n";
                    }
                    else if (x == thisRange.GetBrack().Count - 1)
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm"+ AppResource.ResourceManager.GetString("OrLarger") + ":\n");
                        logclasses += thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm" + AppResource.ResourceManager.GetString("OrLarger")+":\n";
                    }
                    else
                    {
                        Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm");
                        logclasses += thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm:\n";
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

                if (ShowGraph.SelectedIndex == 1)
                {

                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = "Total Logs for Plot (year:"+year.ToString()+ "):";


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
                    for (int x = 1; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        volumes += Math.Round(vols[x], 4).ToString() + "m3\n";
                        worth += Math.Round(vals[x], 4) + "k\n";

                    }
                    ListOfTree.Text = logclasses;
                    GirthOT.Text = volumes;
                    HeightOT.Text = worth;
                }
                else
                {
                    Oxy.Model = new OxyPlot.PlotModel();
                    ListOfTree.Text = "Year: \nMean Diameter : \n Mean Volume: \n Mean Value: \n";
                    HeightOT.Text = "";
                    GirthOT.Text = year.ToString()+"\n"+Math.Round((totalDia / (double)count), 4).ToString() + "\n" + Math.Round((totalvol / (double)count), 4).ToString() + "\n" + Math.Round((total / (double)count), 4).ToString() + "\n";
                }
                if (GraphNo <= 0)
                {
                    Later.IsVisible = false;
                }
                Later.IsVisible = true;
                Earlier.IsVisible = true;

            }
            else if (ShowGraph.SelectedIndex == 3) {
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
                    //SortedList<(int,int), (double, double)> data= new SortedList<(int,int), (double, double)>();
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
                            //GirthOT.Text += y.ToString();
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
                        Title = "Mean growth over time (year: "+ year.ToString()+")"
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
            if (doubletap == DetailsList.SelectedItem.ToString())
            {
                doubletap = "";
               // PlotTitle.Text = Array.IndexOf((Array)DetailsList.ItemsSource, DetailsList.SelectedItem).ToString();
                pickTree.SelectedIndex = Array.IndexOf((Array)DetailsList.ItemsSource, DetailsList.SelectedItem);
                DetailsList.IsVisible = false;
            }
            else {
                doubletap = DetailsList.SelectedItem.ToString();
                PlotTitle.Text = doubletap;
            }
        }
    }
}