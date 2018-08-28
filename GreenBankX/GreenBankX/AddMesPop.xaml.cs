﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GreenBankX
{
    public partial class AddMesPop : Rg.Plugins.Popup.Pages.PopupPage
    {
        int counter;
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
            if (MHeight.Text != null && double.Parse(MHeight.Text) > 0 && Diameter.Text != null && double.Parse(Diameter.Text) > 0 && Application.Current.Properties["Counter"] != null && (int)Application.Current.Properties["Counter"] > -1 && Application.Current.Properties["TCounter"] != null && (int)Application.Current.Properties["TCounter"] > -1 && DateMes.Date <= DateTime.Now)
            {

                counter = (int)Application.Current.Properties["Counter"];
                int tCounter = (int)Application.Current.Properties["TCounter"];
                if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).GetHistory().ContainsKey(DateMes.Date))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Date exists", "Measurement exists for this date", "OK");
                    });
                }
                else
                {
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(tCounter).AddToHistory(float.Parse(Diameter.Text), float.Parse(MHeight.Text), DateMes.Date);
                    Application.Current.Properties["Counter"] = -1;
                    MessagingCenter.Send<AddMesPop>(this, "Append");
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            else if (DateMes.Date > DateTime.Now)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Date is in future", "Please input valid date", "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("input is invalid", "price is input", "OK");
                });
            }

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
    }
}
