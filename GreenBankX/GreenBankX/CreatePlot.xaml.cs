﻿using System;
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

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePlot : ContentPage
	{
        Boolean CanAdd = true;
        List<Position> newpolygon;
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>();
        int setpoly = -1;
        public CreatePlot ()
		{
            InitializeComponent ();
            CancelButton.IsVisible = false;
            }
        public async Task NewPlot() {
            //If setpoly not = -1 (pin selected) converts a set of placed pins into a polygon 
           
            if (setpoly > -1) {
                List<double> lat = new List<double>();
                List<double> lon = new List<double>();
                for (int x = 0; x < newpolygon.Count; x++)
                {
                    lat.Add(newpolygon.ElementAt(x).Latitude);
                    lon.Add(newpolygon.ElementAt(x).Longitude);
                }
                Plot thisPLot = (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly));
                if (thisPLot.GetTag()[0] > lat.Min() && thisPLot.GetTag()[1] > lon.Min() && thisPLot.GetTag()[0] < lat.Max() && thisPLot.GetTag()[1] < lon.Max()) {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly).AddPolygon(newpolygon);
                    SaveAll.GetInstance().SavePlots();
                }
                else {
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
            if (!CanAdd)
            {
                double[] geo = new double[2];
                Application.Current.Properties["ThisLocation"] = new double[2];
                geo[0] = Pins.ElementAt(Pins.Count - 1).Position.Latitude;
                geo[1] = Pins.ElementAt(Pins.Count - 1).Position.Longitude;
                Application.Current.Properties["ThisLocation"] = geo;
                MessagingCenter.Subscribe<Popup>(this, "Add", (sender) =>
                {
                CanAdd = true;
                StartMap(false);
                PolyMap();
                });
                    await PopupNavigation.Instance.PushAsync(Popup.GetInstance());

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
                Pins.Add(new TKCustomMapPin
                {
                    Position = e.Value,
                    Title = "TestPlot",
                    IsVisible = true,
                    ShowCallout = false,
                    
                });
                MyMap.Pins = Pins;
                CanAdd = false;
            }
            CancelButton.IsVisible = true; ;
        }
        public void MapReady() {
            StartMap(true);
            PolyMap();
        }
            public async void StartMap(bool first) {
            //renders map, centres on user, creates pins from plots
            try
            {
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
            if (e.Value.Title == "TestPlot")
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
                        }
                        else
                        {  if (!CanAdd){
                                Pins.RemoveAt(Pins.Count - 1);
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
        public void Cancel() {
            if (setpoly > -1) {
                        MyMap.Pins.ElementAt(setpoly).DefaultPinColor = Color.Red;
                        setpoly = -1;
                        CancelButton.IsVisible = false;
                        newpolygon = new List<Position>();
                        StartMap(false);
                        PolyMap();
                        
                        MyMap.SelectedPin = null;
                        return;

            }
            StartMap(false);
            PolyMap();
            CancelButton.IsVisible = false;
            CancelButton.Text = "Add Plot";
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
    }
}