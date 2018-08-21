using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            InitializeComponent();
            ((List<Plot>)Application.Current.Properties["Plots"]).Count();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
            }
        }
        public void SelectPlot()
        { string trees = "";
            if (pickPlot.SelectedIndex > -1)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                List<Tree> TreeList = ThisPlot.getTrees();
                Tree ThisTree;
                pickTree.Items.Clear();
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString()+"\n";
                    pickTree.Items.Add(ThisTree.ID.ToString());
                }

                ListOfTree.Text = trees;
                pickTree.IsVisible = true;
                //AddTree.IsVisible = true;
               // DeleteTree.IsVisible = false;

                ToolDelete.Text = "Delete Plot";
                ToolDeleteTree.Text = "";
                ToolAddTree.Text = "Add Tree";
                ToolAddMes.Text = "";
            }
        }
        public void SelectTree() {
            string trees = "";
            if (pickTree.SelectedIndex > -1&& pickPlot.SelectedIndex > -1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString() + "\n";
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
                            Lablels.Add("Too Small");
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
                    trees = trees + "Total logs:" + result.GetLength(0) + " Total Price: " + Math.Round(total, 2) + "k\n";
                    for (int x = 0; x < thisRange.GetBrack().Count + 1; x++) {
                        ItemsSource.ElementAt(x).Value = logs[x];
                    }
                    Oxy.Model = new OxyPlot.PlotModel {
                        Title = "Tree ID:"
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
                    trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString() + "\n";
                }

            }

            ToolDeleteTree.Text = "Delete Tree";
            ToolAddMes.Text = "Add Mesurement";
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
                await PopupNavigation.PushAsync(DeleteConfirm.GetInstance());

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
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                });
                await PopupNavigation.PushAsync(AddTreePop.GetInstance());
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
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";

                    }
                    ListOfTree.Text = trees;
                });
                await PopupNavigation.PushAsync(AddMesPop.GetInstance());
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
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                    AddMes.IsVisible = false;
   
                    ToolAddMes.Text = "";
                });
                await PopupNavigation.PushAsync(DeleteConfirm.GetInstance());

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

                trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm): " + girth.ToString() + "Height(m): " + high.ToString() + "\n";
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
                    string title ="Tree ID: " + ThisTree.ID.ToString() + " Date: " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    //string title = GraphNo.ToString() + "/" + (ThisTree.GetHistory().Count - 1).ToString() + " Date: " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
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
                    Titlename.Text = GraphNo.ToString();



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
                trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm): " + girth.ToString() + "Height(m): " + high.ToString() + "\n";
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
                    string title ="Tree ID: " + ThisTree.ID.ToString() + " Date: " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();
                    //string title = GraphNo.ToString() + "/" + (ThisTree.GetHistory().Count - 1).ToString() + " Date: " + ThisTree.GetHistory().ElementAt(GraphNo).Key.ToShortDateString();

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
                    Earlier.IsVisible = true;
                    Titlename.Text = GraphNo.ToString();


                }


            }

        }
        public void SavePlots()
        {
            //Create an instance of ExcelEngine.
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2013.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;

                //Create a workbook with a worksheet
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                //Access first worksheet from the workbook instance.
                //IWorksheet worksheet = workbook.Worksheets[0];

                //Adding text to a cell
                for (int y = 0; y < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); y++)
                {
                    Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(y);
                    workbook.Worksheets.Create(thisPlot.GetName());
                    IWorksheet worksheet = workbook.Worksheets[y + 1];


                    worksheet.SetValue(1, 1, "Name");
                    worksheet.SetValue(2, 1, "Co-ordinates");
                    worksheet.SetValue(1, 2, thisPlot.GetName());
                    worksheet.SetValue(2, 2, thisPlot.GetTag()[0].ToString());
                    worksheet.SetValue(2, 3, thisPlot.GetTag()[1].ToString());
                    worksheet.SetValue(3, 1, "Pricing Name");
                    worksheet.SetValue(3, 2, thisPlot.GetRange().GetName());
                    worksheet.SetValue(3, 3, thisPlot.getTrees().Count.ToString());
                    for (int x = 0; x < thisPlot.getTrees().Count; x++)
                    {
                        worksheet.SetValue(5 + x, 1, thisPlot.getTrees().ElementAt(x).ID.ToString());
                        worksheet.SetValue(5 + x, 2, thisPlot.getTrees().ElementAt(x).Merch.ToString());
                        worksheet.SetValue(5 + x, 3, thisPlot.getTrees().ElementAt(x).GetDia().ToString());
                    }
                }


                workbook.Worksheets[0].Remove();
                //Save the workbook to stream in xlsx format. 
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);

                workbook.Close();

                //Save the stream as a file in the device and invoke it for viewing
                Xamarin.Forms.DependencyService.Get<ISave>().SaveAndView("Plots.xlsx", "application/msexcel", stream);
            }
        }
    }
}