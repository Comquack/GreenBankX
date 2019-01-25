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
                //Longent.IsVisible = true;
            PlotName.Text = EditPlot.GetName();
            Comments.Text = EditPlot.Describe;
            Location.Text = EditPlot.NearestTown;
            PlotYear.Text = EditPlot.YearPlanted.ToString();
               double[] geo = EditPlot.GetTag();
                Latent.Text = geo[0].ToString()+", "+ geo[1].ToString();
                //Longent.Text = geo[1].ToString();
                Owner.Text = EditPlot.Owner;
        }

        public async void Done()
        {
            if (PlotName.Text != null && int.TryParse(PlotYear.Text, out int yearout) && yearout <= DateTime.Now.Year)
            {
                double[] geo;
                string[] splitter = Latent.Text.Split(',');
                if (splitter.Count() != 2)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), "Please add coodinates in Lat, Long form", "OK");
                    });
                    return;
                }
                if (Application.Current.Properties["ThisLocation"] == null && double.TryParse(splitter.ElementAt(0), out double latout) && double.TryParse(splitter.ElementAt(1), out double lonout))
                {
                    if (latout > 90 || latout < -90 || lonout > 180 || lonout <= -180)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), "Please enter valid co-ordinates", "OK");
                        });
                        return;
                    }
                    geo = new double[] { latout, lonout };
                }
                else if (Application.Current.Properties["ThisLocation"] != null)
                {
                    geo = (double[])Application.Current.Properties["ThisLocation"];
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), "Please enter valid co-ordinates", "OK");
                    });
                    return;
                }
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetName(PlotName.Text);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetTag(geo);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Describe = Comments.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).NearestTown = Location.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Owner = Owner.Text;

                if (int.Parse(PlotYear.Text) != 0)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count; i++)
                {
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text && i != (int)Application.Current.Properties["ThisPlot"])
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
                        });
                        return;
                    }
                }
                SaveAll.GetInstance().SavePlots();
                Application.Current.Properties["ThisLocation"] = null;
                PlotName.Text = null;
                Comments.Text = null;
                Location.Text = null;
                Owner.Text = null;
                Latent.Text = null;
                        SaveAll.GetInstance().SaveTrees2();
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
        public async void Done2()
        {
            if (PlotName.Text != null && int.TryParse(PlotYear.Text, out int yearout)&& yearout <= DateTime.Now.Year)
            {
                double[] geo;
                if (double.TryParse(Latent.Text,out double latout) && double.TryParse(Longents.Text, out double lonout))
                {
                    geo = new double[] { latout, lonout };
                }
                else { return; }
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetName(PlotName.Text);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).SetTag(geo);
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Describe = Comments.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).NearestTown = Location.Text;
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).Owner = Owner.Text;

                if (int.Parse(PlotYear.Text) != 0)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]).YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count ; i++){
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text&&i!= (int)Application.Current.Properties["ThisPlot"]) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
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
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
                });
            }
            else
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
           
        }



        protected override void OnAppearing()
        {
            Find.IsVisible = false;
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            EditPlot = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt((int)Application.Current.Properties["ThisPlot"]);
            Latent.IsVisible = true;
            //Longent.IsVisible = true;
            PlotName.Text = EditPlot.GetName();
            Comments.Text = EditPlot.Describe;
            Location.Text = EditPlot.NearestTown;
            PlotYear.Text = EditPlot.YearPlanted.ToString();
            double[] geo = EditPlot.GetTag();
            Latent.Text = geo[0].ToString() + ", " + geo[1].ToString();
            //Longent.Text = geo[1].ToString();
            Owner.Text = EditPlot.Owner;
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



        private  void Expand_Clicked(object sender, EventArgs e)
        {
            bool X = Expand.Text == AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
            ButtBound.IsVisible = !X;
            Location.IsVisible = !X;
            Owner.IsVisible = !X;
            Find.IsVisible = !X;
            Comments.IsVisible = !X;
            Expand.Text = X? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MoreDetails") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
            if (Xamarin.Forms.Application.Current.Properties["First"] != null && Xamarin.Forms.Application.Current.Properties["Last"] != null) {
                Owner.Text = (string)Xamarin.Forms.Application.Current.Properties["First"] + " " + (string)Xamarin.Forms.Application.Current.Properties["Last"];
            }
        }

        private void Latent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Longent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private async void Find_Clicked(object sender, EventArgs e)
        {

            if (Latent.Text == null)
            {
                return;
            }
            double[] geo = new double[] { 0, 0 };
            string[] splitter = Latent.Text.Split(',');
            if (Application.Current.Properties["ThisLocation"] == null && double.TryParse(splitter.ElementAt(0), out double latout) && double.TryParse(splitter.ElementAt(1), out double lonout))
            {
                if (latout > 90 || latout < -90 || lonout > 180 || lonout <= -180)
                {
                    return;
                }
                geo = new double[] { latout, lonout };
            }

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
                catch
                {
                    Location.Text = "Error";
                    Location.IsEnabled = true;
                }
            }
            else if (Latent.Text != null)
            {
                if (Latent.Text == null)
                {
                    return;
                }
                Geoco = new Geocoder();
                Location.IsEnabled = false;
                try
                {
                    var answ = await Geoco.GetAddressesForPositionAsync(new Position(geo[0], geo[1]));
                    Location.Text = answ.First();
                    Location.IsEnabled = true;
                }
                catch
                {
                    Location.Text = "Error";
                    Location.IsEnabled = true;
                }

            }
        }

        private async void ButtBound_Clicked(object sender, EventArgs e)
        {
           int geol = (int)Application.Current.Properties["ThisPlot"];
            bool res = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary2"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryA"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryB"));
            if (res)
            {
                await PopupNavigation.Instance.PushAsync(new PopupBound(geol));
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new BoundList(geol));
            }
            await PopupNavigation.Instance.PushAsync(new PopupBound((int)Application.Current.Properties["ThisPlot"]));

        }
    }
}

