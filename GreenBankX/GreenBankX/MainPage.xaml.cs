using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TK.CustomMap;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GreenBankX
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            Application.Current.Properties["Plots"] = new List<Plot>();
            Application.Current.Properties["Prices"] = new List<PriceRange>();
            LoadPriceFiles();
            LoadPlotFiles();
            LoadTreeFiles();
        }
        async void OpenMenu(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }
        void LoadPriceFiles()
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xlsx");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xlsx", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                for (int x = 0; x < workbook.Worksheets.Count; x++) {

                    SortedList<double, double> bracket = new SortedList<double, double>();
                    IWorksheet sheet = workbook.Worksheets[x];
                    if (sheet.GetValueRowCol(1, 1).ToString()=="Name") { 
                     string name = sheet.GetValueRowCol(1, 2).ToString();
                     double loglen = double.Parse(sheet.GetValueRowCol(2, 2).ToString());
                     for (int y = 0; y < int.Parse(sheet.GetValueRowCol(3, 3).ToString()); y++) {
                           
                             bracket.Add(double.Parse(sheet.GetValueRowCol(4+y, 1).ToString()), double.Parse(sheet.GetValueRowCol(4 + y, 2).ToString()));
                        }
                    ((List<PriceRange>)Application.Current.Properties["Prices"]).Add(new PriceRange(name, "yew", bracket, loglen));
                }
                }

            }

        }
        void LoadPlotFiles()
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Plots.xlsx");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Plots.xlsx", FileMode.Open);
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
                        for (int y = 0; y < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count; y++) {
                            if (((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(y).GetName()== sheet.GetValueRowCol(3, 2).ToString()) {
                                newPlot.SetRange(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(y));
                            }
                        }
                        List<Position> PolyPlot = new List<Position>();
                         for (int y = 0; y < int.Parse(sheet.GetValueRowCol(3, 3).ToString()); y++)
                        {
                            PolyPlot.Add(new Position(double.Parse(sheet.GetValueRowCol(5 + y, 1).ToString()), double.Parse(sheet.GetValueRowCol(5 + y, 2).ToString())));
                        }
                        newPlot.AddPolygon(PolyPlot);
                      ((List<Plot>)Application.Current.Properties["Plots"]).Add(newPlot);
                    }
                }

            }
        }

        void LoadTreeFiles()
        {
            for (int z = 0; z < ((List<Plot>)Application.Current.Properties["Plots"]).Count; z++) {
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(z);
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/"+ thisPlot.GetName()+".xlsx");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + thisPlot.GetName()+".xlsx", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                for (int x = 0; x < workbook.Worksheets.Count; x++)
                {

                    IWorksheet sheet = workbook.Worksheets[x];
                    if (sheet.GetValueRowCol(1, 1).ToString() == "ID")
                    {
                        string ID = sheet.GetValueRowCol(1, 2).ToString();
                            SortedList<DateTime, (double, double)> History= new SortedList<DateTime, (double, double)>();

                        Tree newTree = new Tree(float.Parse(sheet.GetValueRowCol(3, 2).ToString()), float.Parse(sheet.GetValueRowCol(3, 3).ToString()), int.Parse(sheet.GetValueRowCol(1, 2).ToString()),DateTime.Parse((sheet.GetValueRowCol(3, 1).ToString())));
                        for (int y = 1; y < int.Parse(sheet.GetValueRowCol(1, 3).ToString()); y++)
                        {
                                newTree.AddToHistory(float.Parse(sheet.GetValueRowCol(3+y, 2).ToString()), float.Parse(sheet.GetValueRowCol(3+y, 3).ToString()), DateTime.Parse((sheet.GetValueRowCol(3+y, 1).ToString())));
                        }
                            ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(z).AddTree(newTree);
                    }
                }

            }

        }
        }
    }
}
