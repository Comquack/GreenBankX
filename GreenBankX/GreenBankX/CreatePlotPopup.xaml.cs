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
	public partial class CreatePlotPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        Boolean CanAdd = true;
        List<Position> newpolygon;
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>();
        int setpoly = -1;
        public CreatePlotPopup()
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            InitializeComponent ();
            AddPlot.IsVisible=false;
            }
        public async Task NewPlot() {
            //If setpoly not = -1 (pin selected) converts a set of placed pins into a polygon 

            if (setpoly > -1)
            {
                List<double> lat = new List<double>();
                List<double> lon = new List<double>();
                for (int x = 0; x < newpolygon.Count; x++)
                {
                    lat.Add(newpolygon.ElementAt(x).Latitude);
                    lon.Add(newpolygon.ElementAt(x).Longitude);
                }
                Plot thisPLot = (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly));
                if (thisPLot.GetTag()[0] > lat.Min() && thisPLot.GetTag()[1] > lon.Min() && thisPLot.GetTag()[0] < lat.Max() && thisPLot.GetTag()[1] < lon.Max())
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly).AddPolygon(newpolygon);
                    SaveAll.GetInstance().SavePlots();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Apin"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Apin"), "OK");
                    });
                }

                setpoly = -1;
                StartMap(false);
                PolyMap();
                return;
            }
            // setpoly = -1 (no pin selected) adds placed pin into list of plots
            else if (!CanAdd)
            {
                double[] geo = new double[2];
                Application.Current.Properties["ThisLocation"] = new double[2];
                geo[0] = Pins.ElementAt(Pins.Count - 1).Position.Latitude;
                geo[1] = Pins.ElementAt(Pins.Count - 1).Position.Longitude;
                Application.Current.Properties["ThisLocation"] = geo;
                MessagingCenter.Send<CreatePlotPopup>(this, "Add");
                await PopupNavigation.Instance.PopAsync();
            }
            else {
                Application.Current.Properties["ThisLocation"] = null;
                //MessagingCenter.Subscribe<Popup>(this, "Add", (sender) =>
                //{
                //    CanAdd = true;
                //    StartMap(false);
                //    PolyMap();
                //    SaveAll.GetInstance().SavePlots();
                //});
                //await PopupNavigation.Instance.PushAsync(Popup.GetInstance());
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
        
            //If setpoly not = -1 (pin selected) multiple pins can be placed to form polygon
            if (setpoly > -1)
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

                MyMap.Pins = Pins;
            }
            else
            {
                // setpoly = -1 (no pin selected) one pin can be placed at a time
                if (!CanAdd)
                {
                    Pins.RemoveAt(Pins.Count - 1);
                }
                MyMap.Pins = new List<TKCustomMapPin>();
                showName.IsVisible = false;
                AddPlot.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddPlot");
                Pins.Add(new TKCustomMapPin
                {
                    Position = e.Value,
                    Title = "TestPlot",
                    IsVisible = true,
                    ShowCallout = false,
                    
                });
                AddPlot.IsVisible = true;
                MyMap.Pins = Pins;
                CanAdd = false;
            }
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
                for (int x=0; x < num; x++) {
                    double[] pinLoc = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetTag();
                    Position position = new Position(pinLoc[0], pinLoc[1]);
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName(),
                        IsVisible = true,
                        ShowCallout = true
                        
                    });
                }
                MyMap.Pins = Pins;
                if (first)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High);
                    var location = await Geolocation.GetLocationAsync(request);

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
        { if (e.Value.Title == "TestPlot")
            {
                return;
            }
            else if ((e.Value.Title == "Area"))
            {
                Pins.Remove(e.Value);
                newpolygon.Remove(e.Value.Position);
                StartMap(false);
                PolyMap();
            }
            else
            {
                for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count; x++)
                {
                    if (e.Value.Title == ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName())
                    {
                        if (setpoly > -1)
                        {
                            Cancel();
                            return;
                        }
                        else
                        {  if (!CanAdd){
                                Pins.RemoveAt(Pins.Count - 1);
                                return;
                            }
                            CanAdd = true;
                            e.Value.DefaultPinColor = Color.Aqua;
                            setpoly = x;
                            CancelButton.IsVisible = true;
                            MyMap.Pins = new List<TKCustomMapPin>();
                            MyMap.Pins = Pins;

                        }
                    }
                }
                AddPlot.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("SetArea");
                newpolygon = new List<Position>();
            }
        }
        //renders polygons(plot area)
        public void PolyMap() {
            List<TKPolygon> polylist = new List<TKPolygon>();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon().Count > 0)
                {
                    TKPolygon newpoly = new TKPolygon
                    {
                        Coordinates = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon(),
                        Color = Color.LightGray,
                        StrokeColor = Color.LightGreen
                    };
                    polylist.Add(newpoly);
                }
                    }

            MyMap.Polygons = polylist;
        }
        //unselects pin
        async public void Cancel() {
            if (setpoly > -1) {
                        //MyMap.Pins.ElementAt(setpoly).DefaultPinColor = Color.Red;
                        setpoly = -1;
                        newpolygon = new List<Position>();
                        StartMap(false);
                        PolyMap();
                        MyMap.SelectedPin = null;
                
                return;
            }
            setpoly = -1;
            CanAdd = true;
            StartMap(false);
            PolyMap();

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
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);
                
                if (location != null)
                {
                    
                    position = new Position(location.Latitude, location.Longitude);
                }

                //If setpoly not = -1 (pin selected) multiple pins can be placed to form polygon
                if (setpoly > -1)
                {
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
                }
                else
                {
                    // setpoly = -1 (no pin selected) one pin can be placed at a time
                    if (!CanAdd) {
                        Pins.RemoveAt(Pins.Count - 1);
                    }
                    MyMap.Pins = new List<TKCustomMapPin>();
                    showName.IsVisible = false;
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = "TestPlot",
                        IsVisible = true,
                        ShowCallout = false,

                    });
                    MyMap.Pins = Pins;
                    CanAdd = false;
                }
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