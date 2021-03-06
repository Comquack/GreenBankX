﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GreenBankX
{
    public class Tree
    {
        public double Diameter { get; set;}
        public double MerchHeight { get; set;} //Now regular height
        public double ActualMerchHeight { get; set; }
        public int Id { get; }
        private SortedList<DateTime, (double, double, double)> History;
        // constructor
        public Tree(double diam, double merch, int ID, DateTime date)
        {

            Diameter = diam;
            MerchHeight = merch;
            Id = ID;
            ActualMerchHeight = -1;
            History = new SortedList<DateTime, (double, double, double)>
            {
                { date, (diam, merch,ActualMerchHeight) }
            };
        }
        public Tree(double diam, double merch, int ID, DateTime date, double actualmerch)
        {
            ActualMerchHeight = actualmerch;
            Diameter = diam;
            MerchHeight = merch;
            Id = ID;
            History = new SortedList<DateTime, (double, double,double)>
            {
                { date, (diam, merch,ActualMerchHeight) }
            };
        }
        public void AddToHistory(double diam, double merch, DateTime date) {
            History.Add(date, (diam, merch,-1));
            Diameter = History.Last().Value.Item1;
            MerchHeight = Math.Round(History.Last().Value.Item2,2);
        }
        public void AddToHistory(double diam, double merch, DateTime date, double actualmerch)
        {
            History.Add(date, (diam, merch, actualmerch));
            Diameter = History.Last().Value.Item1;
            MerchHeight = Math.Round(History.Last().Value.Item2, 2);
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
        {
            return Math.Round(this.History.ElementAt(this.History.Count-1).Value.Item1,2);
        }
        public double Merch => Math.Round(this.History.ElementAt(this.History.Count - 1).Value.Item2,2);
        public int ID => this.Id;
        public SortedList<DateTime, (double, double,double)> GetHistory() {
            return this.History;
        }
        public string GetCSV()
        {
            String result = Id + "," + Diameter.ToString() + "," + MerchHeight.ToString() + ";";
            return result;
        }

    }
}
