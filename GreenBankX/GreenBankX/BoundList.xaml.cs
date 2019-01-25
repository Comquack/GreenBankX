using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using TK.CustomMap;
using Xamarin.Forms;

using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    public partial class BoundList : Rg.Plugins.Popup.Pages.PopupPage
    {
        int delno = -1;
        List<Position> newpolygon = new List<Position>();
        List<DetailsGraph2> store = new List<DetailsGraph2>();
        bool add = true;
        int plot = -1;
        double[] geo = null;


        public BoundList(int geol)
        {
            InitializeComponent();
            plot = geol;
            store = new List<DetailsGraph2>();
            geo = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(geol).GetTag();
            if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(geol).GetPolygon() != null) {
                for (int x = 0; ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(geol).GetPolygon().Count > x; x++) {
                    TK.CustomMap.Position ponit = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(geol).GetPolygon().ElementAt(x);
                    store.Add(new DetailsGraph2 { Lat = ponit.Latitude, Lon = ponit.Longitude });
                    newpolygon.Add(ponit);
                }
                LBound.ItemsSource = store;
            }
            
        }
        public BoundList(double[] geol)
        {
            plot = -1;
            store = new List<DetailsGraph2>();
            InitializeComponent();
            geo = geol;
            newpolygon = new List<Position>();
        }



        protected override void OnAppearing()
        {

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

        private void Button_Clicked(object sender, EventArgs e)
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
                bool addon = true;
                foreach (Position p in newpolygon) {
                    if (p.Latitude == latout && p.Longitude == lonout) {
                        addon = false;
                    }
                }
                if (!addon) {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), "Waypoint exists", "OK");
                    });
                    return;
                }
                geo = new double[] { latout, lonout };
                newpolygon.Add(new Position(latout, lonout));
                store.Add(new DetailsGraph2 { Lat = latout, Lon = lonout});
                LBound.ItemsSource = null;
                LBound.ItemsSource = store;
                Latent.Text = null;

            }
        }

        private async void Done_Clicked(object sender, EventArgs e)
        {
            List<double> lat = new List<double>();
            List<double> lon = new List<double>();
            if (newpolygon.Count() < 3)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NotEnoughPoints"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("fourpointBound"), "OK");
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
                if (plot > -1)
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(plot).AddPolygon(((List<TK.CustomMap.Position>)Application.Current.Properties["Bounds"]));
                }
                MessagingCenter.Send<BoundList>(this, "Add");
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

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private void LBound_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            delno = -1;
            foreach (Position p in newpolygon)
            {
                if (p.Latitude == ((DetailsGraph2)e.Item).Lat && p.Longitude == ((DetailsGraph2)e.Item).Lon)
                {
                    delno = newpolygon.IndexOf(p);
                    Del.IsVisible = true;
                }
            }
            
        }

        private void Del_Clicked(object sender, EventArgs e)
        {
            if (delno > -1) {
                newpolygon.RemoveAt(delno);
                store.RemoveAt(delno);
                LBound.ItemsSource = null;
                LBound.ItemsSource = store;
            }
           
        }
    }
}
