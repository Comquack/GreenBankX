using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString()+"\n";
                    pickTree.Items.Add(ThisTree.ID.ToString());
                }

                ListOfTree.Text = trees;
                Delete.IsVisible = true;
                DeleteTree.IsVisible = true;
                pickTree.IsVisible = true;
            }
        }
        public void SelectTree() {
            string trees = "";
            if (pickTree.SelectedIndex > -1&& pickPlot.SelectedIndex > -1) {
                Tree ThisTree = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).getTrees().ElementAt(pickTree.SelectedIndex);
                trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString() + "\n";
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange() != null)
                {
                    PriceRange thisRange = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).GetRange();
                    Calculator Calc = new Calculator();
                    Calc.SetPrices(thisRange);
                    double[,] result = Calc.Calcs(ThisTree.GetDia(), ThisTree.Merch);
                    double total = 0;
                    for (int x = 0; x < result.GetLength(0); x++)
                    {
                        total = +result[x, 1];
                    }
                    trees = trees + "Total logs:" + result.GetLength(0) + " Total Price: " + Math.Round(total, 2) + "k";
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
                    trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm): " + ThisTree.GetDia().ToString() + "Height(m): " + ThisTree.Merch.ToString() + "\n";
                }

            }
            ListOfTree.Text = trees;
        }

        public async void DelPlot() {
            if (pickPlot.SelectedIndex > -1)
            {
                MessagingCenter.Subscribe<DeleteConfirm>(this, "Delete", (sender) => {
                    string trees = "";
  
                        ((List<Plot>)Application.Current.Properties["Plots"]).RemoveAt(pickPlot.SelectedIndex);
                        Delete.IsVisible = false;
                    DeleteTree.IsVisible = false;
                    ListOfTree.Text = trees;
                        pickPlot.Items.Clear();
                    pickTree.Items.Clear();
                    pickTree.IsVisible = false;
                    Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex);
                    List<Tree> TreeList = ThisPlot.getTrees();
                    Tree ThisTree;
                    for (int x = 0; x < TreeList.Count; x++)
                    {
                        ThisTree = TreeList.ElementAt(x);
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                });
                await PopupNavigation.PushAsync(new DeleteConfirm());

            }
        }

        public async void DelTree()
        {
            if (pickPlot.SelectedIndex > -1)
            {
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
                        trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString() + "/n";
                        pickTree.Items.Add(ThisTree.ID.ToString());
                    }
                });
                await PopupNavigation.PushAsync(new DeleteConfirm());

            }
        }

    }
}