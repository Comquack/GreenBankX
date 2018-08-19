using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Delete.IsVisible = true;
                DeleteTree.IsVisible = true;
                pickTree.IsVisible = true;
                AddTree.IsVisible = true;
                DeleteTree.IsVisible = false;
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
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).ToString() + "cm or larger");
                        }
                        else
                        {
                            Lablels.Add(thisRange.GetBrack().ElementAt(x).ToString() + "cm - " + thisRange.GetBrack().ElementAt(x).ToString() + "cm");
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
            DeleteTree.IsVisible = true;
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
                        Delete.IsVisible = false;
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = trees;
                        pickPlot.Items.Clear();
                    pickTree.Items.Clear();
                    pickTree.IsVisible = false;
                    AddTree.IsVisible = false;
                    AddMes.IsVisible = false;
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "\n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                });
                await PopupNavigation.PushAsync(new DeleteConfirm());

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
                await PopupNavigation.PushAsync(new AddTreePop());
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
                    pickTree.Items.Clear();
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Girth(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                    ListOfTree.Text = trees;
                });
                await PopupNavigation.PushAsync(new AddMesPop());
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
                });
                await PopupNavigation.PushAsync(new DeleteConfirm());

            }
        }

    }
}