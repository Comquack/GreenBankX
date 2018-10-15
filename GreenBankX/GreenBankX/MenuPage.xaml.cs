﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using GreenBankX.Resources;
using System.Threading;

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
            string nu = DependencyService.Get<ILogin>().UseDrive(-1);

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
        private void ChangeLang() {
            System.Globalization.CultureInfo userSelectedCulture;
            if (Thread.CurrentThread.CurrentCulture == new System.Globalization.CultureInfo("lo-LA"))
            {
                userSelectedCulture = new System.Globalization.CultureInfo("en-AU");
                Thread.CurrentThread.CurrentCulture = userSelectedCulture;
                Bttn1.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                Bttn2.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                Bttn3.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                Bttn4.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Pricings");
            }
            else
            {
                userSelectedCulture = new System.Globalization.CultureInfo("lo-LA");
                Thread.CurrentThread.CurrentCulture = userSelectedCulture;
                Bttn1.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MeasureTree");
                Bttn2.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                Bttn3.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("ManagePlots");
                Bttn4.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Pricings");
                //Lang.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Upload");
            }
        }

        private void ToolDown_Clicked(object sender, EventArgs e)
        {
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