﻿
using GreenBankX.Resources;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TK.CustomMap;
using Xamarin.Auth;
using Xamarin.Forms;

namespace GreenBankX
{
    
    class SaveAll
    {
        public static SaveAll instance = new SaveAll();
        public static SaveAll GetInstance()
        {
            if (instance == null)
            {
                return new SaveAll();
            }
            return instance;
        }

        private SaveAll()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY4MzVAMzEzNjJlMzIyZTMwZmMzUTBVc2x2STVZNG4rTm1mdXlXQ1czR09UQ1p0QzB2SmNjWFFtZ2RmOD0=");
        }

        public void SaveEvery() {
            SavePlots();
            SavePricing();
            SaveTrees2();
        }
        public void SavePricing()
        {
            //Create an instance of ExcelEngine.
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2013.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;

                //Create a workbook with a worksheet
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                workbook.Version = ExcelVersion.Excel97to2003;

                //Adding text to a cell
                for (int y = 0; y < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); y++)
                {
                    PriceRange thisPrice = ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(y);
                    workbook.Worksheets.Create(thisPrice.GetName());
                    IWorksheet worksheet = workbook.Worksheets[y + 1];

                    worksheet.SetValue(1, 1, "Name");
                    worksheet.SetValue(2, 1, "Log Size");
                    worksheet.SetValue(1, 2, thisPrice.GetName());
                    worksheet.SetValue(2, 2, thisPrice.GetLength().ToString());
                    worksheet.SetValue(3, 1, "Size");
                    worksheet.SetValue(3, 2, "Price");
                    worksheet.SetValue(3, 3, thisPrice.GetBrack().Count.ToString());
                    for (int x = 0; x < thisPrice.GetBrack().Count; x++)
                    {
                        worksheet.SetValue(4 + x, 1, thisPrice.GetBrack().ElementAt(x).Key.ToString());
                        worksheet.SetValue(4 + x, 2, thisPrice.GetBrack().ElementAt(x).Value.ToString());
                    }
                    worksheet.Range["A1:A3"].CellStyle.Locked = true;
                    worksheet.Range[3,2].CellStyle.Locked = true;
                }
                workbook.Worksheets[0].Remove();
                //Save the workbook to stream in xlsx format. 
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);

                workbook.Close();

                //Save the stream as a file in the device and invoke it for viewing
                Xamarin.Forms.DependencyService.Get<ISave>().Save("Pricings.xls", "application/msexcel", stream);
            }

        }
        public void SavePlots()
        {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {

                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                workbook.Version = ExcelVersion.Excel97to2003;

                for (int y = 0; y < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); y++)
                    {
                        Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(y);
                        workbook.Worksheets.Create(thisPlot.GetName());
                        IWorksheet worksheet = workbook.Worksheets[y + 1];


                        worksheet.SetValue(1, 1, "Name");
                        worksheet.SetValue(2, 1, "Co-ordinates");
                    worksheet.SetValue(1, 4, "Owner");
                    worksheet.SetValue(2, 4, "Location");
                    worksheet.SetValue(3, 4, "Year Planted");
                    worksheet.SetValue(4, 4, "Comments");
                    if (thisPlot.Owner != null) {
                        worksheet.SetValue(1, 5, thisPlot.Owner);
                    }
                    if (thisPlot.NearestTown != null)
                    {
                        worksheet.SetValue(2, 5, thisPlot.NearestTown);
                    }
                    if (thisPlot.Describe != null)
                    {
                        worksheet.SetValue(4, 5, thisPlot.Describe);
                    }
                    worksheet.SetValue(3, 5, thisPlot.YearPlanted.ToString());
                    worksheet.SetValue(1, 2, thisPlot.GetName());
                        worksheet.SetValue(2, 2, thisPlot.GetTag()[0].ToString());
                        worksheet.SetValue(2, 3, thisPlot.GetTag()[1].ToString());
                        worksheet.SetValue(3, 1, "Pricing Name");
                      //  if (thisPlot.GetRange() != null){
                        //    worksheet.SetValue(3, 2, thisPlot.GetRange().GetName());
                       // }
                        worksheet.SetValue(3, 3, thisPlot.GetPolygon().Count.ToString());

                        worksheet.SetValue(4, 1, "Border Co-ordinates");
                    worksheet.Range["$A$4:$B$4"].Merge();
                    for (int x = 0; x < thisPlot.GetPolygon().Count; x++)
                        {
                            worksheet.SetValue(5 + x, 1, thisPlot.GetPolygon().ElementAt(x).Latitude.ToString());
                            worksheet.SetValue(5 + x, 2, thisPlot.GetPolygon().ElementAt(x).Longitude.ToString());
                        }
                    }

                    workbook.Worksheets[0].Remove();
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    workbook.Close();
                    Xamarin.Forms.DependencyService.Get<ISave>().Save("Plots.xls", "application/msexcel", stream);
                }
            
        }
        
        public void SaveTrees2()
        {
            int count = 0;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {

                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                workbook.Version = ExcelVersion.Excel97to2003;
                IWorksheet worksheet = workbook.Worksheets[0];
                worksheet.SetValue(1, 1, "Tree ID");
                worksheet.SetValue(1, 2, "Plot");
                worksheet.SetValue(1, 3, "Date");
                worksheet.SetValue(1, 4, "Girth");
                worksheet.SetValue(1, 5, "Height");
                worksheet.SetValue(1, 6, "Merchantable Height");
                worksheet.SetValue(1, 14, "Currency");
                worksheet.SetValue(1, 15, "Rate vs USD");

                for (int x = 0; x < ((List<(string, double)>)Application.Current.Properties["Currenlist"]).Count(); x++) {
                    try
                    {
                        worksheet.SetValue(2 + x, 14, ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt(x).Item1);
                        worksheet.SetValue(2 + x, 15, ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt(x).Item2.ToString());
                    }
                    catch { worksheet.SetValue(2 + x, 15, "ErrorEventArgs"); }
                }

                    for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
                {
                    int minyear = 0;
                    int maxyear = 0;
                    Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                    List<Tree> TreeList = thisPlot.getTrees();
                    for (int y = 0; y < TreeList.Count; y++)
                    {
                        Tree thisTree = TreeList.ElementAt(y);
                        for (int z = 0; z < thisTree.GetHistory().Count; z++)
                        {
                            if (minyear == 0)
                            {
                                minyear = thisTree.GetHistory().ElementAt(z).Key.Year;
                            }
                            else
                            {
                                minyear = Math.Min(minyear, thisTree.GetHistory().ElementAt(z).Key.Year);
                            }
                            maxyear = Math.Max(maxyear, thisTree.GetHistory().ElementAt(z).Key.Year);
                            worksheet.SetValue(2 + count, 1, thisTree.ID.ToString());
                            worksheet.SetValue(2 + count, 2, thisPlot.GetName().ToString());
                            worksheet.SetValue(2 + count, 3, thisTree.GetHistory().ElementAt(z).Key.ToString());
                            worksheet.SetValue(2 + count, 4, thisTree.GetHistory().ElementAt(z).Value.Item1.ToString());
                            worksheet.SetValue(2 + count, 5, thisTree.GetHistory().ElementAt(z).Value.Item2.ToString());
                            worksheet.SetValue(2 + count, 6, thisTree.GetHistory().ElementAt(z).Value.Item3.ToString());
                            count++;
                        }
                    }
                }
                worksheet.SetValue(1, 11, count.ToString());
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                workbook.Close();
                Xamarin.Forms.DependencyService.Get<ISave>().Save("trees.xls", "application/msexcel", stream);
            }

        }
        public void DeletePlot(string name) {
               bool doesExist = System.IO.File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Plots.xls");
                if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Plots.xls", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                workbook.Version = ExcelVersion.Excel97to2003;
                for (int x = 0; x < workbook.Worksheets.Count; x++)
                {
                    if (workbook.Worksheets.ElementAt(x).Name == name) {
                        workbook.Worksheets.Remove(x);
                    }
                }
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                workbook.Close();
                Xamarin.Forms.DependencyService.Get<ISave>().Save("Plots.xls", "application/msexcel", stream);

            }
        }

        //loads data from .xls files. prices. data for plots is stored in Pricings.xls
        public void LoadPriceFiles()
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xls");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xls", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                for (int x = 0; x < workbook.Worksheets.Count; x++)
                {

                    SortedList<double, double> bracket = new SortedList<double, double>();
                    IWorksheet sheet = workbook.Worksheets[x];
                    if (sheet.GetValueRowCol(1, 1).ToString() == "Name")
                    {
                        string name = sheet.GetValueRowCol(1, 2).ToString();
                        double loglen = double.Parse(sheet.GetValueRowCol(2, 2).ToString());
                        for (int y = 0; y < int.Parse(sheet.GetValueRowCol(3, 3).ToString()); y++)
                        {

                            bracket.Add(double.Parse(sheet.GetValueRowCol(4 + y, 1).ToString()), double.Parse(sheet.GetValueRowCol(4 + y, 2).ToString()));
                        }
                    ((List<PriceRange>)Application.Current.Properties["Prices"]).Add(new PriceRange(name, "yew", bracket, loglen));
                    }
                }
                inputStream.Dispose();
            }

        }
        //loads data from .xls files plots. data for plots is stored in Plots.xls
        public void LoadPlotFiles()
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Plots.xls");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Plots.xls", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                for (int x = 0; x < workbook.Worksheets.Count; x++)
                {

                    SortedList<double, double> bracket = new SortedList<double, double>();
                    IWorksheet sheet = workbook.Worksheets[x];
                    if (sheet.GetValueRowCol(1, 1).ToString() == "Name")
                    {
                        string name = sheet.GetValueRowCol(1, 2).ToString();
                        double[] geotag = { double.Parse(sheet.GetValueRowCol(2, 2).ToString()), double.Parse(sheet.GetValueRowCol(2, 3).ToString()) };

                        Plot newPlot = new Plot(name);
                        newPlot.SetTag(geotag);
                        newPlot.Owner = sheet.GetValueRowCol(1, 5).ToString();
                        try { newPlot.YearPlanted = int.Parse(sheet.GetValueRowCol(3, 5).ToString()); } catch { };
                        newPlot.NearestTown = sheet.GetValueRowCol(2, 5).ToString();
                        newPlot.Owner = sheet.GetValueRowCol(1, 5).ToString();
                        newPlot.Describe = sheet.GetValueRowCol(4, 5).ToString();
                        List<Position> PolyPlot = new List<Position>();
                        for (int y = 0; y < int.Parse(sheet.GetValueRowCol(3, 3).ToString()); y++)
                        {
                            PolyPlot.Add(new Position(double.Parse(sheet.GetValueRowCol(5 + y, 1).ToString()), double.Parse(sheet.GetValueRowCol(5 + y, 2).ToString())));
                        }
                        newPlot.AddPolygon(PolyPlot);
                        ((List<Plot>)Application.Current.Properties["Plots"]).Add(newPlot);
                    }
                }
                inputStream.Dispose();
            }
        }
        //loads data from .xls files populates plots with trees. data for trees is stored in <PlotName>.xls
        public void LoadTreeFiles2()
        {
            List<(string, double)> currency = new List<(string, double)>();
            int shift = 0;
            int treecounter = -1;
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/trees.xls");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/trees.xls", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                IWorksheet sheet = workbook.Worksheets[0];
                Plot Thisplot;
                if (sheet.GetValueRowCol(1, 1).ToString() == "Tree ID")
                {
                    //if (sheet.GetValueRowCol(1, 14).ToString() == "Currency") {
                        int p = 0;
                        while (sheet.GetValueRowCol(2+p, 14).ToString()  != null && sheet.GetValueRowCol(2+p, 14).ToString() != "") {
                            currency.Add((sheet.GetValueRowCol(2+p, 14).ToString(), double.Parse(sheet.GetValueRowCol(2+p, 15).ToString())));
                            p++;
                        }
                        Application.Current.Properties["Currenlist"] = currency;
                    //}
                    if (sheet.GetValueRowCol(1, 6).ToString() == "Merchantable Height") {
                        shift = 1;
                    }
                    shift = 1;
                    for (int y = 0; y < int.Parse(sheet.GetValueRowCol(1, 10+shift).ToString()); y++)
                    {
                        for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count; x++)
                        {
                            if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName() == sheet.GetValueRowCol(2 + y, 2).ToString())
                            {
                                Thisplot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                                for (int z = 0; z < Thisplot.getTrees().Count; z++)
                                {
                                    if (Thisplot.getTrees().ElementAt(z).Id.ToString() == sheet.GetValueRowCol(2 + y, 1).ToString())
                                    {
                                        treecounter = z;
                                    }
                                }
                                if (treecounter > -1)
                                {
                                    if (shift == 1)
                                    {
                                        Thisplot.getTrees().ElementAt(treecounter).AddToHistory(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 6).ToString()));
                                    }
                                    else { 
                                        Thisplot.getTrees().ElementAt(treecounter).AddToHistory(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString()));
                                    }
                                 }
                                else
                                {
                                    if (shift == 1)
                                    {
                                        Thisplot.AddTree(new Tree(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), int.Parse(sheet.GetValueRowCol(2 + y, 1).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 6).ToString())));
                                    }
                                    else
                                    {
                                        Thisplot.AddTree(new Tree(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), int.Parse(sheet.GetValueRowCol(2 + y, 1).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString())));
                                    }
                                }
                                treecounter = -1;
                                x = ((List<Plot>)Application.Current.Properties["Plots"]).Count + 1;
                            }
                        }
                    }
                }
                inputStream.Dispose();
            }

        }

        public void LoadAll() {
            ((List<PriceRange>)Application.Current.Properties["Prices"]).Clear();
            ((List<Plot>)Application.Current.Properties["Plots"]).Clear();
            LoadPriceFiles();
            LoadPlotFiles();
            LoadTreeFiles2();
        }

        public void Kamel() {
            string output = "<?xml version =\"1.0\" encoding=\"UTF-8\"?>\n<kml xmlns=\"http://www.opengis.net/kml/2.2\">\n<Document>";
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count; x++)
            {
                Plot ThisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                output += "\n<Placemark>\n<name>"+ ThisPlot.GetName()+ "</name>\n";
                double[] tag =  ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetTag();
                output += "<description>\n";
                if (ThisPlot.YearPlanted > 0) {
                    output += AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("YPlant") + ": " + ThisPlot.YearPlanted.ToString()+"\n";
                }

                List<Tree> TreeList = ThisPlot.getTrees();
                int year = DateTime.Now.Year;
               
                output +="</description>";
                   output += "<Point>\n<coordinates>"+tag[1].ToString()+","+tag[0].ToString()+"</coordinates>\n </Point>\n";
                if (ThisPlot.GetPolygon().Count > 0)
                {
                    output += "<Polygon><outerBoundaryIs><LinearRing><coordinates>\n";
                    for (int y = 0; y < ThisPlot.GetPolygon().Count; y++)
                    {
                        output += ThisPlot.GetPolygon().ElementAt(y).Longitude.ToString() + ", " + ThisPlot.GetPolygon().ElementAt(y).Latitude + ", 0.\n";
                    }
                    output += "  </coordinates></LinearRing></outerBoundaryIs></Polygon>\n<Style><PolyStyle><color>#a00000ff</color><outline>0</outline></PolyStyle></Style>\n</Placemark>";
                }
                else {
                    output += "</Placemark>";
                }
            }      
        
            output += "\n</Document>\n</kml>";

            byte[] byteArray = Encoding.UTF8.GetBytes(output);
            MemoryStream stream = new MemoryStream(byteArray);
            Xamarin.Forms.DependencyService.Get<ISave>().Save("map.kml", "text/plain", stream);
            
        }

    }
}
