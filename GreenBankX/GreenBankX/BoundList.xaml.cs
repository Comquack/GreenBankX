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
            double[] geo = new double[] { 0, 0 };
            string[] splitter = Latent.Text.Split(',');
            if (Application.Current.Properties["ThisLocation"] == null && double.TryParse(splitter.ElementAt(0), out double latout) && double.TryParse(splitter.ElementAt(1), out double lonout))
            {
                if (latout > 90 || latout < -90 || lonout > 180 || lonout <= -180)
                {
                    return;
                }
                geo = new double[] { latout, lonout };
                newpolygon.Add(new Position(latout, lonout));
                store.Add(new DetailsGraph2 { Lat = latout, Lon = lonout});

            }
        }

        private void Done_Clicked(object sender, EventArgs e)
        {

        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private void LBound_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}
