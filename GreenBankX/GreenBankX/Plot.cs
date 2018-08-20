using System;
using System.Collections.Generic;
using System.Text;
using TK.CustomMap;
using Xamarin.Essentials;

namespace GreenBankX
{
    class Plot
    {
        String name;
        double[] geotag = { 0, 0 };
        List<Tree> trees;
        List<Position> polygon;
        String Csv;
        PriceRange PlotPrice;
        public Plot(String name)
        {
            this.name = name;
            this.trees = new List<Tree>();
            polygon = new List<Position>();
        }
        public void AddTree(Tree newTree)
        {
            this.trees.Add(newTree);
        }
        public List<Tree> getTrees()
        {
            return trees;
        }
        public void SetTag(double[] geotag)
        {
            this.geotag = geotag;
        }

        public double[] GetTag()
        {
            return this.geotag;
        }
        public string GetName()
        {
            return this.name;
        }

        public void SetRange(PriceRange newRange) {
            PlotPrice = newRange;
        }

        public PriceRange GetRange() {
            return PlotPrice;
        }


        public String GetCSV()
        {
            String result = name + "," + geotag[0] + "," + geotag[1] + ";";
            foreach (Tree a in this.trees)
            {
                result += a.GetCSV();
            }
            return result;
        }
        public void AddPolygon(List<Position> newpoly) {
            polygon = newpoly;
        }
        public List<Position> GetPolygon() {
            return polygon;
        }
       // public void setCSV(String csv)
       // {
           // this.Csv = csv;
           // String[] data = csv.Split(';');
           // String[] data2 = data[0].Split(',');
           // this.name = data2[0];
           // this.geotag[0] = Double.Parse(data2[1]);
          //  this.geotag[1] = Double.Parse(data2[2]);
          //  for (int x = 1; x < data.Length; x++)
          //  {
          //      String[] dataT = data[x].Split(',');
         //       this.trees.Add(new Tree(float.Parse(dataT[1]), float.Parse(dataT[2]),x));
        //    }
        //}

    }
}
