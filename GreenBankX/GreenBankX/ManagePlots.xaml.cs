using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                for (int x = 0; x < TreeList.Count; x++)
                {
                    ThisTree = TreeList.ElementAt(x);
                    trees = trees + "ID: " + ThisTree.ID.ToString() + "Diameter(cm)" + ThisTree.GetDia().ToString() + "Height(m)" + ThisTree.Merch.ToString()+"/n";
                }

                ListOfTree.Text = trees;
            }
        }
    }
}