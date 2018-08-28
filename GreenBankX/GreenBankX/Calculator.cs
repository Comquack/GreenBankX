using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenBankX
{
    class Calculator
    {
        private static readonly double A = 0.0022;
        private static readonly double B = -0.0202;
        private static readonly double C = 0.5441;
        private static double LOGLEN = 2.3;
        private static double BHEIGHT = 1.2;
        private static double STUMP = .15;
        private static double BARK = .9;
        private PriceRange prices;
        public Calculator() {
        }
        public void SetPrices(PriceRange newprices) {
            prices = newprices;
        }
        public PriceRange GetPrices()
        {
            return prices;
        }


        public double[,] Calcs(double breastGirth, double merchHeight)
        {
            LOGLEN = prices.GetLength();
            SortedList<double, double> brack = prices.GetBrack();
            int noLogs = (int)Math.Floor((merchHeight) / LOGLEN);
            double[,] totals = new double[noLogs, 5];
            double taper = 0;
            double rH = 0;
            double rL = 0;
            double breastDiameter = breastGirth / (Math.PI);
            double value = 0;
            double sizeClass = 0;
            for (int i = 0; i < noLogs; i++)
            {
                if (i == 0)
                {
                    taper = A * Math.Pow((breastDiameter - 2 * BARK), 2) + B * (breastDiameter - 2 * BARK) + C;
                    rH = ((breastDiameter - 2 * BARK) - (LOGLEN - BHEIGHT + STUMP) * taper) / 2;
                }
                else
                {
                    taper = A * Math.Pow(2 * rL, 2) + B * 2 * rL + C;
                    rH = (2 * rL - LOGLEN * taper) / 2;
                }
                value = 0;
                sizeClass = -1;
                for (int a = 0; a < brack.Count; a++)
                {
                    
                    if (2 * rH > brack.ElementAt(a).Key)
                    {
                        value = brack.ElementAt(a).Value;
                        sizeClass = a;

                    }
                }
                totals[i, 0] = sizeClass;
               // totals[i, 1] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2) * value;
               // totals[i, 2] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2);
               totals[i, 1] = (0.33333)*LOGLEN * Math.PI *( Math.Pow(rH / 100, 2)+(rH / 100)*(rL / 100) + Math.Pow(rL / 100, 2)) * value;
                totals[i, 2] = (0.33333) * LOGLEN * Math.PI * (Math.Pow(rH / 100, 2) + (rH / 100) * (rL / 100) + Math.Pow(rL / 100, 2));
                totals[i, 3] = 2 * rH;
                totals[i, 4] = taper;

                rL = rH;
            }
            return totals;
        }

    }
}
