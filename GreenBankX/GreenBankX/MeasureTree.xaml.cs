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
	public partial class MeasureTree : ContentPage
	{
		public MeasureTree ()
		{
			InitializeComponent ();
		}
        public void RunCalc() {
            Calculator calc = new Calculator();
           double[,] result = calc.Calc(double.Parse(girth.Text), double.Parse(height.Text));
            string resText0 = "Diameter\n";
            string resText1 = "Price\n";
            string resText2 = "Volume\n";
            double[] range = calc.rangeBracket();
            for (int i = 0; i < result.GetLength(0); i++) {
                resText0 = resText0 + Math.Round(result[i, 3], 2) + "cm\n";
                resText1 = resText1 + Math.Round(result[i, 1], 2) + " kip\n"; 
                resText2 = resText2 + Math.Round(result[i, 2], 4) + "m3\n";
            }
            Result0.Text = resText0;
            Result1.Text = resText1;
            Result2.Text = resText2;

        }
	}
}