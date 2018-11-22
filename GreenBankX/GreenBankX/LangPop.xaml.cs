using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace GreenBankX
{
    public partial class LangPop : Rg.Plugins.Popup.Pages.PopupPage
    {
        public static LangPop instance = new LangPop();
        private CultureInfo userSelectedCulture;

        public static LangPop GetInstance()
        {
            if (instance == null)
            {
                return new LangPop();
            }
            return instance;
        }

        private LangPop()
        {
            
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

        private async void English_Clicked(object sender, EventArgs e)
        {
            userSelectedCulture = new System.Globalization.CultureInfo("en-AU");
            Thread.CurrentThread.CurrentCulture = userSelectedCulture;
            Application.Current.Properties["Language"] = new System.Globalization.CultureInfo("en-AU");
            MessagingCenter.Send<LangPop>(this, "Done");
            await PopupNavigation.Instance.PopAsync();
        }

        private async void Lao_Clicked(object sender, EventArgs e)
        {
            userSelectedCulture = new System.Globalization.CultureInfo("lo-LA");
            Application.Current.Properties["Language"] = new System.Globalization.CultureInfo("lo-LA");
            Thread.CurrentThread.CurrentCulture = userSelectedCulture;
            MessagingCenter.Send<LangPop>(this, "Done");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
