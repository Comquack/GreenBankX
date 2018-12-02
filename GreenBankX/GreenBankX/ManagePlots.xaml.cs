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
	public partial class ManagePlots : ContentPage
	{
        List<Plot> changedPlot;
        int GraphNo = -1;
        int Listhadler = -1;
        DetailsGraph doubletap = null;
        Tree doubletapTree;
        ObservableCollection<PlotContainer> plotty = new ObservableCollection<PlotContainer>();
        PlotContainer doubletap2;
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
            PlotList.IsVisible = false;
            Listhadler = 0;
            ObservableCollection<Tree> TreeTails = new ObservableCollection<Tree>();
            string girths = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth")+"\n";
            string heights = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Height") + "\n";
            if (pickPlot.SelectedIndex == pickPlot.Items.Count-1&& pickPlot.SelectedIndex>-1) {
                await Navigation.PushAsync(new CreatePlot());
                return;
            }
            if (pickPlot.SelectedIndex > -1)
            {
                ToolEdit.Text = "Edit Plot";
                AddMes.IsVisible = false;
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName();
                if (ThisPlot.Owner != null && ThisPlot.Owner != "")
                {
                    trees += "Owner: " + ThisPlot.Owner + "\n";
                }
                if (ThisPlot.NearestTown != null && ThisPlot.NearestTown != "")
                {
                    trees += "Location: " + ThisPlot.NearestTown + "\n";
                }
                if (ThisPlot.Describe != null && ThisPlot.Describe != "")
                {
                    trees += "Comments: " + ThisPlot.Describe + "\n";
                }
                List<Tree> TreeList = ThisPlot.getTrees();
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

                ListOfTree.Text = "";
                GirthOT.Text = "";
                HeightOT.Text = "";
                ToolPricing.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CPrice");
                DetailsList.ItemsSource = TreeTails;
                DetailsList.HeightRequest = (40 * Math.Min(TreeTails.Count, 5)) + (10 * Math.Min(TreeTails.Count, 5)) + 60;
                PlotTitle.Text = trees;
                pickTree.IsVisible = true;
                ToolDelete.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DeletePlot");
                ToolDeleteTree.Text = "";
            }
            else {
                OnAppearing();
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

            ToolDeleteTree.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DeleteTree");
            AddMes.IsVisible = true;
            
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
                    SaveAll.GetInstance().SaveTrees2();
                });
                MessagingCenter.Subscribe<AddMesPop>(this, "Alter", (sender) =>
                {
                    GraphNo = (int)Application.Current.Properties["HCounter"];
                    LatEar();
                    DetailsList.IsVisible = false;
                    LogClassList.IsVisible = false;
                    SaveAll.GetInstance().SaveTrees2();
                });
                await PopupNavigation.Instance.PushAsync(AddMesPop.GetInstance());
            }
        }
        public async void DelTree()
        {
            if (pickPlot.SelectedIndex > -1&& ToolDelete.Text!="")
            {
                MessagingCenter.Unsubscribe<DeleteConfirm>(this, "Delete");
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                    string trees;
                   int selec = ((ObservableCollection<Tree>)DetailsList.ItemsSource).IndexOf((Tree)DetailsList.SelectedItem);
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().RemoveAt(selec);
                    DeleteTree.IsVisible = false;
                    pickTree.Items.Clear();
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    trees = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Name") + ": " + ThisPlot.GetName() +" "+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Area") + ": " + Math.Round(ThisPlot.GetArea(), 2) + "km2";
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);          
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    pickTree.Items.Add("Add new tree");
                    AddMes.IsVisible = false;
                    int storen = pickPlot.SelectedIndex;
                    pickPlot.SelectedIndex = -1;
                    pickPlot.SelectedIndex = storen;
                    SelectTree();
                    SaveAll.GetInstance().SaveTrees2();
                    ToolDelete.Text = "";
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
                 await PopupNavigation.Instance.PushAsync(ChangePrice.GetInstance());
                }
        }
        //data displaed changes when selection is changed
 
  

        private void DetailsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

                    ToolDeleteTree.Text = "Delete Tree";
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
DetailsList.IsVisible = false;
            LogClassList.IsVisible = false;
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
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                plotty.Add(new PlotContainer(thisPlot.GetName(), thisPlot.getTrees().Count, thisPlot.YearPlanted));
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                pickPlot.SelectedIndex = -1;
            }
            PlotList.ItemsSource = plotty;
            pickPlot.Items.Add("Add Plot");
            pickPlot.SelectedIndex = -1;
            PlotList.IsVisible = true;
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
        }



        public async void EditPlot()
        {
            if (pickPlot.SelectedIndex > -1)
            {
                Application.Current.Properties["ThisPlot"] = pickPlot.SelectedIndex;
                Application.Current.Properties["ThisLocation"] = null;
                MessagingCenter.Subscribe<PlotPopupEdit>(this, "Edit", (sender) =>
                {
                    SelectPlot();
                    SaveAll.GetInstance().SavePlots();
                });
                await PopupNavigation.Instance.PushAsync(PlotPopupEdit.GetInstance());
            }
        }
        private void PlotList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (doubletap2 == PlotList.SelectedItem)
            {
                pickPlot.SelectedIndex = plotty.IndexOf((PlotContainer)PlotList.SelectedItem);
            }
            else
            {
                doubletap2 = (PlotContainer)PlotList.SelectedItem;
            }
        }
        protected override bool OnBackButtonPressed()
        {
            if (DetailsList.IsVisible)
            {
                OnAppearing();
                PlotTitle.IsVisible = false;
                return true;
            }
            else
            {
                base.OnBackButtonPressed();
                return true;
            }
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