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
using Xamarin.Auth;
using System.ComponentModel;

namespace GreenBankX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            try
            {
                if (((List<PriceRange>)Application.Current.Properties["Prices"]).Count == 0)
                {
                    LoadPriceFiles();
                }
                if (((List<Plot>)Application.Current.Properties["Plots"]).Count == 0)
                {
                    LoadPlotFiles();
                    LoadTreeFiles2();
                }
            }
            catch
            {

            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:

                    break;

                case Device.Android:
                    try { var nu = "Hello: " + Xamarin.Forms.DependencyService.Get<ILogin>().AccountName(); }
                    catch
                    {
                        ToolDrive.Text = "";
                        ToolDown.Text = "";
                    }

                    break;
            }
        }
        public void Signot()
        {
            Xamarin.Forms.DependencyService.Get<ILogin>().SignOut();
        }
        void Driv3r()
        {
            string nu = (Xamarin.Forms.DependencyService.Get<ILogin>().UseDrive(-1));

        }
        void OnLoginTest()
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    try { var nu  =  Xamarin.Forms.DependencyService.Get<ILogin>().AccountName(); }
                    catch
                    {
                        clientId = Constants.AndroidClientId;
                        redirectUri = Constants.AndroidRedirectUrl;
                        bool wait = Xamarin.Forms.DependencyService.Get<ILogin>().SignIn();
                        try { var nu = Xamarin.Forms.DependencyService.Get<ILogin>().AccountName(); }
                        catch { }
                        ToolDrive.Text = "Upload";
                        ToolDown.Text = "Download";
                    }

                    break;
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
                inputStream.Dispose();
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
                inputStream.Dispose();
            }
        }
        //loads data from .xls files populates plots with trees. data for trees is stored in <PlotName>.xls
        void LoadTreeFiles2()
        {
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
                    for (int y = 0; y < int.Parse(sheet.GetValueRowCol(1, 10).ToString()); y++)
                    {
                        for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count; x++)
                        {
                            if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName() == sheet.GetValueRowCol(2 + y, 2).ToString())
                            {
                                Thisplot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x);
                                for (int z = 0; z < Thisplot.getTrees().Count; z++)
                                {
                                    if (Thisplot.getTrees().ElementAt(z).id.ToString() == sheet.GetValueRowCol(2 + y, 1).ToString())
                                    {
                                        treecounter = z;
                                    }
                                }
                                if (treecounter > -1)
                                {
                                    Thisplot.getTrees().ElementAt(treecounter).AddToHistory(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString()));
                                }
                                else
                                {
                                    Thisplot.AddTree(new Tree(double.Parse(sheet.GetValueRowCol(2 + y, 4).ToString()), double.Parse(sheet.GetValueRowCol(2 + y, 5).ToString()), int.Parse(sheet.GetValueRowCol(2 + y, 1).ToString()), DateTime.Parse(sheet.GetValueRowCol(2 + y, 3).ToString())));
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

        private void ToolDown_Clicked(object sender, EventArgs e)
        {
            var nu = (Xamarin.Forms.DependencyService.Get<ILogin>().Download(-1));
        }

    }
    class ClockViewModel : INotifyPropertyChanged
    {
        String dateTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClockViewModel()
        {
            this.DateTime = (string)Application.Current.Properties["Boff"];

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                this.DateTime = (string)Application.Current.Properties["Boff"];
                return true;
            });
        }

        public String DateTime
        {
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("DateTime"));
                    }
                }
            }
            get
            {
                return dateTime;
            }
        }
    }
}