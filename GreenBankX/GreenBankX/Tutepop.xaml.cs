using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreenBankX.Resources;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace GreenBankX
{
    public partial class Tutepop : Rg.Plugins.Popup.Pages.PopupPage
    {
        public static Tutepop instance = new Tutepop();
        private CultureInfo userSelectedCulture;

        public static Tutepop GetInstance()
        {
            //  if (instance == null)

            return new Tutepop();
          //  }
           // return instance;
        }

        private Tutepop()
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
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

        private void PriceB_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            dennis.HeightRequest = Application.Current.MainPage.Height / 2;
            dennis.WidthRequest = Application.Current.MainPage.Width;
            dennis.IsVisible = true;
            dennis.Source = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube1");
          //  Demmous.Text= AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube1");
        }

        private void PlotB_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            dennis.HeightRequest = Application.Current.MainPage.Height / 2;
            dennis.WidthRequest = Application.Current.MainPage.Width;
            dennis.IsVisible = true;
            dennis.Source = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube2");
          //  Demmous.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube2");
        }

        private void SummB_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties["Language"] != null)
            {
                Thread.CurrentThread.CurrentCulture = (CultureInfo)Application.Current.Properties["Language"];
            }
            dennis.HeightRequest = Application.Current.MainPage.Height / 2;
            dennis.WidthRequest = Application.Current.MainPage.Width;
            dennis.IsVisible = true;
            dennis.Source = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube3");
           // Demmous.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("zyoutube3");
        }

        async private void Close_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
