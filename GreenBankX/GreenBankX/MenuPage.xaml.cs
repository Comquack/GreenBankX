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
	public partial class MenuPage : ContentPage
	{
        Tree tree1;
        Plot plot1;
        Calculator calc = new Calculator();
        double[,] result;
        public MenuPage ()
		{
			InitializeComponent ();
            tree1 = new Tree(100, 10, 1);
            plot1 = new Plot("Ethl");
            plot1.AddTree(tree1);
            result = calc.Calc(100, 10);
        }
        async void OpenMeasure(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MeasureTree());
        }
        async void OpenMap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePlot());
        }
        async void OpenManager(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManagePlots());
        }
    }

}