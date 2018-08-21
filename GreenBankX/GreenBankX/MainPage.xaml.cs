using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            LoadFiles();
        }
        async void OpenMenu(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }
        void LoadFiles()
        {
            bool doesExist = File.Exists(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xlsx");
            if (doesExist)
            {
                ExcelEngine excelEngine = new ExcelEngine();
                FileStream inputStream = new FileStream(DependencyService.Get<ISave>().GetFileName() + "/Pricings.xlsx", FileMode.Open);
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                string test = "";
                for (int x = 0; x < workbook.Worksheets.Count; x++) {

                    SortedList<double, double> bracket = new SortedList<double, double>();
                    IWorksheet sheet = workbook.Worksheets[x];
                    if (sheet.GetValueRowCol(1, 1).ToString()=="Name") { 
                     string name = sheet.GetValueRowCol(1, 2).ToString();
                     double loglen = double.Parse(sheet.GetValueRowCol(2, 2).ToString());
                     for (int y = 0; y < int.Parse(sheet.GetValueRowCol(3, 3).ToString()); y++) {
                            //test = test + "key "+double.Parse(sheet.GetValueRowCol(4 + x, 1).ToString())+"val " + double.Parse(sheet.GetValueRowCol(4 + x, 2).ToString())+"\n";
                             bracket.Add(double.Parse(sheet.GetValueRowCol(4+y, 1).ToString()), double.Parse(sheet.GetValueRowCol(4 + y, 2).ToString()));
                        }
                        //test = test + "break\n";
                    ((List<PriceRange>)Application.Current.Properties["Prices"]).Add(new PriceRange(name, "yew", bracket, loglen));
                }
                }


                boffo.Text = test;
            }
            else { 
            boffo.Text = "Goodbye";
            }
        }
    }
}
