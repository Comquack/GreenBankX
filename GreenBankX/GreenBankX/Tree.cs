using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GreenBankX
{
    public class Tree
    {
        private double diameter;
        private double merchHeight;
        private int id;
        private SortedList<DateTime, (double,double)> History;
        // constructor
        public Tree(float diam, float merch, int ID, DateTime date)
        {

            diameter = diam;
            merchHeight = merch;
            id = ID;
            History = new SortedList<DateTime, (double, double)>();
            History.Add(date, (diam, merch));
        }
        public void AddToHistory(float diam, float merch, DateTime date) {
            History.Add(date, (diam, merch));
        }
        public Boolean RemoveFromHistory(DateTime date)
        {
            if (History.ContainsKey(date)) {
                History.Remove(date);
                return true;
            } else {
                return false;
            }
        }


        public double GetDia()
        { return this.History.ElementAt(this.History.Count-1).Value.Item1;
        }
        public double Merch => this.History.ElementAt(this.History.Count - 1).Value.Item2;
        public int ID => this.id;
        public SortedList<DateTime, (double, double)> GetHistory() {
            return this.History;
        }
        public string GetCSV()
        {
            String result = id + "," + diameter.ToString() + "," + merchHeight.ToString() + ";";
            return result;
        }

    }
}
