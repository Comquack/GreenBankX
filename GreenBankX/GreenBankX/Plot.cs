using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TK.CustomMap;
using Xamarin.Essentials;

namespace GreenBankX
{
    class Plot
    {
        string name;
        double[] geotag = { 0, 0 };
        List<Tree> trees;
        List<Position> polygon;
        //PriceRange PlotPrice;
        public string Owner { get; set;}
        public int YearPlanted { get; set; }
        public string Describe { get; set; }
        public string NearestTown { get; set; }
        public Plot(string name)
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
        public void SetName(string newname)
        {
            this.name=newname;
        }
      //  public void SetRange(PriceRange newRange) {
      //      PlotPrice = newRange;
      //  }

      //  public PriceRange GetRange() {
      //      return PlotPrice;
      //  }


        public double GetArea()
        {
            double area = 0;
            int m = GetPolygon().Count;
            for (int x = 0; x < m; x++) {
                area = area + GetPolygon().ElementAt(x).Latitude* 111139* 111139* (GetPolygon().ElementAt((((x-1)%m)+m)%m).Longitude - GetPolygon().ElementAt((((x+1) % m) + m) % m).Longitude)/1000000;
            }
            return Math.Abs(area)*0.5;
        }
        public void AddPolygon(List<Position> newpoly) {
            polygon = newpoly;
        }
        public List<Position> GetPolygon() {
            return polygon;
        }
    }
}
