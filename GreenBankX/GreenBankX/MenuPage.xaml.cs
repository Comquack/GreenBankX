using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


using Syncfusion.XlsIO;
using System.IO;
using TK.CustomMap;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        public MenuPage ()
		{
			InitializeComponent ();
            try
            {
                if (((List<PriceRange>)Application.Current.Properties["Prices"]).Count == 0)
                {
                    LoadPriceFiles();
                }
                if (((List<Plot>)Application.Current.Properties["Plots"]).Count == 0)
                {
                    LoadPlotFiles();
                    LoadTreeFiles();
                }
            }
            catch
            {
                
            }
        }
        async void OpenMeasure(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MeasureTree());
        }
        async void OpenMap(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePlot());
        }
        async void OpenManager(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManagePlots());
        }
        async void OpenPrice(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePricing());
        }
       

        //loads data from .xls files. prices. data for plots is stored in Pricings.xls
        void LoadPriceFiles()
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

            }

        }
        //loads data from .xls files plots. data for plots is stored in Plots.xls
        void LoadPlotFiles()
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
                        for (int y = 0; y < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count; y++)
                        {
                            if (((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(y).GetName() == sheet.GetValueRowCol(3, 2).ToString())
                            {
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
        //loads data from .xls files populates plots with trees. data for trees is stored in <PlotName>.xls
        void LoadTreeFiles()
        {
            for (int z = 0; z < ((List<Plot>)Application.Current.Properties["Plots"]).Count; z++)
            {
                Plot thisPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(z);
                bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/" + thisPlot.GetName() + ".xls");
                if (doesExist)
                {
                    ExcelEngine excelEngine = new ExcelEngine();
                    FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/" + thisPlot.GetName() + ".xls", FileMode.Open);
                    IApplication application = excelEngine.Excel;
                    IWorkbook workbook = application.Workbooks.Open(inputStream);
                    for (int x = 0; x < workbook.Worksheets.Count; x++)
                    {

                        IWorksheet sheet = workbook.Worksheets[x];
                        if (sheet.GetValueRowCol(1, 1).ToString() == "ID")
                        {
                            string ID = sheet.GetValueRowCol(1, 2).ToString();
                            SortedList<DateTime, (double, double)> History = new SortedList<DateTime, (double, double)>();

                            Tree newTree = new Tree(double.Parse(sheet.GetValueRowCol(3, 2).ToString()), double.Parse(sheet.GetValueRowCol(3, 3).ToString()), int.Parse(sheet.GetValueRowCol(1, 2).ToString()), DateTime.Parse((sheet.GetValueRowCol(3, 1).ToString())));
                            for (int y = 1; y < int.Parse(sheet.GetValueRowCol(1, 3).ToString()); y++)
                            {
                                double.Parse(sheet.GetValueRowCol(3 + y, 2).ToString());
                                newTree.AddToHistory(double.Parse(sheet.GetValueRowCol(3 + y, 2).ToString()), double.Parse(sheet.GetValueRowCol(3 + y, 3).ToString()), DateTime.Parse((sheet.GetValueRowCol(3 + y, 1).ToString())));
                            }
                                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(z).AddTree(newTree);
                        }
                    }

                }

            }
        }
    }

}