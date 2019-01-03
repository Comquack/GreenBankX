using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    public partial class NotPopup : ContentPage
    {
        Geocoder Geoco;
        Plot NextPlot;

        public NotPopup()
        {
            InitializeComponent();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            if (Application.Current.Properties["ThisLocation"] == null)
            {
                Latent.IsVisible = true;
                Longent.IsVisible = true;
            }
            else {
                Latent.IsVisible = true;
                Longent.IsVisible = true;
               double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                Latent.Text = geo[0].ToString();
                Longent.Text = geo[1].ToString();
            }

        }
        public void Done()
        {
            if (PlotName.Text != null && int.TryParse(PlotYear.Text, out int yearout)&& yearout <= DateTime.Now.Year)
            {
                double[] geo;
                if (Application.Current.Properties["ThisLocation"] == null && double.TryParse(Latent.Text,out double latout) && double.TryParse(Longent.Text, out double lonout))
                {
                    geo = new double[] { latout, lonout };
                }
                else if (Application.Current.Properties["ThisLocation"] != null)
                {
                    geo = (double[])Application.Current.Properties["ThisLocation"];
                }
                else {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("input is invalid", "Please enter co-ordinates", "OK");
                    });
                    return; }
                NextPlot = new Plot(PlotName.Text);
                NextPlot.SetTag(geo);
                NextPlot.Describe = Comments.Text;
                NextPlot.NearestTown = Location.Text;
                NextPlot.Owner = Owner.Text;
                if (int.Parse(PlotYear.Text) != 0)
                {
                    NextPlot.YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count ; i++){
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("input is invalid", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
                        });
                        return;
                    }
                }
                ((List<Plot>)Application.Current.Properties["Plots"]).Add(NextPlot);
                SaveAll.GetInstance().SavePlots();
                Application.Current.Properties["ThisLocation"] = null;
                PlotName.Text=null;
                Comments.Text=null;
                Location.Text=null;
                Owner.Text=null;
                Latent.Text = null;
                Longent.Text = null;
                SaveAll.GetInstance().SavePlots();
            }
            else if (PlotName.Text == null)
            {
                NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName");
            }
            else
            {
                NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate");
            }
           
        }



        protected override void OnAppearing()
        {

            if (Application.Current.Properties["PriceStore"] == null)
            {

                if (Application.Current.Properties["ThisLocation"] == null)
                {
                    Latent.Text = null;
                    Longent.Text = null;
                    Latent.IsVisible = true;
                    Longent.IsVisible = true;
                }
                else
                {
                    Latent.IsVisible = true;
                    Longent.IsVisible = true;
                    double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                    Latent.Text = geo[0].ToString();
                    Longent.Text = geo[1].ToString();
                }
                PlotName.Text = null;
                Comments.Text = null;
                Location.Text = null;
                Owner.Text = null;
                PlotYear.Text = null;

                Expand.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MoreDetails");
                Location.IsVisible = false;
                Owner.IsVisible = false;
                Comments.IsVisible = false;
            }
            else {
                Expand.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
                Location.IsVisible = true;
                Owner.IsVisible = true;
                Comments.IsVisible = true;
            };
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {

            base.OnDisappearing();

        }

        // ### Methods for supporting animations in your popup page ###

       

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }


        private async void Expand_Clicked(object sender, EventArgs e)
        {
            double[] geo;
            bool X = Expand.Text == AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
            Location.IsVisible = !X;
            Owner.IsVisible = !X;
            Comments.IsVisible = !X;
            Expand.Text = X? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MoreDetails") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails"); ;
            if (!X) {
                if (Application.Current.Properties["ThisLocation"] != null)
                {     
                    geo = (double[])Application.Current.Properties["ThisLocation"];
                    Geoco = new Geocoder();
                    Location.IsEnabled = false;
                    try
                    {
                        var answ = await Geoco.GetAddressesForPositionAsync(new Position(geo[0], geo[1]));
                        Location.Text = answ.First();
                        Location.IsEnabled = true;
                    }
                    catch { Location.Text = "Error";
                        Location.IsEnabled = true;
                    }
                }
                else if (Latent.Text != null && Longent.Text != null) {
                    geo = new double[] { double.Parse(Latent.Text), double.Parse(Longent.Text) };
                    Geoco = new Geocoder();
                    Location.IsEnabled = false;
                    try
                    {
                        var answ = await Geoco.GetAddressesForPositionAsync(new Position(geo[0], geo[1]));
                        Location.Text = answ.First();
                        Location.IsEnabled = true;
                    }
                    catch { Location.Text = "Error";
                        Location.IsEnabled = true;
                    }
                }  
            }
            if (Xamarin.Forms.Application.Current.Properties["First"] != null && Xamarin.Forms.Application.Current.Properties["Last"] != null) {
                Owner.Text = (string)Xamarin.Forms.Application.Current.Properties["First"] + " " + (string)Xamarin.Forms.Application.Current.Properties["Last"];
            }
        }

        private void Latent_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue != null&&!double.TryParse(e.NewTextValue, out ans)) { Application.Current.Properties["ThisLocation"] = null; }
           else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 90 || double.Parse(e.NewTextValue) < -90))
            {
                Latent.Text = e.OldTextValue;
            }
            Application.Current.Properties["ThisLocation"] = null;
        }
        private void Longent_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out ans)) { Application.Current.Properties["ThisLocation"] = null; }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 180 || double.Parse(e.NewTextValue) <= -180))
            {
                Longent.Text = e.OldTextValue;
            }
            Application.Current.Properties["ThisLocation"] = null;
        }

        private async void Button_Clicked( EventArgs e)
        {
            MessagingCenter.Subscribe<CreatePlotPopup>(this, "Add", (sender) =>
            {
                if (Application.Current.Properties["ThisLocation"] == null)
                {
                    Latent.IsVisible = true;
                    Longent.IsVisible = true;
                }
                else
                {
                    Latent.IsVisible = true;
                    Longent.IsVisible = true;
                    double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                    Latent.Text = geo[0].ToString();
                    Longent.Text = geo[1].ToString();
                }
                
            });
            await PopupNavigation.Instance.PushAsync(new CreatePlotPopup());

        }
    }
}
