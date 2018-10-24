using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class AddMesPop : Rg.Plugins.Popup.Pages.PopupPage
    {
        int counter;
        int tCounter;
        int hist;
        public static AddMesPop instance = new AddMesPop();
        public static AddMesPop GetInstance()
        {
            if (instance == null)
            {
                return new AddMesPop();
            }
            return instance;
        }

        private AddMesPop()
        { 
            InitializeComponent();
        }
        public async void Done() {
            counter = (int)Application.Current.Properties["Counter"];
            tCounter = (int)Application.Current.Properties["TCounter"];
            hist = (int)Application.Current.Properties["HCounter"];
            if (MHeight.Text != null && double.Parse(MHeight.Text) > 0 && Diameter.Text != null && double.Parse(Diameter.Text) > 0 && Application.Current.Properties["Counter"] != null && (int)Application.Current.Properties["Counter"] > -1 && Application.Current.Properties["TCounter"] != null && (int)Application.Current.Properties["TCounter"] > -1 && DateMes.Date <= DateTime.Now)
            {
                bool editor = (DateMes.Date == ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ElementAt(hist).Key) && Edit.IsToggled;
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ContainsKey(DateMes.Date)&&!editor)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DateExists"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DateExistsB"), "OK");
                    });
                }
                else
                {
                    if (Edit.IsToggled)
                    {
                        ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().RemoveAt(hist);
                        ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).AddToHistory(double.Parse(Diameter.Text), double.Parse(MHeight.Text), DateMes.Date);
                         MessagingCenter.Send<AddMesPop>(this, "Alter");
                    }
                    else
                    {
                        ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).AddToHistory(double.Parse(Diameter.Text), double.Parse(MHeight.Text), DateMes.Date);
                        MessagingCenter.Send<AddMesPop>(this, "Append");
                    }
                    
                    
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            else if (DateMes.Date > DateTime.Now)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DFute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("input is invalid", "input" + AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("IsInvalid"), "OK");
                });
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        private void Diameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                Diameter.Text = e.OldTextValue;
            }
        }

        private void MHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 100 || double.Parse(e.NewTextValue) < 0))
            {
                MHeight.Text = e.OldTextValue;
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

        private void Edit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Edit.IsToggled)
            {
                TreeBut.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EMes");
                counter = (int)Application.Current.Properties["Counter"];
                tCounter = (int)Application.Current.Properties["TCounter"];
                hist = (int)Application.Current.Properties["HCounter"];
                DateMes.Date = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ElementAt(hist).Key;
                Diameter.Text = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ElementAt(hist).Value.Item1.ToString();
                MHeight.Text = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ElementAt(hist).Value.Item2.ToString();
            }
            else {
                TreeBut.Text = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("AddMeasurement");
                DateMes.Date = DateTime.Now;
                Diameter.Text = "";
                MHeight.Text = "";
            }
        }
    }
}
