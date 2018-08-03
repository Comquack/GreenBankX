using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TK.CustomMap;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePlot : ContentPage
	{
        TKCustomMapPin newPin;
        List<TKCustomMapPin> Pins = new List<TKCustomMapPin>();
        public CreatePlot ()
		{
            StartMap();
            InitializeComponent ();
            }
        public void NewPlot() {
            //Plot newPlot = new Plot();
        }
        public void PlacePin(Position position)
        {
            MyMap.Pins = new List<TKCustomMapPin>();
            Pins.Add( new TKCustomMapPin
            {
                Position = position,
                Title = "TestPlot"+ Pins.Count.ToString(),
                IsVisible = true,
                ShowCallout = false
            });

            MyMap.Pins = Pins;
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