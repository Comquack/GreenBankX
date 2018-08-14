﻿using System;
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
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
            }
             for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                pickPlot.Items.Add(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName());
             }  
		}
        public void RunCalc() {
            Calculator calc = new Calculator();
            if (pickPrice.SelectedIndex > -1)
            {
                calc.SetPrices(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                double[,] result = calc.Calcs(double.Parse(girth.Text), double.Parse(height.Text));
                string resText0 = "Log Size\n";
                string resText1 = "Price\n";
                string resText2 = "Volume\n";
                SortedList<double, double> brack = calc.GetPrices().GetBrack();
                double total = 0;
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    if (result[i, 0] == 0)
                    {
                        resText0 = resText0 + "Log is too small\n";
                    }
                    else if (result[i, 0]==brack.Count-1) {
                        resText0 = resText0 + brack.ElementAt((int)result[i, 0]) + "cm or larger\n";
                    }
                    else {
                        resText0 = resText0 + brack.ElementAt((int)result[i, 0]).Key + "-" + brack.ElementAt((int)result[i, 0]+1).Key + " cm\n";
                    }
                    resText1 = resText1 + Math.Round(result[i, 1], 2) + " kip\n";
                    resText2 = resText2 + Math.Round(result[i, 2], 4) + "m3\n";
                }
                Result0.Text = resText0;
                Result1.Text = resText1;
                Result2.Text = resText2;
            }
        }

        public void RunAdd() {
            if (girth.Text != null && height.Text != null && pickPlot.SelectedIndex != -1) {
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(pickPlot.SelectedIndex).AddTree(new Tree(float.Parse(girth.Text), float.Parse(height.Text),0));
            }
        }
	}
}