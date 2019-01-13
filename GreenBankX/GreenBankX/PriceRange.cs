using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBankX
{
    class PriceRange
    {
        private SortedList<double,double> PriceBrack;
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
        public double GetLength() => logLen;
        public void SetLength(int len) {
            logLen = len;
        }

        public SortedList<double, double> GetBrack()
        {
            return PriceBrack;
        }
        //set value of all Price brackets
        public void SetBrack(SortedList<double, double> newBrack)
        {
            PriceBrack = newBrack;
        }
        //Add new Entry to Price brackets
        public Boolean addBrack(double dia, double price)
        {
            try { PriceBrack.Add(dia, price); }catch{
                return false;
            }
            return true;
        }
    }
}

