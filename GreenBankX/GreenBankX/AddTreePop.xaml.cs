﻿using System;
using System.Collections.Generic;
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
    public partial class AddTreePop : Rg.Plugins.Popup.Pages.PopupPage
    {
        int counter;
        public static AddTreePop instance = new AddTreePop();
        public static AddTreePop GetInstance()
        {
            if (instance == null)
            {
                return new AddTreePop();
            }
            return instance;
        }

        private AddTreePop()
        { 
            InitializeComponent();
        }
        public async void Done() {
            int ID;
            if (MHeight.Text != null && double.Parse(MHeight.Text) > 0 && Diameter.Text != null && double.Parse(Diameter.Text) > 0 && Application.Current.Properties["Counter"] != null && (int)Application.Current.Properties["Counter"] > -1 && DateMes.Date < DateTime.Now)
            {
                ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().Count-1).Id + 1;
                counter = (int)Application.Current.Properties["Counter"];
                ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).AddTree(new Tree(double.Parse(Diameter.Text), double.Parse(MHeight.Text), ID, DateMes.Date));
                Application.Current.Properties["Counter"] = -1;
                MessagingCenter.Send<AddTreePop>(this, "Add");
                await PopupNavigation.Instance.PopAsync();
            }
            else if (DateMes.Date > DateTime.Now) {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("DFute"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
            else if (((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).YearPlanted >= DateMes.Date.Year)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Date is Before Plot is Planted", AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("input is invalid", "input is invalid", "OK");
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
    }
}
