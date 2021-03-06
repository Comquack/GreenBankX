﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenBankX
{
    class Calculator
    {

        private static readonly double A2 = 0.59256;
        private static readonly double B2 = 0.63308;
        private static double LOGLEN = 2.3;
        private static readonly double BHEIGHT = 1.3;
        private static readonly double STUMP = .15;
        private double BARK = 1.3;
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


        public double[,] Calcs(double breastGirth, double TotalHeight)
        {   
            LOGLEN = prices.GetLength();
            SortedList<double, double> brack = prices.GetBrack();
            int noLogs = (int)Math.Floor((TotalHeight) / LOGLEN);
            double[,] totals = new double[noLogs, 5];
            double taper = 0;
            double rH = 0;
            double rL = 0;
            double breastDiameter = breastGirth / (Math.PI);
            BARK = 0.0266 * breastDiameter + 0.2399;
            double value = 0;
            double height = 0;
            double sizeClass = 0;
            height = STUMP;
            for (int i = 0; i < noLogs; i++)
            {
                height += LOGLEN;
                    rH = (TotalHeight - height) * ((A2 * TotalHeight * (Math.Pow(B2, 2)) * (BHEIGHT - height)) / ((1 + B2 * (height)) * (1 + B2 * BHEIGHT) * (1 + B2 * TotalHeight)) + ((0.77715 / TotalHeight + 0.01239 * ((breastDiameter - BARK) / 10) + -0.0027653 * Math.Pow(((breastDiameter - BARK) / 10), 2)) * (height - BHEIGHT)) + ((breastDiameter - BARK) / (TotalHeight - BHEIGHT)));
                    rH = rH / 2;
                
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
                totals[i, 1] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2) * value;
                totals[i, 2] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2);
                totals[i, 3] = 2 * rH;
                totals[i, 4] = taper;

                rL = rH;
            }
            return totals;
        }
        public double[,] Calcs(double breastGirth, double TotalHeight, double merchHeight)
        {
            if (merchHeight == -1) {
                return Calcs(breastGirth, TotalHeight);
            }
            LOGLEN = prices.GetLength();
            SortedList<double, double> brack = prices.GetBrack();
            int noLogs = (int)Math.Floor((TotalHeight) / LOGLEN);
            double[,] totals = new double[noLogs, 5];
            double rH = 0;
            double rL = 0;
            
            double breastDiameter = breastGirth / (Math.PI);
            BARK = 0.0266 * breastDiameter + 0.2399;
            double value = 0;
            double oversize = 0;
            double height = 0;
            double sizeClass = 0;
            height = STUMP;
            for (int i = 0; i < noLogs; i++)
            {
                oversize = 1;
                height += LOGLEN;
                rH = (TotalHeight - height) * ((A2 * TotalHeight * (Math.Pow(B2, 2)) * (BHEIGHT - height)) / ((1 + B2 * (height)) * (1 + B2 * BHEIGHT) * (1 + B2 * TotalHeight)) + ((0.77715 / TotalHeight + 0.01239 * ((breastDiameter - 2*BARK) / 10) + -0.0027653 * Math.Pow(((breastDiameter - 2 * BARK) / 10), 2)) * (height - BHEIGHT)) + ((breastDiameter - 2 * BARK) / (TotalHeight - BHEIGHT)));
                rH = rH / 2;

                value = 0;
                sizeClass = -1;
                for (int a = 0; a < brack.Count; a++)
                {
                    if (height > merchHeight) {
                        oversize = 0;
                    }
                    if (2 * rH > brack.ElementAt(a).Key)
                    {
                        value = brack.ElementAt(a).Value*oversize;
                        sizeClass = a;
                    }
                }
                totals[i, 0] = sizeClass;
                totals[i, 1] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2) * value;
                totals[i, 2] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2);
                totals[i, 3] = 2 * rH;
                totals[i, 4] = BARK;

                rL = rH;
            }
            return totals;
        }

    }
}
