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
    public partial class PlotPopupEdit : Rg.Plugins.Popup.Pages.PopupPage
    {
        Geocoder Geoco;
        Plot EditPlot;
        int Priceno = -1;
        public static PlotPopupEdit instance = new PlotPopupEdit();
        public static PlotPopupEdit GetInstance()
        {
            if (instance == null)
            {
                return new PlotPopupEdit();
            }
            return instance;
        }

        private PlotPopupEdit()
        {
            InitializeComponent();
            EditPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]);
                Latent.IsVisible = true;
                Longent.IsVisible = true;
            PlotName.Text = EditPlot.GetName();
            Comments.Text = EditPlot.Describe;
            Location.Text = EditPlot.NearestTown;
               double[] geo = EditPlot.GetTag();
                Latent.Text = geo[0].ToString();
                Longent.Text = geo[1].ToString();
                Owner.Text = EditPlot.Owner;
                for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
                if(EditPlot.GetRange()!= null && ((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName() == EditPlot.GetRange().GetName()){
                    Priceno = x;
                }
            }
            pickPrice.Items.Add(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddPricing"));
            pickPrice.SelectedIndex = Priceno;
        }

        public async void Done()
        {
            if (PlotName.Text != null && int.TryParse(PlotYear.Text, out int yearout)&& yearout <= DateTime.Now.Year)
            {
                double[] geo;
                if (double.TryParse(Latent.Text,out double latout) && double.TryParse(Longent.Text, out double lonout))
                {
                    geo = new double[] { latout, lonout };
                }
                else { return; }
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetName(PlotName.Text);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetTag(geo);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Describe = Comments.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).NearestTown = Location.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Owner = Owner.Text;
                if (pickPrice.SelectedIndex > -1 && pickPrice.SelectedIndex < pickPrice.Items.Count-1)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetRange(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                }
                else if (pickPrice.SelectedIndex == pickPrice.Items.Count)
                {
                    await Navigation.PushAsync(new CreatePricing());
                    return;
                }
                if (int.Parse(PlotYear.Text) != 0)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count ; i++){
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text&&i!= (int)Application.Current.Properties["ThisPlot"]) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("input is invalid", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
                        });
                        return;
                    }
                }
                MessagingCenter.Send<PlotPopupEdit>(this, "Edit");
                SaveAll.GetInstance().SavePlots();
                await PopupNavigation.Instance.PopAsync();
            }
            else if (PlotName.Text == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("input is invalid", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
                });
            }
            else
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("input is invalid", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
           
        }



        protected override void OnAppearing()
        {
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
