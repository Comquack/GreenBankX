using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TK.CustomMap;
using Rg.Plugins.Popup.Services;
using TK.CustomMap.Overlays;
using Syncfusion.XlsIO;
using System.IO;

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
            }
        public async Task NewPlot() {
            //If setpoly not = -1 (pin selected) converts a set of placed pins into a polygon 
           
            if (setpoly > -1) {
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(setpoly).AddPolygon(newpolygon);
                setpoly = -1;
                AddPlot.Text = "New Plot";
                StartMap();
                PolyMap();
                return;
            }
            // setpoly = -1 (no pin selected) adds placed pin into list of plots
            if (CanAdd == false)
            {
                double[] geo = new double[2];
                Application.Current.Properties["ThisLocation"] = new double[2];
                geo[0] = Pins.ElementAt(Pins.Count - 1).Position.Latitude;
                geo[1] = Pins.ElementAt(Pins.Count - 1).Position.Longitude;
                Application.Current.Properties["ThisLocation"] = geo;
                await PopupNavigation.PushAsync(new Popup());
                CanAdd = true;
                StartMap();
                PolyMap();
            }
  
        }
        public void PlacePin(Position position)
        {
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
                if (CanAdd)
                {

                }
                else
                {
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
        }
        public void MapReady() {
            StartMap();
            PolyMap();
        }
            public async void StartMap() {
            //renders map, centres on user, creates pins from plots
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    MyMap.MoveToMapRegion(
                        MapSpan.FromCenterAndRadius(
                        new Position(location.Latitude, location.Longitude), Distance.FromKilometers(1)));
                }
                Pins = new List<TKCustomMapPin>();
                MyMap.Pins = Pins;
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
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        private void MyMap_PinSelected(object sender, TKGenericEventArgs<TKCustomMapPin> e)
        {
            if (e.Value.Title == "TestPlot") {
                return;
            }
            
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count; x++) {
               if (e.Value.Title == ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetName())
               {
                    e.Value.DefaultPinColor = Color.Aqua;
                    setpoly = x;
                    CancelButton.IsVisible = true;
                }
            }
            AddPlot.Text = "Set Area";
            newpolygon = new List<Position>();
        }
        //renders polygons(plot area)
        public void PolyMap() {
            List<TKPolygon> polylist = new List<TKPolygon>();
            for (int x = 0; x < ((List<Plot>)Application.Current.Properties["Plots"]).Count(); x++) {
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon().Count > 0)
                {
                    TKPolygon newpoly = new TKPolygon();
                    newpoly.Coordinates = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetPolygon();
                    newpoly.Color = Color.LightGray;
                    newpoly.StrokeColor = Color.LightGreen;
                    polylist.Add(newpoly);
                }
                    }

            MyMap.Polygons = polylist;
        }
        //unselects pin
        public void Cancel() {
            if (setpoly > -1) {
              
                        setpoly = -1;
                        CancelButton.IsVisible = false;
                        newpolygon = new List<Position>();
                        StartMap();
                        PolyMap();
                        MyMap.SelectedPin.DefaultPinColor = Color.Red;
                        MyMap.SelectedPin = null;
                        return;

            }
        }
        public void SavePlots() {
            SaveAll.GetInstance().SavePlots();
        }
    }
}