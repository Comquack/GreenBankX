using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using GreenBankX.Resources;
using System.Threading;
using Rg.Plugins.Popup.Services;

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
                    try { var nu = DependencyService.Get<ILogin>().AccountName();

                    }
                    catch
                    {

                    }

                    break;
            }
        }
        public void Signot()
        {
            if (Toolout.Text == "")
            {
                return;
            }
            DependencyService.Get<ILogin>().SignOut();
            try
            {
                var nu = DependencyService.Get<ILogin>().AccountName();

            }
            catch
            {

            }
        }
        void Driv3r()
        {
            if (ToolDrive.Text == "")
            {
                return;
            }
            string nu = DependencyService.Get<ILogin>().UseDrive(-1);

        }
        async void OnLoginTest()
        {
            string clientId = null;
            string redirectUri = null;
            if (ToolIn.Text == "")
            {
                return;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    try
                    {
                        //var nu = DependencyService.Get<ILogin>().AccountName();
                        await Navigation.PushAsync(new OAuthNativeFlowPage());

                    }
                    catch
                    {
                        await Navigation.PushAsync(new OAuthNativeFlowPage());
                        //bool wait = DependencyService.Get<ILogin>().SignIn();
                        //try
                        //{
                        // var nu = DependencyService.Get<ILogin>().AccountName();

                        // }
                        // catch { }

                    }
                    break;

                case Device.Android:
                    try { var nu  =  DependencyService.Get<ILogin>().AccountName();

                    }
                    catch
                    {
                        clientId = Constants.AndroidClientId;
                        redirectUri = Constants.AndroidRedirectUrl;
                       bool wait = DependencyService.Get<ILogin>().SignIn();
                        try { var nu = DependencyService.Get<ILogin>().AccountName();

                        }
                        catch { }

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
        private async System.Threading.Tasks.Task ChangeLang() {
            MessagingCenter.Unsubscribe<LangPop>(this, "Done");
            MessagingCenter.Subscribe<LangPop>(this, "Done", (sender) => {
                Bttn1.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                Bttn2.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                Bttn3.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                Bttn4.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Pricings");
            });
               await PopupNavigation.Instance.PushAsync(LangPop.GetInstance());
        }

        private void ToolDown_Clicked(object sender, EventArgs e)
        {
            if (ToolDown.Text == "") {
                return;
            }
            var nu = DependencyService.Get<ILogin>().Download(-1);
        }

        private void boffo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (boffo.Text == "Finished" && (bool)Application.Current.Properties["Load"]) {
                SaveAll.GetInstance().LoadAll();
            } else if((bool)Application.Current.Properties["Signed"]) {
                ToolDrive.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Upload");
                ToolDown.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Download");
                Toolout.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SignOut");
                ToolIn.Text = "";
            }
            else if (!(bool)Xamarin.Forms.Application.Current.Properties["Signed"])
            {
                ToolDrive.Text = "";
                ToolDown.Text = "";
                Toolout.Text = "";
                ToolIn.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SignIn");
            }
        }

        private void Tute_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["Tutorial"] = true;
            Device.BeginInvokeOnMainThread(async () =>
            {
               bool res = await DisplayAlert("Tutorial", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TuteMenu0"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                if (res)
                {
                    await DisplayAlert("Signing in to Google", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TuteMenu1"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                }
                else {
                    Application.Current.Properties["Tutorial"] = false;
                }
            });
        }
    }
    class ClockViewModel : INotifyPropertyChanged
    {
        string dateTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClockViewModel()
        {
            DateTime = (string)Application.Current.Properties["Boff"];

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                DateTime = (string)Application.Current.Properties["Boff"];
                return true;
            });
        }

        public string DateTime
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