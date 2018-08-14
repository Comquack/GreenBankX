using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBankX
{
    class PriceRange
    {
        private SortedList<double,double> PriceBrack;
        private double[,] PriceBracket;
        private string brackName;
        private string treeType;
        private double logLen;

        public PriceRange(string name, string type , SortedList<double, double> newBracket,double setLen) {
            brackName = name;
            treeType = type;
            PriceBrack = newBracket;
            logLen = setLen;

        }
        public string GetName() {
            return brackName;
        }
        public double GetLength()
        {
            return logLen;
        }
 
        public double[,] GetBracket() {
            return PriceBracket;
        }

        public void SetBracket(double[,] newBrack)
        {
            PriceBracket = newBrack;
        }

        public SortedList<double, double> GetBrack()
        {
            return PriceBrack;
        }

        public void SetBrack(SortedList<double, double> newBrack)
        {
            PriceBrack = newBrack;
        }
        public Boolean addBrack(double dia, double price)
        {
            try { PriceBrack.Add(dia, price); }catch{
                return false;
            }
            return true;
        }
    }
}

