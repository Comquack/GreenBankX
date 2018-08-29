using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        worksheet.SetValue(1, 2, thisPlot.GetName());
                        worksheet.SetValue(2, 2, thisPlot.GetTag()[0].ToString());
                        worksheet.SetValue(2, 3, thisPlot.GetTag()[1].ToString());
                        worksheet.SetValue(3, 1, "Pricing Name");
                        worksheet.SetValue(3, 2, thisPlot.GetRange().GetName());
                        worksheet.SetValue(3, 3, thisPlot.GetPolygon().Count.ToString());
                        worksheet.SetValue(4, 1, "Border Co-ordinates");
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
        
        public void SaveTrees(int all, List<Plot> names)
        {
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++)
            {
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                if (all == 1||names.Contains(thisPlot) || !File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + thisPlot.GetName() + ".xls")) { 
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);
                    workbook.Version = ExcelVersion.Excel97to2003;
                    List<Tree> TreeList = thisPlot.getTrees();
                    for (int y = 0; y < TreeList.Count; y++)
                    {
                        Tree thisTree = TreeList.ElementAt(y);
                        workbook.Worksheets.Create(thisTree.ID.ToString());
                        IWorksheet worksheet = workbook.Worksheets[y + 1];


                        worksheet.SetValue(1, 1, "ID");
                        worksheet.SetValue(1, 2, thisTree.ID.ToString());
                        worksheet.SetValue(1, 3, thisTree.GetHistory().Count.ToString());
                        worksheet.SetValue(2, 1, "Date");
                        worksheet.SetValue(2, 2, "Girth");
                        worksheet.SetValue(2, 1, "Mercjantable Height");
                        for (int z = 0; z < thisTree.GetHistory().Count; z++)
                        {
                            worksheet.SetValue(3 + z, 1, thisTree.GetHistory().ElementAt(z).Key.ToString());
                            worksheet.SetValue(3 + z, 2, thisTree.GetHistory().ElementAt(z).Value.Item1.ToString());
                            worksheet.SetValue(3 + z, 3, thisTree.GetHistory().ElementAt(z).Value.Item2.ToString());
                        }
                    }
                    if (workbook.Worksheets.Count > 1) { workbook.Worksheets[0].Remove(); }


                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    workbook.Close();
                    Xamarin.Forms.DependencyService.Get<ISave>().Save(thisPlot.GetName() + ".xls", "application/msexcel", stream);
                }

            }
            }

        }
        public void DeletePlot(string name) {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + name + ".xls");
            if (doesExist)
            {
                try
                {
                    File.Delete(DependencyService.Get<ISave>().GetFileName() + "/" + name + ".xls");
                }
                catch { }
            }
                doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Plots.xls");
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
    }
}
