using System;
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
    public partial class AvgTreePop : Rg.Plugins.Popup.Pages.PopupPage
    {
        int counter;
        public static AvgTreePop instance = new AvgTreePop();
        public static AvgTreePop GetInstance()
        {
            if (instance == null)
            {
                return new AvgTreePop();
            }
            return instance;
        }

        private AvgTreePop()
        { 
            InitializeComponent();
        }
        public async void Done() {
            int ID;
            if (height.Text != null && double.Parse(height.Text) > 0 && girth.Text != null && double.Parse(girth.Text) > 0 && Application.Current.Properties["Counter"] != null && (int)Application.Current.Properties["Counter"] > -1 && DateMes.Date < DateTime.Now)
            {
                for (int q = 0; q < int.Parse(many.Text); q++) {
                    ID = ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().ElementAt(((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).getTrees().Count - 1).Id + 1;
                    counter = (int)Application.Current.Properties["Counter"];
                    ((List<Plot>)Application.Current.Properties["Plots"]).ElementAt(counter).AddTree(new Tree(double.Parse(girth.Text) * (GirthDBH.IsToggled ? Math.PI : 1), double.Parse(height.Text), ID, DateMes.Date));

                }
                Application.Current.Properties["Counter"] = -1;
                MessagingCenter.Send<AvgTreePop>(this, "Add");
                await PopupNavigation.Instance.PopAsync();
            } else if (int.TryParse(many.Text,out int mork) || mork < 1) {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NoTreeErr"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("NoTreeErr"), "OK");
                });
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
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("PastTree"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("EnterVDate"), "OK");
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("InputInv"), "OK");
                });
            }

        }
        protected override void OnAppearing()
        {
            many.Text = null;
            girth.Text = (((double)Application.Current.Properties["AvgGirth"])>-1)?((double)Application.Current.Properties["AvgGirth"]).ToString():null;
            height.Text = (((double)Application.Current.Properties["AvgH"]) > -1) ? ((double)Application.Current.Properties["AvgH"]).ToString() : null;
            base.OnAppearing();
            Application.Current.Properties["AvgH"] = -1;
            Application.Current.Properties["AvgGirth"] = -1;
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

        private void height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out double ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 100 || double.Parse(e.NewTextValue) < 0))
            {
                height.Text = e.OldTextValue;
            }
        }

        private void girth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out double ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) >= 1000 || double.Parse(e.NewTextValue) < 0))
            {
                girth.Text = e.OldTextValue;
            }
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                girth.Placeholder = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Diameter");
                if (girth.Text != null)
                {
                    girth.Text = (Math.Round(double.Parse(girth.Text) / Math.PI, 3)).ToString();
                }
            }
            else
            {
                girth.Placeholder = AppResource.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentCulture, true, true).GetString("Girth");
                if (girth.Text != null)
                {
                    girth.Text = (Math.Round(double.Parse(girth.Text) * Math.PI, 3)).ToString();
                }
            }
        }

        private void merchheight_TextChanged(object sender, TextChangedEventArgs e)
        {
            double ans;
            if (e.NewTextValue == null || e.NewTextValue == "") { }
            else if (e.NewTextValue != null && !double.TryParse(e.NewTextValue, out ans)) { }
            else if (e.NewTextValue != null && e.NewTextValue != "" && (double.Parse(e.NewTextValue) > 100 || double.Parse(e.NewTextValue) <= 0))
            {
                merchheight.Text = e.OldTextValue;
            }
            else if (e.NewTextValue != null && e.NewTextValue != "" && double.TryParse(height.Text, out ans) && ans < double.Parse(e.NewTextValue))
            {
                merchheight.Text = e.OldTextValue;
            }
        }

        private void MerhH_Toggled(object sender, ToggledEventArgs e)
        {
            merchheight.IsVisible = e.Value;
            merchheight.Text = e.Value ? "" : "";
        }
    }
}
