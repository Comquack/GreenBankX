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
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    public partial class MeasureResult : Rg.Plugins.Popup.Pages.PopupPage
    {
        List<DetailsGraph2> store = new List<DetailsGraph2>();
        Geocoder Geoco;
        Plot EditPlot;
        int Priceno = -1;
        public static MeasureResult instance = new MeasureResult();
        public static MeasureResult GetInstance()
        {
            if (instance == null)
            {
                return new MeasureResult();
            }
            return instance;
        }
        public static MeasureResult GetInstance(List<DetailsGraph2> enter)
        {
            if (instance == null)
            {
                return new MeasureResult();
            }
            instance.store = enter;
            return instance;
        }
        private MeasureResult()
        {
            InitializeComponent();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }

        }



        protected override void OnAppearing()
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            string a = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth").Split('(')[0];
            string b = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter").Split(' ')[0];
            labbott.Text = (GirthDBH2.IsToggled ? a : textInfo.ToUpper(a)) + "/" + (GirthDBH2.IsToggled ? textInfo.ToUpper(b) : b);
            base.OnAppearing();
            LogList.ItemsSource = null;
            PriceA.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TotalPrice") + "(" + ((int)Application.Current.Properties["Currenselect"] == -1 ? "$" : ((List<(string, double)>)Application.Current.Properties["Currenlist"]).ElementAt((int)Application.Current.Properties["Currenselect"]).Item1) + ")";
            LogList.ItemsSource = store;
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

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private void GirthDBH2_Toggled(object sender, ToggledEventArgs e)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            string a = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth").Split('(')[0];
            string b = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter").Split(' ')[0];
            labbott.Text = (GirthDBH2.IsToggled ? a:textInfo.ToUpper(a))+"/"+ (GirthDBH2.IsToggled ? textInfo.ToUpper(b):b);
                if (LogList.IsVisible && LogList.ItemsSource != null)

            {
                List<DetailsGraph2> deets = (List<DetailsGraph2>)LogList.ItemsSource;
                List<DetailsGraph2> deets2 = new List<DetailsGraph2>();
                foreach (DetailsGraph2 answer in deets)
                {
                    SortedList<double, double> brack = answer.brack;
                    double[,] result = answer.result;
                    int i = answer.resultrow;
                    if (result == null) { answer.label = "Totals"; }
                    else if (result[i, 0] == -1)
                    {
                        //answer.label = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("TooSmall");
                    }
                    else if (result[i, 0] == brack.Count - 1)
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key * (!GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + "cm" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("OrLarger"));
                    }
                    else
                    {
                        answer.label = (Math.Round(brack.ElementAt((int)result[i, 0]).Key * (!GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + "-" + Math.Round(brack.ElementAt((int)result[i, 0] + 1).Key * (!GirthDBH2.IsToggled ? 1 / Math.PI : 1), 2) + "cm");

                    }
                    deets2.Add(answer);
                }
                LogList.ItemsSource = null;
                LogList.ItemsSource = deets2;
                LogList.IsVisible = true;
            }
        }


    }
}
