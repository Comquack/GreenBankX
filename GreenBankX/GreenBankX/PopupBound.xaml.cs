using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TK.CustomMap;
using Rg.Plugins.Popup.Services;
using TK.CustomMap.Overlays;
using TK.CustomMap.Api.Google;
using GreenBankX.Resources;
using System.Threading;
using System.Globalization;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PopupBound : Rg.Plugins.Popup.Pages.PopupPage
    {
        List<double[]> allpoints = new List<double[]>();
        double[] geo;
        GeolocationRequest request;
        Boolean CanAdd = true;
        List<Position> newpolygon=new List<Position>();
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>();
        int setpoly = -1;
        public PopupBound(double[] geol)
        {
            Application.Current.Properties["Bounds"] = new List<TK.CustomMap.Position>();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            Application.Current.Properties["Bounds"]=new List<TK.CustomMap.Position>();
            geo = geol;
            setpoly = -1;
            InitializeComponent ();
            AddPlot.IsVisible=false;
        }
        public PopupBound(int geol)
        {
            Application.Current.Properties["Bounds"] = new List<TK.CustomMap.Position>();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            Application.Current.Properties["Bounds"] = new List<TK.CustomMap.Position>();
            geo = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(geol).GetTag();
            setpoly = geol;
            InitializeComponent();
            AddPlot.IsVisible = false;
        }
        public async Task NewPlot() {
                List<double> lat = new List<double>();
                List<double> lon = new List<double>();
            if (newpolygon.Count() < 3)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Not enough points", "Boundary requires at least 3 points.", "OK");
                });
                return;
            }

            for (int x = 0; x < newpolygon.Count; x++)
                {
                    lat.Add(newpolygon.ElementAt(x).Latitude);
                    lon.Add(newpolygon.ElementAt(x).Longitude);
                ((List<TK.CustomMap.Position>)Application.Current.Properties["Bounds"]).Add(newpolygon.ElementAt(x));
                } 
                if (geo[0] > lat.Min() && geo[1] > lon.Min() && geo[0] < lat.Max() && geo[1] < lon.Max())
                {
                if (setpoly > -1) {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly).AddPolygon(((List<TK.CustomMap.Position>)Application.Current.Properties["Bounds"]));
                }
                MessagingCenter.Send<PopupBound>(this, "Add");
                await PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Apin"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Apin"), "OK");
                    });
                }


        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
        }
    
        public void PlacePin(object sender, TKGenericEventArgs<Position> e)
        {
                MyMap.Pins = new List<TKCustomMapPin>();
                showName.IsVisible = false;
                Pins.Add(new TKCustomMapPin
                {
                    Position = e.Value,
                    Title = "Area",
                    IsVisible = true,
                    ShowCallout = false,
                    DefaultPinColor = Color.Green

                });
                newpolygon.Add(e.Value);
            AddPlot.IsVisible = true;
                MyMap.Pins = Pins;
            CancelButton.IsVisible = true; ;
        }
        public void MapReady() {
            if (((bool)Application.Current.Properties["Tutorial"]) && (bool)Application.Current.Properties["Tutplot"])
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool res = await DisplayAlert("Plot", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePlot0"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Continue"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Skip"));
                    if (res)
                    {
                        await DisplayAlert("Plot", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePlot1"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        await DisplayAlert("Plot", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TutePlot2"),AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Next"));
                        Application.Current.Properties["Tutplot"] = false;
                    }
                    else
                    {
                        Application.Current.Properties["Tutplot"] = false;
                    }
                });
            }
            StartMap(true);
            PolyMap();
        }
            public async void StartMap(bool first) {
            //renders map, centres on user, creates pins from plots
            try
            {
   
                Pins = new List<TKCustomMapPin>();
                int num = ((List<Plot>)Application.Current.Properties["Plots"]).Count();
                    double[] pinLoc = geo;
                    Position position = new Position(pinLoc[0], pinLoc[1]);
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = "New Plot",
                        IsVisible = true,
                        ShowCallout = true
                        
                    });
                    
                
                MyMap.Pins = Pins;
                if (first)
                {
                    request = new GeolocationRequest(GeolocationAccuracy.High);
                    var location = new Position(pinLoc[0], pinLoc[1]); ;

                    if (location != null)
                    {
                        MyMap.MoveToMapRegion(
                            MapSpan.FromCenterAndRadius(
                            new Position(location.Latitude, location.Longitude), Distance.FromKilometers(1)));
                    }
                }
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("map load error", "map load error", "OK");
                });
            }
        }

        private void MyMap_PinSelected(object sender, TKGenericEventArgs<TKCustomMapPin> e)
        { if (e.Value.Title == "New Plot")
            {
                return;
            }
            else if ((e.Value.Title == "Area"))
            {
                Pins.Remove(e.Value);
                newpolygon.Remove(e.Value.Position);
                MyMap.Pins = Pins;
                //StartMap(false);
                //PolyMap();
            }
            else
            {
            }
        }
        //renders polygons(plot area)
        public void PolyMap() {
            //List<TKPolygon> polylist = new List<TKPolygon>();
            //for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
            //    if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon().Count > 0)
            //    {
            //        TKPolygon newpoly = new TKPolygon
            //        {
            //            Coordinates = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon(),
            //            Color = Color.LightGray,
            //            StrokeColor = Color.LightGreen
            //        };
            //        polylist.Add(newpoly);
            //    }
            //        }

            //MyMap.Polygons = polylist;
        }
        //unselects pin
        async public void Cancel() {
            await PopupNavigation.Instance.PopAsync();

        }
        public void SavePlots() {
            SaveAll.GetInstance().SavePlots();
        }

        public async void PinHere()
        { 
            Position position = new Position();
            try
            {
                var location = await Geolocation.GetLocationAsync(request);
                
                if (location != null)
                {
                    
                    position = new Position(location.Latitude, location.Longitude);
                }

                //If setpoly not = -1 (pin selected) multiple pins can be placed to form polygon

                    MyMap.Pins = new List<TKCustomMapPin>();
                    showName.IsVisible = false;
                    
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = "TestPlot",
                        IsVisible = true,
                        ShowCallout = false,
                        DefaultPinColor = Color.Green

                    });
                    newpolygon.Add(position);

                    MyMap.Pins = Pins;
                CancelButton.IsVisible = true; ;
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("pin here error", "pin here error", "OK");
                });
            }
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
                if (width < height)
                {
                AtLocationButton.WidthRequest = height / 5;
                CancelButton.WidthRequest = height / 5;
                AddPlot.WidthRequest = height / 5;

            }
                else
                {
                AtLocationButton.WidthRequest = width / 5;
                CancelButton.WidthRequest = width / 5;
                AddPlot.WidthRequest = width / 5;

            }

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

    }
}