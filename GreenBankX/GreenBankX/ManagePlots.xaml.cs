using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBankX.Resources;
using OxyPlot.Axes;
using OxyPlot.Series;
using Rg.Plugins.Popup.Services;
using Syncfusion.XlsIO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ManagePlots : ContentPage
	{
        int GraphNo = -1;
		public ManagePlots ()
		{
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
            InitializeComponent();
            ((List<Plot>)Application.Current.Properties["Plots"]).Count();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
            Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(90, GridUnitType.Star);
            Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(0, GridUnitType.Star);
        }
        public void SelectPlot()
        { string trees = "";
            if (pickPlot.SelectedIndex > -1)
            {
                
                   Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                trees = AppResource.ResourceManager.GetString("Name") + ": " + ThisPlot.GetName() + "\n"+ AppResource.ResourceManager.GetString("Area")+": " + ThisPlot.GetArea()+"km2\n";
                List <Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("DeletePlot")+": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString()+"\n";
                    pickTree.Items.Add(ThisTree.ID.ToString());
                }

                ListOfTree.Text = trees;
                pickTree.IsVisible = true;
                Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(90, GridUnitType.Auto);
                Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(0, GridUnitType.Star);
                Oxy.IsVisible = false;
                ToolDelete.Text = AppResource.ResourceManager.GetString("DeletePlot");
                ToolDeleteTree.Text = "";
                ToolAddTree.Text = AppResource.ResourceManager.GetString("AddTree");
                ToolAddMes.Text = "";
            }
        }
        public void SelectTree() {
            string trees = "";
            if (pickTree.SelectedIndex > -1&& pickPlot.SelectedIndex > -1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "\n";
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange() != null)
                {
                    PriceRange thisRange = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange();
                    Calculator Calc = new Calculator();
                    Calc.SetPrices(thisRange);
                    double[,] result = Calc.Calcs(ThisTree.GetDia(), ThisTree.Merch);
                    double total = 0;
                    List<string> Lablels = new List<string>();
                    List<BarItem> ItemsSource = new List<BarItem>();
                   for (int x = -1; x< thisRange.GetBrack().Count; x++) {
                        ItemsSource.Add(new BarItem { CategoryIndex = x });
                        if (x == -1)
                        {
                            Lablels.Add(AppResource.ResourceManager.GetString("TooSmall"));
                        }
                        else if (x == thisRange.GetBrack().Count - 1)
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm or larger");
                        }
                        else
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm - " + thisRange.GetBrack().ElementAt(x+1).Key.ToString() + "cm");
                        }
                    }
                    int[] logs = new int[thisRange.GetBrack().Count + 1];
                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        logs[(int)result[x, 0]+1]++;
                        total = +result[x, 1];      
                     }
                    trees = trees + AppResource.ResourceManager.GetString("TotalLogs") + ": " + result.GetLength(0) + AppResource.ResourceManager.GetString("TotalPrice")+": " + Math.Round(total, 2) + "k\n";
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++) {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    Oxy.Model = new OxyPlot.PlotModel {
                        Title = AppResource.ResourceManager.GetString("TreeID") + ": "
                    };
                    var barSeries = new BarSeries
                    {
                        ItemsSource = ItemsSource,
                        LabelPlacement = LabelPlacement.Inside,
                        LabelFormatString = "{0}"
                    };

                    Oxy.Model.Series.Add(barSeries);
                    Oxy.Model.Axes.Add(new CategoryAxis
                    {
                        Position = AxisPosition.Left,
                        Key = "Log Classes",
                        ItemsSource = Lablels
                    });
                    Oxy.IsVisible = true;
                    GraphNo = 0;
                    Later.IsVisible = false;
                    Earlier.IsVisible = true;
                    Graphgrid.RowDefinitions.ElementAt(0).Height = new GridLength(20, GridUnitType.Star);
                    Graphgrid.RowDefinitions.ElementAt(1).Height = new GridLength(70, GridUnitType.Star);


                }
                

            }
            else if (pickTree.SelectedIndex == -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                List<Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "\n";
                }

            }

            ToolDeleteTree.Text = AppResource.ResourceManager.GetString("DeleteTree");
            ToolAddMes.Text = AppResource.ResourceManager.GetString("AddMeasurement");
            // DeleteTree.IsVisible = true;
            AddMes.IsVisible = true;
            ListOfTree.Text = trees;
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
                    for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                    {
                        pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
                    }
                    ListOfTree.Text = trees;
                    
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
                    string trees = "";
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    pickTree.Items.Clear();
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "\n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                    pickTree.SelectedIndex = ThisPlot.getTrees().Count - 1;
                    SelectTree();
                });
                await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
            }
       }
        public async void AddTreeMes()
        {
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
                        trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "/n";

                    }
                    ListOfTree.Text = trees;
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
                    string trees = "";

                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().RemoveAt(pickTree.SelectedIndex);
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = trees;
                    pickTree.Items.Clear();
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + ThisTree.GetDia().ToString() + AppResource.ResourceManager.GetString("Height") + ": " + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                    AddMes.IsVisible = false;
   
                    ToolAddMes.Text = "";
                });
                await PopupNavigation.Instance.PushAsync(DeleteConfirm.GetInstance());

            }
        }

        private void Earlier_Clicked(object sender, EventArgs e)
        {
            string trees = "";
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                if (GraphNo < ThisTree.GetHistory().Count-1) {
                    GraphNo++;
                }
                double girth = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item1;
                double high = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item2;

                trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + girth.ToString() + AppResource.ResourceManager.GetString("Height") + ": " + high.ToString() + "\n";
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
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm or larger");
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
                    }
                    trees = trees + "Total logs:" + result.GetLength(0) + " Total Price: " + Math.Round(total, 2) + "k\n";
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = AppResource.ResourceManager.GetString("TreeID") + ": " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Date") + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                   
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
                    Oxy.Model.Axes.Add(new CategoryAxis
                    {
                        Position = AxisPosition.Left,
                        Key = "Log Classes",
                        ItemsSource = Lablels
                    });
                    Oxy.IsVisible = true;
                    Later.IsVisible = true;
                    if (GraphNo >= ThisTree.GetHistory().Count - 1)
                    {
                        Earlier.IsVisible = false;
                    }
                    if (GraphNo <= 0)
                    {
                        Later.IsVisible = false;
                    }

                }


            }
        }

        private void Later_Clicked(object sender, EventArgs e)
        {
            string trees = "";
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1)
            {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                if (GraphNo > 0)
                {
                    GraphNo--;
                }
                double girth = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item1;
                double high = ThisTree.GetHistory().ElementAt(GraphNo).Value.Item2;
                trees = trees + "ID: " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Girth") + ": " + girth.ToString() + AppResource.ResourceManager.GetString("Height") + ": " + high.ToString() + "\n";
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
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).Key.ToString() + "cm or larger");
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
                    }
                    trees = trees + "Total logs:" + result.GetLength(0) + " Total Price: " + Math.Round(total, 2) + "k\n";
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++)
                    {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    string title = AppResource.ResourceManager.GetString("TreeID") + ": " + ThisTree.ID.ToString() + AppResource.ResourceManager.GetString("Date") + ": " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    

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
                    Oxy.Model.Axes.Add(new CategoryAxis
                    {
                        Position = AxisPosition.Left,
                        Key = "Log Classes",
                        ItemsSource = Lablels
                    });
                    Oxy.IsVisible = true;
                    if (GraphNo <= 0)
                    {
                        Later.IsVisible = false;
                    }
                    Later.IsVisible = true;
                    Earlier.IsVisible = true;
                    if (GraphNo >= ThisTree.GetHistory().Count - 1)
                    {
                        Earlier.IsVisible = false;
                    }

                }


            }

        }
        public void Save()
        {
            SaveAll.GetInstance().SavePlots();
            SaveAll.GetInstance().SaveTrees();
        }
        public void ChangePrice() {

        }
        public void ChangeTree()
        {

        }
        public void PlotOverTime()
        {
            if (pickTree.SelectedIndex > -1 && pickPlot.SelectedIndex > -1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
            }
        }
    }
}