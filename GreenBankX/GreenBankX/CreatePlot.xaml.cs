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

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePlot : ContentPage
	{
        Boolean CanAdd = true;
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>(); 
        public CreatePlot ()
		{
            
          StartMap();
            InitializeComponent ();
            }
        public async Task NewPlot() {
            if (CanAdd == false)
            {
                double[] geo = new double[2];
                Application.Current.Properties["ThisLocation"] = new double[2];
                geo[0] = Pins.ElementAt(Pins.Count - 1).Position.Latitude;
                geo[1] = Pins.ElementAt(Pins.Count - 1).Position.Longitude;
                Application.Current.Properties["ThisLocation"] = geo;
                await PopupNavigation.PushAsync(new Popup());
                CanAdd = true;
            }
  
        }
        public void PlacePin(Position position)
        {
            if (CanAdd)
            {

            }
            else {
                Pins.RemoveAt(Pins.Count - 1);
            }
            MyMap.Pins = new List<TKCustomMapPin>();
            Pins.Add(new TKCustomMapPin
            {
                Position = position,
                Title = "TestPlot" + Pins.Count.ToString(),
                IsVisible = true,
                ShowCallout = false
            });
            MyMap.Pins = Pins;
            CanAdd = false;
        }
            public async void StartMap() {
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
                int num = ((List<Plot>)Application.Current.Properties["Plots"]).Count();
                for (int x=0; x < num; x++) {
                    double[] pinLoc = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(x).GetTag();
                    Position position = new Position(pinLoc[0], pinLoc[1]);
                    Pins.Add(new TKCustomMapPin
                    {
                        Position = position,
                        Title = "TestPlot" + Pins.Count.ToString(),
                        IsVisible = true,
                        ShowCallout = false
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

	}
}