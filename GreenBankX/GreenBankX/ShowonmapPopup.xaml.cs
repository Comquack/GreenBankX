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
	public partial class ShowonmapPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        List<Position> newpolygon;
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>();
        int setpoly = -1;
        public ShowonmapPopup()
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            InitializeComponent ();
            }
 
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
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
                int num = ((List<Plot>)Application.Current.Properties["PlotsOnMap"]).Count();
                for (int x=0; x < num; x++) {
                    double[] pinLoc = ((List<Plot>)Application.Current.Properties["PlotsOnMap"]).ElementAt(x).GetTag();
                    Position position = new Position(pinLoc[0], pinLoc[1]);
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = ((List<Plot>)Application.Current.Properties["PlotsOnMap"]).ElementAt(x).GetName(),
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
        {
            e.Value.ShowCallout = true;
           
        }
        //renders polygons(plot area)
        public void PolyMap() {
            List<TKPolygon> polylist = new List<TKPolygon>();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["PlotsOnMap"]).Count(); x++) {
                if (((List<Plot>)Application.Current.Properties["PlotsOnMap"]).ElementAt(x).GetPolygon().Count > 0)
                {
                    TKPolygon newpoly = new TKPolygon
                    {
                        Coordinates = ((List<Plot>)Application.Current.Properties["PlotsOnMap"]).ElementAt(x).GetPolygon(),
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
            StartMap(false);
            PolyMap();

            await PopupNavigation.Instance.PopAsync();

        }
        public void SavePlots() {
            SaveAll.GetInstance().SavePlots();
        }

 
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
                if (width < height)
                {
                CancelButton.WidthRequest = height / 5;
            }
                else
                {

                CancelButton.WidthRequest = width / 5;

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