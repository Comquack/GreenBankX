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
                    SaveAll.GetInstance().LoadPriceFiles();
                }
                if (((List<Plot>)Application.Current.Properties["Plots"]).Count == 0)
                {
                    SaveAll.GetInstance().LoadPlotFiles();
                    SaveAll.GetInstance().LoadTreeFiles2();
                }
            }
            catch{}

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