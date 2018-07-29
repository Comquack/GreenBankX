using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;

namespace GreenBankX
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreatePlot : ContentPage
	{
		public CreatePlot ()
		{
			InitializeComponent ();
            StartMap();


        }
        public async void StartMap() {
            try
            {
                var request = new  GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                { 
                    MyMap.MoveToRegion(
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