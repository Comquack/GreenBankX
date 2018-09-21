using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    public partial class Popup : Rg.Plugins.Popup.Pages.PopupPage
    {
        Plot NextPlot;
        public static Popup instance = new Popup();
        public static Popup GetInstance()
        {
            if (instance == null)
            {
                return new Popup();
            }
            return instance;
        }

        private Popup()
        {
            InitializeComponent();
            for (int x = 0; x < ((List<PriceRange>)Application.Current.Properties["Prices"]).Count(); x++)
            {
                pickPrice.Items.Add(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(x).GetName());
            }
            pickPrice.Items.Add("Add New Pricing");
        }
        public async void Done()
        {
            if (PlotName.Text != null && int.Parse(PlotYear.Text) <= DateTime.Now.Year)
            {
                double[] geo = (double[])Application.Current.Properties["ThisLocation"];
                NextPlot = new Plot(PlotName.Text);
                NextPlot.SetTag(geo);
                if (pickPrice.SelectedIndex > -1 && pickPrice.SelectedIndex < pickPrice.Items.Count-1)
                {
                    NextPlot.SetRange(((List<PriceRange>)Application.Current.Properties["Prices"]).ElementAt(pickPrice.SelectedIndex));
                }
                else if (pickPrice.SelectedIndex == pickPrice.Items.Count)
                {
                    await Navigation.PushAsync(new CreatePricing());
                    return;
                }
                if (int.Parse(PlotYear.Text) != 0)
                {
                    NextPlot.YearPlanted = int.Parse(PlotYear.Text);
                }
                string api = "AIzaSyB7X3Ro62OQEySQtwQ5MInuwej0cVCaGAM";
                string lat = geo[0].ToString();
                string lng = geo[1].ToString();
                string baseurl = "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}";
                string requestUri = string.Format(baseurl, lat, lng, api);
                Console.WriteLine("Hello: 8===D~~~~~"+ requestUri);
               // using (WebClient wc = new WebClient())
                //{
                 //   wc.DownloadStringCompleted +=
                  //    new DownloadStringCompletedEventHandler(Wc_DownloadStringCompletedAsync);
                  //  wc.DownloadStringAsync(new Uri(requestUri));
               // }
                ((List<Plot>)Application.Current.Properties["Plots"]).Add(NextPlot);
                MessagingCenter.Send<Popup>(this, "Add");
                SaveAll.GetInstance().SavePlots();
                await PopupNavigation.Instance.PopAsync();
            }
            else if (PlotName.Text == null)
            {
                NameLabel.Text = AppResource.ResourceManager.GetString("EnterName");
            }
            else
            {
                NameLabel.Text = AppResource.ResourceManager.GetString("EnterVDate");
            }
           
        }



         async void Wc_DownloadStringCompletedAsync(object sender,
                       DownloadStringCompletedEventArgs e)
        {
            var xmlElm = XElement.Parse(e.Result);

            var status = (from elm in xmlElm.Descendants()
                          where elm.Name == "status"
                          select elm).FirstOrDefault();
            if (status.Value.ToLower() == "ok")
            {
                var res = (from elm in xmlElm.Descendants()
                           where elm.Name == "formatted_address"
                           select elm).FirstOrDefault();
                Console.WriteLine(res.Value);

            }
            else
            {
                Console.WriteLine("No Address Found");
            }
            ((List<Plot>)Application.Current.Properties["Plots"]).Add(NextPlot);
            MessagingCenter.Send<Popup>(this, "Add");
            await PopupNavigation.Instance.PopAsync();
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

        private async Task pickPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickPrice.SelectedIndex == pickPrice.Items.Count-1)
            {
                await Navigation.PushAsync(new CreatePricing());
                return;
            }
        }
    }
}
