using System;
using System.Collections.Generic;
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
    public partial class Popup : Rg.Plugins.Popup.Pages.PopupPage
    {
        Geocoder Geoco;
        Plot NextPlot;
        public static Popup instance = new Popup();
        public static Popup GetInstance()
        {
            if (instance == null)
            {
                return new Popup();
            }
            return instance;
        }

        private Popup()
        {
            InitializeComponent();
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
                for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
            }
            pickPrice.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddPricing"));



        }
        public async void Done()
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
                else { return; }
                NextPlot = new Plot(PlotName.Text);
                NextPlot.SetTag(geo);
                NextPlot.Describe = Comments.Text;
                NextPlot.NearestTown = Location.Text;
                NextPlot.Owner = Owner.Text;
                if (pickPrice.SelectedIndex > -1 && pickPrice.SelectedIndex < pickPrice.Items.Count-1)
                {
                    NextPlot.SetRange(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                }
                else if (pickPrice.SelectedIndex == pickPrice.Items.Count)
                {
                    await Navigation.PushAsync(new CreatePricing());
                    return;
                }
                if (int.Parse(PlotYear.Text) != 0)
                {
                    NextPlot.YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count ; i++){
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text) {
                        NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName");
                        return;
                    }
                }
                ((List<Plot>)Application.Current.Properties["Plots"]).Add(NextPlot);
                MessagingCenter.Send<Popup>(this, "Add");
                SaveAll.GetInstance().SavePlots();
                await PopupNavigation.Instance.PopAsync();
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
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }

        private async Task pickPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickPrice.SelectedIndex == pickPrice.Items.Count-1)
            {
                await Navigation.PushAsync(new CreatePricing());
                return;
            }
        }

        private async void Expand_Clicked(object sender, EventArgs e)
        {
            double[] geo;
            bool X = Expand.Text == "Less Details";
            pickPrice.IsVisible = !X;
            Location.IsVisible = !X;
            Owner.IsVisible = !X;
            Comments.IsVisible = !X;
            Expand.Text = X? "Add More Detail" :"Less Details";
            if (!X) {
                if (Application.Current.Properties["ThisLocation"] != null)
                {     
                    geo = (double[])Application.Current.Properties["ThisLocation"];
                    Geoco = new Geocoder();
                    try
                    {
                        var answ = await Geoco.GetAddressesForPositionAsync(new Position(geo[0], geo[1]));
                        Location.Text = answ.First();
                    }
                    catch { Location.Text = "Error"; }
                }
                else if (Latent.Text != null && Longent.Text != null) {
                    geo = new double[] { double.Parse(Latent.Text), double.Parse(Longent.Text) };
                    Geoco = new Geocoder();
                    try
                    {
                        var answ = await Geoco.GetAddressesForPositionAsync(new Position(geo[0], geo[1]));
                        Location.Text = answ.First();
                    }
                    catch { Location.Text = "Error"; }
                }  
            }
            if (Xamarin.Forms.Application.Current.Properties["First"] != null && Xamarin.Forms.Application.Current.Properties["Last"] != null) {
                Owner.Text = (string)Xamarin.Forms.Application.Current.Properties["First"] + " " + (string)Xamarin.Forms.Application.Current.Properties["Last"];
            }
        }

        private void Latent_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue != null&&!double.TryParse(e.NewTextValue, out ans)) { }
           else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 90 || double.Parse(e.NewTextValue) < -90))
            {
                Latent.Text = e.OldTextValue;
            }
        }
        private void Longent_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 180 || double.Parse(e.NewTextValue) <= -180))
            {
                Longent.Text = e.OldTextValue;
            }
        }
    }
}
