using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBankX
{
    class Calculator
    {
        private static double A = 0.0022;
        private static double B = -0.0202;
        private static double C = 0.5441;
        private double[] rangeValue = new double[]{ 1000000, 1500000, 1800000, 2000000, 2500000};
        private double[] rangeSizes = new double[]{ 12, 16, 19, 21, 30 };
        private int rangeNo = 5;
        private static double LOGLEN = 2.3;
        private static double BHEIGHT = 1.2;
        private static double STUMP = .15;
        private static double BARK = .9;
        public Calculator() {
        }

        public double[,] Calc(double breastGirth, double merchHeight)
        {
            int noLogs = (int)Math.Floor((merchHeight) / LOGLEN);
            double[,] totals = new double[noLogs,5];
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
                for (int a = 0; a < rangeNo; a++)
                {
                    if (2 * rH > rangeSizes[a])
                    {
                        value = rangeValue[a];
                        sizeClass = a;

                    }
                }
                totals[i,0] = sizeClass;
                totals[i,1] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2) * value;
                totals[i,2] = LOGLEN * Math.PI * Math.Pow(rH / 100, 2);
                totals[i,3] = 2 * rH;
                totals[i,4] = taper;

                rL = rH;
            }
            return totals;
        }
        public void setRanges(double[] ranges, double[] values)
        {
            rangeValue = new double[ranges.Length];
            rangeSizes = new double[ranges.Length];
            rangeNo = ranges.Length;
            for (int i = 0; i < ranges.Length; i++)
            {
                rangeSizes[i] = ranges[i];
                rangeValue[i] = values[i];
            }
        }
        public double[] rangeBracket()
        {
            return rangeSizes;
        }
        public double[] GetRangeValue()
        {
            return rangeValue;
        }
        public int getNo()
        {
            return rangeNo;
        }

    }
}
