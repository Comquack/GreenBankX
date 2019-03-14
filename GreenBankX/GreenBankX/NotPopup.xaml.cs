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
using Xamarin.Auth;
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
            }
            else {
                Latent.IsVisible = true;
               double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                Latent.Text = geo[0].ToString()+", "+geo[1].ToString();
            }

        }

        public async void Done()
        {
            if (PlotName.Text != null && int.TryParse(PlotYear.Text, out int yearout)&& yearout <= DateTime.Now.Year)
            {
                double[] geo;
                string[] splitter = Latent.Text.Split(',');
                if (splitter.Count() != 2) {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("WrongCoord"), "OK");
                    });
                    return;
                }
                if (Application.Current.Properties["ThisLocation"] == null && double.TryParse(splitter.ElementAt(0),out double latout) && double.TryParse(splitter.ElementAt(1), out double lonout))
                {
                    if (latout > 90 || latout < -90 || lonout > 180 || lonout <= -180) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OORCoord"), "OK");
                        });
                        return;  
                }
                    geo = new double[] { latout, lonout };
                }
                else if (Application.Current.Properties["ThisLocation"] != null)
                {
                    geo = (double[])Application.Current.Properties["ThisLocation"];
                }
                else {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OORCoord"), "OK");
                    });
                    return; }
                NextPlot = new Plot(PlotName.Text);
                NextPlot.SetTag(geo);
                NextPlot.Describe = Comments.Text;
                NextPlot.NearestTown = Location.Text;
                NextPlot.Owner = Owner.Text;
                if (Application.Current.Properties["Bounds"] != null && ((List<TK.CustomMap.Position>)Application.Current.Properties["Bounds"]).Count > 1) {
                    NextPlot.AddPolygon(((List<TK.CustomMap.Position>)Application.Current.Properties["Bounds"]));
                    Application.Current.Properties["Bounds"] = new List<TK.CustomMap.Position>();
                }
                if (int.Parse(PlotYear.Text) != 0)
                {
                    NextPlot.YearPlanted = yearout;
                }
                for (int i = 0; i < ((List<Plot>)Application.Current.Properties["Plots"]).Count ; i++){
                    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(i).GetName() == PlotName.Text) {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterName"), "OK");
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
                bool res = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddTree2"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("yes"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("no"));
                if (res)
                {
                    Application.Current.Properties["Counter"] = ((List<Plot>)Application.Current.Properties["Plots"]).Count - 1;
                    MessagingCenter.Subscribe<AddTreePop>(this, "Save", async (sender) =>
                    {
                        SaveAll.GetInstance().SaveTrees2();
                        NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot")+": "+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                        await Task.Delay(5000);
                        NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                    });
                    await PopupNavigation.Instance.PushAsync(AddTreePop.GetInstance());
                }
                else {
                    NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot") +": "+ AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Saved");
                    await Task.Delay(5000);
                    NameLabel.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("CreatePlot");
                }

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
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutplot0"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Plot", "This is the Create Plot screen. Here you can create new plots for your trees.", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert("Plot Location", "The location of the plot can be designated either by typing in the co-ordinates, or by tapping Add From Map to bring up a map.", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("More details", "Additional details can be added to the plot by selecting, More Details. This will let you set a boundary for the plot, Name the nearest town, the owner and ass additional comments about the plot", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Set Boundary", "Set Boundary lets you define the boundary of your plot of land. This will be further explained when the option is selected", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Details", "The nearest town can be located automatically by selecting Find Location. Owner's  Name will be automatically filled in if you have logged into Google.", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Adding Trees", "When Add Plot is selected, if the plot is valid, you will be given the option to add trees to the plot. Adding trees will be explained further when it is time to start adding trees", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutplot0"] = false;
                    }
                    else
                    {
                        Application.Current.Properties["Tutplot0"] = false;
                    }
                });
            }
            if (Application.Current.Properties["PriceStore"] == null)
            {

                if (Application.Current.Properties["ThisLocation"] == null)
                {
                    Latent.Text = null;
                    Latent.IsVisible = true;
                }
                else
                {
                    Latent.IsVisible = true;
                    double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                    Latent.Text = geo[0].ToString()+", "+geo[1].ToString();
                }
                PlotName.Text = null;
                Comments.Text = null;
                Location.Text = null;
                Owner.Text = null;
                PlotYear.Text = null;
                Find.IsVisible = false;
                Expand.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MoreDetails");
                Location.IsVisible = false;
                Bound.IsVisible = false;
                Owner.IsVisible = false;
                Comments.IsVisible = false;
            }
            else {
                Expand.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
                Bound.IsVisible = true;
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


        private void Expand_Clicked(object sender, EventArgs e)
        {
            bool X = Expand.Text == AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails");
            Location.IsVisible = !X;
            Bound.IsVisible = !X;
            Owner.IsVisible = !X;
            Comments.IsVisible = !X;
            Find.IsVisible = !X;
            Expand.Text = X ? AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("MoreDetails") : AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("LessDetails"); ;

            if (Xamarin.Forms.Application.Current.Properties["First"] != null && Xamarin.Forms.Application.Current.Properties["Last"] != null) {
                Owner.Text = (string)Xamarin.Forms.Application.Current.Properties["First"] + " " + (string)Xamarin.Forms.Application.Current.Properties["Last"];
            }
        }

        private void Latent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
       

        private async void Button_Clicked( EventArgs e)
        {
            MessagingCenter.Subscribe<CreatePlotPopup>(this, "Add", (sender) =>
            {
                if (Application.Current.Properties["ThisLocation"] == null)
                {
                    Latent.IsVisible = true;
                }
                else
                {
                    Latent.IsVisible = true;
                    double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                    Latent.Text = geo[0].ToString()+", "+ geo[1].ToString();
                }
                if (Application.Current.Properties["User"] != null && ((User)Application.Current.Properties["User"]).Name != null) {
                    Owner.Text = ((User)Application.Current.Properties["User"]).Name;
                }
                
            });
            await PopupNavigation.Instance.PushAsync(new CreatePlotPopup());

        }

        private async void Find_Clicked(object sender, EventArgs e)
        {
            if (Latent.Text == null) {
                return;
            }
           
            double[] geo =new double[] { 0, 0 };
            string[] splitter = Latent.Text.Split(',');
            if (splitter.Count() != 2)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("WrongCoord"), "OK");
                });
                return;
            }
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

        private async void Bound_Clicked(EventArgs e)
        {
            if (Latent.Text == null)
            {
                return;
            }
            double[] geo = new double[] { 0, 0 };
            string[] splitter = Latent.Text.Split(',');
            if (splitter.Count() != 2)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("WrongCoord"), "OK");
                });
                return;
            }
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
                bool res = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary2"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryA"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryB"));
                if (res)
                {
                    await PopupNavigation.Instance.PushAsync(new PopupBound(geo));
                }
                else {
                    await PopupNavigation.Instance.PushAsync(new BoundList(geo));
                }

            }
            else if (Latent.Text != null)
            {
                bool res = await DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundary2"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryA"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetBoundaryB"));
                if (res)
                {
                    await PopupNavigation.Instance.PushAsync(new PopupBound(geo));
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new BoundList(geo)); 
                }
            }
            
        }
    }
}
