using System;
using System.Collections.Generic;
using System.Text;

namespace GreenBankX
{
    public class Tree
    {
        private double diameter;
        private double merchHeight;
        private int id;
        // constructor
        public Tree(float diam, float merch, int ID )
        {
            diameter = diam;
            merchHeight = merch;
            id = ID;
        }

        public double GetDia()
        { return this.diameter;
        }
        public double Merch => this.merchHeight;
        public int ID => this.id;
        public string GetCSV()
        {
            String result = id + "," + diameter.ToString() + "," + merchHeight.ToString() + ";";
            return result;
        }

    }
}
